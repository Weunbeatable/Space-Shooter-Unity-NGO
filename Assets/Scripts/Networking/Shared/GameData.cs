using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Map
{
    Default
}

public enum GameMode
{
    Default
}

public enum GameQeue
{
    Solo,
    Team
}
[Serializable]
public class UserData 
{
    public string userName;

    public string userAuthId;

    public GameInfo userGamePreferences;
}

[Serializable]
public class GameInfo
{
    public Map map;
    public GameMode gameMode;
    public GameQeue gameQeue;

    public string ToMultiplayQueue()
    {
        return "";
    }
}
