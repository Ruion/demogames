using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Settings to be used by gameplay objects
/// add variable like speed, hp, game time and make other class to use those values, to made tweaking and testing easier.
/// </summary>
[System.Serializable]
public class Settings
{
    [FolderPath(AbsolutePath=true, UseBackslashes = true)] 
    public string savePath;
    public string fileName = "GameSetting";
    public string scoreName = "game_score";
    public int scoreToWin = 3;
    public bool DebugMode = false;
 
    public Settings(Settings setting)
    {
      //  Debug.Log(setting.savePath);
     //   savePath = setting.savePath;
        
//        DebugMode = setting.DebugMode;

   //     fileName = setting.fileName;
     //   scoreName = setting.scoreName;
     //   scoreToWin = setting.scoreToWin;
    }
}
