using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using System.IO;
using Facebook;

public class CSocialNetworkManager : MonoBehaviour {

    #region Singleton

    private static CSocialNetworkManager m_Instance;
    private static object m_SingtonObject = new object();

    public static CSocialNetworkManager Instance
    {
        get
        {
            lock (m_SingtonObject)
            {
                if (m_Instance == null)
                {
                    var go = Instantiate(Resources.Load<GameObject>("Prefabs/SocialNetworkManager"));
                    m_Instance = go.GetComponent<CSocialNetworkManager>();
                }
                return m_Instance;
            }
        }
    }

    #endregion

    #region Properties

    private static Dictionary<string, string> m_ScoreFormData;

    public enum CoverImageType
    {
        small, normal, album, large, square
    }

    #endregion

    #region Implementation MonoBehaviour

    private void Awake()
    {
        m_Instance = this;
        DontDestroyOnLoad(this.gameObject);
        m_ScoreFormData = new Dictionary<string, string>();
    }

    private void Start() {
        FB.Init(this.OnInitComplete, this.OnHideUnity);
    }

    #endregion

    #region Main methods

    private void OnInitComplete()
    {
#if UNITY_EDITOR
        Debug.Log("FB: Init complete");
#endif
        if (FB.IsLoggedIn == false)
        {
            // FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, this.HandleResult);
            // FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends", "publish_actions" }, this.HandleResult);
        }
        else
        {
            // FB.ShareLink(new Uri("https://developers.facebook.com/"), callback: this.HandleResult);
        }
    }

    private void OnHideUnity(bool isGameShown)
    {

    }

    private void HandleResult (IResult result, Action success = null)
    {
        var stringResult = string.Empty;
        if (result == null)
        {
            stringResult = "Null Response\n";
            Debug.Log(stringResult);
            return;
        }

        if (!string.IsNullOrEmpty(result.Error))
        {
            stringResult = "Error - Check log for details";
            stringResult += "Error Response:\n" + result.Error;
        }
        else if (result.Cancelled)
        {
            stringResult = "Cancelled - Check log for details";
            stringResult += "Cancelled Response:\n" + result.RawResult;
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            stringResult = "Success - Check log for details";
            stringResult += "Success Response:\n" + result.RawResult;
            if (success != null)
            {
                success();
            }
        }
        else
        {
            stringResult = "Empty Response\n";
        }
        Debug.Log(stringResult);
    }

    private void HandleLogIn(Action complete) {
        if (FB.IsLoggedIn)
        {
            if (complete != null)
            {
                complete();
            }
        }
        else
        {
            //FB.LogInWithReadPermissions(new List<string>() {"public_profile", "email", "user_friends", "publish_actions" }, (result) => {
            //    this.HandleResult(result, complete);
            //});
            FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, (result) => {
                this.HandleResult(result, complete);
            });
        }
    }

    public void ShareLinkFB()
    {
        FB.ShareLink(new Uri(CUltilities.APP_URL),
                    callback: (result) => {
                        Debug.Log(result.RawResult);
                    });
    }

    public void ShareFB(string title, string caption, string description, string imageURL = "")
    {
        FB.FeedShare(
                    string.Empty,
                    new Uri(CUltilities.APP_URL),
                    title,
                    caption,
                    description,
                    new Uri(imageURL),
                    string.Empty,
                    callback: (result) => {
                        Debug.Log(result.RawResult);
                    });
    }

    public void GetLeaderboard(int limit, Action<List<object>> callback) {
        HandleLogIn(() =>
        {
            FB.API("/app/scores?fields=score,user.limit("+limit+")", HttpMethod.GET, callback: (result) =>
            {
                if (callback != null)
                {
                    var desericalizeResult = MiniJSON.Json.Deserialize(result.RawResult) as Dictionary<string, object>;
                    callback(desericalizeResult["data"] as List<object>);
                }
            });
        });
    }

    public void GetMeScore(Action<CScoreParser> callBack)
    {
        HandleLogIn(() =>
        {
            FB.API("/me/scores", HttpMethod.GET, (scoreResult) =>
            {
                if (callBack != null)
                {
                    var desericalizeResult = MiniJSON.Json.Deserialize(scoreResult.RawResult) as Dictionary<string, object>;
                    var dataResult = desericalizeResult["data"] as List<object>;
                    if (dataResult.Count > 0)
                    {
                        var data = dataResult[0] as Dictionary<string, object>;
                        var score = CScoreParser.Parser(data);
                        callBack(score);
                    }
                }
            });
        });
    }

    public void SetNewScore(float score, Action<string> callBack)
    {
        HandleLogIn(() =>
        {
            m_ScoreFormData["score"] = score.ToString();
            FB.API("/me/scores", HttpMethod.POST, (scoreResult) =>
            {
                if (callBack != null)
                {
                    callBack(scoreResult.RawResult);
                }
            }, m_ScoreFormData);
        });
    }

    public void GetCoverImage(CoverImageType type, string userID, Action<string> callback)
    {
        HandleLogIn(() =>
        {
            FB.API("/" + userID + "/picture?redirect=false&type=" + type.ToString(), HttpMethod.GET, callback: (result) =>
            {
                if (callback != null)
                {
                    var data = MiniJSON.Json.Deserialize(result.RawResult) as Dictionary<string, object>;
                    callback((data["data"] as Dictionary<string, object>)["url"].ToString());
                }
            });
        });
    }

    public void CreateTexture(string url, Action<Texture> callback, bool saveFileLocal = false, string fileName = "")
    {
        StartCoroutine(HandleCreateTexture(url, callback, saveFileLocal, fileName));
    }

    private IEnumerator HandleCreateTexture(string url, Action<Texture> callback, bool saveFileLocal = false, string fileName = "")
    {
        var www = new WWW(url);
        while (!www.isDone)
        {
            yield return null;
        }
        yield return www.isDone;
        if (callback != null)
        {
            var path = Application.persistentDataPath + "/" + fileName + ".jpg";
            Texture2D texture = null;
            if (File.Exists(path))
            {
                var data = File.ReadAllBytes(path);
                texture = new Texture2D(50, 50);
                texture.LoadImage(data);
                texture.Apply();
            }
            else
            {
                texture = www.texture;
                if (saveFileLocal)
                {
                    File.WriteAllBytes(path, texture.EncodeToJPG());
                }
            }
            callback(texture);
        }
    }

    #endregion

}
