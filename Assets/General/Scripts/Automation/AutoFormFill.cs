using UnityEngine;
using TMPro;

public class AutoFormFill : ComponentTaskInvoker
{
    public DBModelEntity db;
    public TMP_InputField field;

    [SerializeField]
    private bool disableRecordAfterUse = false;

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

        string name = db.ExecuteCustomSelectObject("SELECT name FROM " + db.dbSettings.tableName + " WHERE is_enabled = 'true'").ToString();

        field.text = name;

        if (disableRecordAfterUse)
            db.ExecuteCustomNonQuery(string.Format("UPDATE {0} SET is_enabled ='false' WHERE name = '{1}'", db.dbSettings.tableName, name));
    }

    public void AutoFillEmail()
    {
        VerifyDebugMode();

        if (!isDebugging) return;

        string email = db.ExecuteCustomSelectObject("SELECT email FROM " + db.dbSettings.tableName + " WHERE is_enabled = 'true'").ToString();

        field.text = email;

        if (disableRecordAfterUse)
            db.ExecuteCustomNonQuery(string.Format("UPDATE {0} SET is_enabled ='false' WHERE email = '{1}'", db.dbSettings.tableName, email));
    }

    public void AutoFillContact()
    {
        VerifyDebugMode();

        if (!isDebugging) return;

        string contact = db.ExecuteCustomSelectObject("SELECT contact FROM " + db.dbSettings.tableName + " WHERE is_enabled = 'true'").ToString();

        field.text = contact;

        if (disableRecordAfterUse)
            db.ExecuteCustomNonQuery(string.Format("UPDATE {0} SET is_enabled ='false' WHERE contact = '{1}'", db.dbSettings.tableName, contact));
    }
}