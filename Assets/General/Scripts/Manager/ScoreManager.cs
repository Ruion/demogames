using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public ScoreVisualizer scoreVisualizer;

    public SoundManager soundManager;
    public Transform spawnTarget;
    public GameObject addScoreEffectPrefab;
    public GameObject minusScoreEffectPrefab;
    public ParticleSystem playerScoreEffectPrefab;
    public ParticleSystem playerMinusScoreEffectPrefab;

    [ReadOnly]
    public string scoreName = "score";

    private void Awake()
    {
        instance = this;

        PlayerPrefs.SetString(scoreName, "0");
    }

    public void AddScore(int amount)
    {
        SpawnScoreEffect(addScoreEffectPrefab);
        scoreVisualizer.UpdateText(amount);
        SpawnPlayerAddScoreEffect();
        soundManager.AddScore();
    }

    public void MinusScore(int amount)
    {
        SpawnScoreEffect(minusScoreEffectPrefab);
        scoreVisualizer.UpdateText(-amount);
        soundManager.MinusScore();
        SpawnPlayerMinusScoreEffect();
    }

    public void SpawnScoreEffect(GameObject effectPrefab)
    {
        GameObject vfx = Instantiate(effectPrefab, spawnTarget.position, Quaternion.identity);
    }

    public void SpawnPlayerAddScoreEffect()
    {
        playerScoreEffectPrefab.Clear();
        playerScoreEffectPrefab.Play();
    }

    public void SpawnPlayerMinusScoreEffect()
    {
        playerMinusScoreEffectPrefab.Clear();
        playerMinusScoreEffectPrefab.Play();
    }
}