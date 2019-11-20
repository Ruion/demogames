using UnityEngine;

public class GameSettingEntity : MonoBehaviour
{
    [Header("GameSetting - SAVE setting every new project")]
    public Settings gameSettings;

    [ContextMenu("SaveSetting")]
    public virtual void SaveSetting()
    {
        GameSetting.SaveSetting(gameSettings);
    }

    [ContextMenu("LoadSetting")]
    public virtual void LoadSetting()
    {
        gameSettings = GameSetting.LoadSetting(gameSettings.fileName);
    }

    [ContextMenu("LoadMasterSetting")]
    public virtual void LoadGameSettingFromMaster()
    {
        GameSettingEntity dm = GameObject.Find("GameSettingEntity_DoNotChangeName").GetComponent<GameSettingEntity>();
        if (dm == this) return;

        dm.LoadSetting();
        gameSettings = dm.gameSettings;

    }

    public virtual void Awake()
    {
        LoadGameSettingFromMaster();
    }
}
