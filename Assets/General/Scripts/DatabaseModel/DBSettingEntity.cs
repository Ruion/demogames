using Sirenix.OdinInspector;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

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
              
        // fetch & Update setting from Setting.json
        JSONSetter jsonSetter = FindObjectOfType<JSONSetter>();
        dbSettings.folderPath = jsonSetter.savePath;
        
        // add api to Setting.json - playerdata_sendAPI : submit-player-data
        DBSettingEntity[] dBSettingEntities = FindObjectsOfType<DBSettingEntity>();
        foreach (DBSettingEntity e in dBSettingEntities)
        {
            if(string.IsNullOrEmpty(e.dbSettings.sendAPI)) continue;
            
            jsonSetter.UpdateSetting(e.dbSettings.fileName+ "-API", e.dbSettings.sendAPI);
        }

        // legacy binary formatter save method
        // DBSetting.SaveSetting(dbSettings);

        // save to json file
        JSONExtension.SaveObject(dbSettings.folderPath + "\\" + name, dbSettings);
    }

    [Button(ButtonSizes.Large), GUIColor(.3f, .78f, .78f)][ButtonGroup("Setting")]
    public virtual void LoadSetting()
    {
        // fetch & Update setting from global JSONSetter
        JSONSetter jsonSetter = FindObjectOfType<JSONSetter>();
        dbSettings.folderPath = jsonSetter.savePath;

        string filePath = dbSettings.folderPath + "\\" + name;
       
       // Load from json file
       dbSettings = JsonConvert.DeserializeObject<DBEntitySetting>(File.ReadAllText(filePath + ".json"));

        // fetch & Update setting from global JSONSetter
        JObject jObject = jsonSetter.LoadSetting();
        dbSettings.sendURL = jObject["ServerDomainURL"].ToString();

        // load sendAPI from global setting file
        if(jObject.ContainsKey(dbSettings.fileName+"-API")) dbSettings.sendAPI = jObject[dbSettings.fileName+"-API"].ToString();
    }
    #endregion

}
