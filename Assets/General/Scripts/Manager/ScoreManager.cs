using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance;

    public ScoreVisualizer scoreVisualizer;

    public SoundManager soundManager;
    public Transform player;
    public Transform scoreEffectContainer;
    public GameObject addScoreEffectPrefab;
    public GameObject minusScoreEffectPrefab;

    [ReadOnly]
    public string scoreName = "game_score";

    void Awake()
    {
        instance = this;

        GameSettingEntity dm = GameObject.Find("GameSettingEntity_DoNotChangeName").GetComponent<GameSettingEntity>();
        dm.LoadSetting();

        if(dm == null)
        {
            Debug.LogError("GameSettingEntity not found in scene");
        }
        else
        {
            scoreName = dm.gameSettings.scoreName;
        }

        PlayerPrefs.SetString(scoreName, "0");
    }

	public void AddScore(int amount)
    {
        soundManager.AddScore();
        SpawnScoreEffect(addScoreEffectPrefab);
       scoreVisualizer.UpdateText(amount);             
    }

    public void MinusScore(int amount)
    {
        soundManager.MinusScore();
        SpawnScoreEffect(minusScoreEffectPrefab);
        scoreVisualizer.UpdateText(-amount);       
    }

    public void SpawnScoreEffect(GameObject effectPrefab)
    {
        Instantiate(effectPrefab, player.position + Vector3.up , Quaternion.identity, scoreEffectContainer);
    }

}
