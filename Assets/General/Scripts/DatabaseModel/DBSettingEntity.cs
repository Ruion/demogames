using Sirenix.OdinInspector;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

/// <summary>
/// Base class for DBModelMaster
/// Tips: Inherit this class to allow accessing database setting
/// </summary>
public class DBSettingEntity : SerializedMonoBehaviour
{
    public DBEntitySetting dbSettings;

    #region Basics
    public virtual void Awake()
    {
       // LoadSetting();
    }

    [Button(ButtonSizes.Large), GUIColor(.3f, .78f, .78f)][ButtonGroup("Setting")]
    public virtual void SaveSetting()
    {
              
        // fetch & Update setting from global JSONSetter
        JSONSetter jsonSetter = FindObjectOfType<JSONSetter>();
        dbSettings.folderPath = jsonSetter.savePath;

        // add url to global setting - sendURL : http://domain.com/public/api
        jsonSetter.UpdateSetting("sendURL", dbSettings.sendURL);
        
        // add api to global setting - playerdata_sendAPI : submit-player-data
        DBSettingEntity[] dBSettingEntities = FindObjectsOfType<DBSettingEntity>();
        foreach (DBSettingEntity e in dBSettingEntities)
        {
            if(string.IsNullOrEmpty(e.dbSettings.sendAPI)) continue;
            
            jsonSetter.UpdateSetting(e.dbSettings.fileName+ "-API", e.dbSettings.sendAPI);
        }


        DBSetting.SaveSetting(dbSettings);
        LoadSetting();
    }

    [Button(ButtonSizes.Large), GUIColor(.3f, .78f, .78f)][ButtonGroup("Setting")]
    public virtual void LoadSetting()
    {
        // fetch & Update setting from global JSONSetter
        JSONSetter jsonSetter = FindObjectOfType<JSONSetter>();
        dbSettings.folderPath = jsonSetter.savePath;

        dbSettings = DBSetting.LoadSetting(dbSettings.folderPath + "\\" + dbSettings.fileName);
       
        // fetch & Update setting from global JSONSetter
        JObject jObject = jsonSetter.LoadSetting();
        dbSettings.sendURL = jObject["sendURL"].ToString();

        if(jObject.ContainsKey(dbSettings.fileName+"-API")) dbSettings.sendAPI = jObject[dbSettings.fileName+"-API"].ToString();
    }
    #endregion

}
