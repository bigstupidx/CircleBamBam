using System;

public class CEnum {

	public enum EGameType : int {
        None = 0,
        Ball = 1,

        GroupNormal = 100,
        GroupNormal_1 = 101,
        GroupNormal_2 = 102,
        GroupNormal_3 = 103,
        GroupNormal_4 = 104,
        GroupNormal_5 = 105,

        GroupMedium = 200,
        GroupMedium_1 = 201,
        GroupMedium_2 = 202,
        GroupMedium_3 = 203,
        GroupMedium_4 = 204,
        GroupMedium_5 = 205,

        GroupHard = 300,
        GroupHard_1 = 301,
        GroupHard_2 = 302,
        GroupHard_3 = 303,
        GroupHard_4 = 304,
        GroupHard_5 = 305
    }

    public enum ELevel : int
    {
        Level_Normal = 0,
        Level_Medium = 100,
        Level_Hard = 200
    }

    public enum EGameState : int {
        None = 0,
        Idle = 1,
        Start = 2,
        End = 10
    }

    public enum ETrapMoveType : int
    {
        NoneMove = 0,
        MovePoint = 1,
        MoveAndRotatonPoint = 2,
        Rotation = 3,
        ScaleX = 4
    }

    public enum ECircleMoveType : int
    {
        RotationCenter = 0,
        MoveAndRotation = 1
    }

    public enum EScene : int
    {
        StartScene = 0,
        MainScene = 1
    }

    public enum ELanguage {
        EN,
        FR,
        VN,
        CN
    }

}
