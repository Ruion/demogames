using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBank;
using System;
using System.Data;

public class RandomFeature : MonoBehaviour
{
    public VoucherDBModelEntity vdb;
    public int temp_total = 0;
    public List<ProbabilityCheck> voucher_probability = new List<ProbabilityCheck>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public void MakeRandomProbability()
    {
        voucher_probability = new List<ProbabilityCheck>();
        temp_total = 0;

        DataRowCollection drc = vdb.ExecuteCustomSelectQuery("SELECT * FROM " + vdb.dbSettings.localDbSetting.tableName + " WHERE quantity >= 1");

        int count = 0;
        foreach (DataRow dr in drc)
        {

            ProbabilityCheck temp = new ProbabilityCheck();
            voucher_probability.Add(temp);
            voucher_probability[count].id = (int)dr[0];
            voucher_probability[count].min_prob = temp_total;
            temp_total += (int)dr["quantity"];
            voucher_probability[count].max_prob = temp_total;
            voucher_probability[count].type = dr["name"].ToString();
            count += 1;
        }
    }

    public void CalculateProbability()
    {
        int rand = UnityEngine.Random.Range(0, temp_total);
        int dd = 0;
        foreach (ProbabilityCheck s in voucher_probability)
        {
            if (rand >= s.min_prob && rand < s.max_prob)
            {
                vdb.ChosenVoucher = s.id;
                Debug.Log("Chosen Voucher is " + s.id + " & type is " + s.type);
            }
            dd += 1;
        }
        if(dd == 0)
        {
            vdb.ChosenVoucher = 4;
        }
    }
}

public class ProbabilityCheck
{
    public int id;
    public int min_prob;
    public int max_prob;
    public string type;
}