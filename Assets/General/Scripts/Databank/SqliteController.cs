using System.Data;
using UnityEngine;
using DataBank;
using TMPro;
using System.Data.Common;

public class SqliteController : MonoBehaviour
{
    public Transform list;
    public GameObject prefab;
    public string dbName = "user_db_honda";
    public string tableName = "user";

    public GameObject errorRetrieveHandler;
    public TextMeshProUGUI errorRetrieveHandlerText;

    IDataReader reader;

    [ContextMenu("ShowAll")]
    void OnEnable()
    {
        GameSettingEntity dm = GameObject.Find("GameSettingEntity_DoNotChangeName").GetComponent<GameSettingEntity>();
        dm.LoadSetting();
        dbName = dm.gameSettings.dbName;
        tableName = dm.gameSettings.tableName;

        UserDB userDb = new UserDB(dbName, tableName);

        try { reader = userDb.GetAllData(); }
        catch(DbException ex) { errorRetrieveHandler.SetActive(true); errorRetrieveHandlerText.text = ex.Message; }

        while (reader.Read())
        {
            UserEntity entity = new UserEntity();
            entity.name = reader[1].ToString();
            entity.email = reader[2].ToString();
            entity.phone = reader[3].ToString();
            entity.score = reader[4].ToString();
            entity.register_datetime = reader[5].ToString();
            entity.is_submitted = reader[6].ToString();

            GameObject newR = Instantiate(prefab, list);
            TextMeshProUGUI text = newR.GetComponent<TextMeshProUGUI>();
            text.text = reader[0].ToString();

            TextMeshProUGUI nameT = newR.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            nameT.text = entity.name;

            TextMeshProUGUI emailT = newR.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            emailT.text = entity.email;

            TextMeshProUGUI phoneT = newR.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            phoneT.text = entity.phone;

            TextMeshProUGUI scoreT = newR.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
            scoreT.text = entity.score;

            TextMeshProUGUI registerT = newR.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
            registerT.text = entity.register_datetime;

            TextMeshProUGUI syncT = newR.transform.GetChild(5).GetComponent<TextMeshProUGUI>();
            syncT.text = entity.is_submitted;

        }

    }

    private void OnDisable()
    {
        foreach(Transform child in list)
        {
            Destroy(child.gameObject);
        }
    }

}
