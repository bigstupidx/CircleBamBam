using System.Collections.Generic;

public class CScoreParser {

    public int Score;
    public string NameUser;
    public string ID;

    public CScoreParser()
    {

    }

    public static CScoreParser Parser(Dictionary<string, object> value)
    {
        var parse = new CScoreParser();
        var dataUser = value["user"] as Dictionary<string, object>;
        parse.Score = int.Parse (value["score"].ToString());
        parse.NameUser = dataUser["name"].ToString().ToUTF8();
        parse.ID = dataUser["id"].ToString();
        return parse;
    }

}
