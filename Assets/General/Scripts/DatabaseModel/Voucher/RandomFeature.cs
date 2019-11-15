using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBank;
using System;
using System.Data;
using System.Linq;

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
        try
        {
            voucher_probability = new List<ProbabilityCheck>();
            temp_total = 0;

            DataRowCollection drc = vdb.ExecuteCustomSelectQuery("SELECT * FROM " + vdb.dbSettings.localDbSetting.tableName + " WHERE quantity >= 1");

            int count = 0;
            foreach (DataRow dr in drc)
            {
                ProbabilityCheck temp = new ProbabilityCheck();
                voucher_probability.Add(temp);
                voucher_probability[count].id = System.Int32.Parse(dr[0].ToString());
                voucher_probability[count].min_prob = temp_total;
                temp_total += System.Int32.Parse(dr["quantity"].ToString());
                voucher_probability[count].max_prob = temp_total;
                voucher_probability[count].name = dr["name"].ToString();
                voucher_probability[count].quantity = System.Int32.Parse(dr["quantity"].ToString());
                count += 1;
            }
        }catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    [ContextMenu("CalculateProbability")]
    public ProbabilityCheck CalculateProbability()
    {
        try
        {
            MakeRandomProbability();

            int rand = UnityEngine.Random.Range(0, temp_total);

            ProbabilityCheck pc = voucher_probability
                .Where(x => rand > x.min_prob && rand < x.max_prob)
                .First();

            if (pc != null) { Debug.Log("Choosen voucher name : " + pc.name); return pc; }
            else { Debug.Log("Choosen voucher name : " + pc.name); return voucher_probability.LastOrDefault(); }
 
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }

        /*
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
        */
    }
}

public class ProbabilityCheck
{
    public int id;
    public int min_prob;
    public int max_prob;
    public string name;
    public int quantity;
}