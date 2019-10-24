
public class RewardManager : GameSettingEntity
{
    public RewardFeature[] rewardFeatures;

    public override void Awake()
    {
        base.Awake();
        rewardFeatures = GetComponentsInChildren<RewardFeature>();
    }

    public void GiveReward()
    {
        LoadGameSettingFromMaster();

        switch (gameSettings.rewardType)
        {
            case RewardType.PrintVoucher:
            {
                    rewardFeatures[0].GiveReward();
                    break;
            }

            case RewardType.DropGift:
            {
                rewardFeatures[1].GiveReward();
                break;
            }

        }
    }

}

public enum RewardType
{
    PrintVoucher,
    DropGift,
   // EmailVoucher
}