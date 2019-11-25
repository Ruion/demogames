using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
    
/// <summary>
/// Class use by EventConditional to compare scriptableScore.score is lower or higher than score to win game.
/// </summary>
[System.Serializable]
public class ScoreCondition
{
    public string scoreName;
    public int score{ get{return int.Parse(PlayerPrefs.GetString(scoreName));} }
    public int minimumScore;
}

public class EventConditional : MonoBehaviour {
    public ScoreCondition[] scoreConditions;
    public bool conditionIsPass = true;
    public UnityEvent OnScoreCardPass;
    public UnityEvent OnScoreCardNotPass;
  

/// <summary>
/// Execute unity events base on score value of ScroreCardCondition[] objects. The score lower than minimumScore in any ScoreCardCondition count as not pass.
/// </summary>
	public void ExecuteScriptableScoreCondition()
    {
        for (int i = 0; i < scoreConditions.Length; i++)
        {
            if (scoreConditions[i].score < scoreConditions[i].minimumScore)
            {
                conditionIsPass = false;
            }            
        }

        if (conditionIsPass)
        {
           if( OnScoreCardPass.GetPersistentEventCount() > 0 ) OnScoreCardPass.Invoke();
        }
        else
        {
            OnScoreCardNotPass.Invoke();
            
            if (OnScoreCardNotPass.GetPersistentEventCount() > 0)OnScoreCardNotPass.Invoke();
        }
    }
}