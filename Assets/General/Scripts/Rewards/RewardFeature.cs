using UnityEngine;

public class RewardFeature : GameSettingEntity
{
    
    public virtual void GiveReward()
    {
        LoadGameSettingFromMaster();
        throw new System.NotImplementedException("The requested feature is not implemented.");
    }
}
