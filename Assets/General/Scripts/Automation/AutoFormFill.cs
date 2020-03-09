using UnityEngine;
using TMPro;

public class AutoFormFill : ComponentTaskInvoker
{
    public DBModelEntity db;
    public TMP_InputField field;

    public void AutoFillField()
    {
        VerifyDebugMode();

        if (!isDebugging) return;

        string redeem_code = db.ExecuteCustomSelectQuery(
           string.Format("SELECT * FROM {0} WHERE voucher_status = 'ready' LIMIT 1", db.dbSettings.tableName))
            [0]["redeem_code"].ToString();

        field.text = redeem_code;
    }

    public void AutoFillName()
    {
        VerifyDebugMode();

        if (!isDebugging) return;

        int count = PlayerPrefs.GetInt("playerNameCount", 1);

        field.text = "demo" + count.ToString();

        PlayerPrefs.SetInt("playerNameCount", count + 1);
    }

    public void AutoFillEmail()
    {
        VerifyDebugMode();

        if (!isDebugging) return;

        int count = PlayerPrefs.GetInt("playerEmailCount", 1);

        field.text = "demo" + count + "@gmail.com";

        PlayerPrefs.SetInt("playerEmailCount", count + 1);
    }

    public void AutoFillContact()
    {
        VerifyDebugMode();

        if (!isDebugging) return;

        int replaceIndex = PlayerPrefs.GetInt("playerContactReplaceCount", 1); ;
        string contact = "0000000";

        int count = PlayerPrefs.GetInt("playerContactCount", 1);

        // remove and replace the string in specific index
        string finalContact = contact.Remove(replaceIndex, 1).Insert(replaceIndex, count.ToString());

        field.text = finalContact;

        PlayerPrefs.SetInt("playerContactCount", count + 1);

        if (count > 9)
        {
            PlayerPrefs.SetInt("playerContactReplaceCount", replaceIndex + 1);
            PlayerPrefs.SetInt("playerContactCount", 0);
        }
    }
}