using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

    [Header("0 - HOME")]
    public int HOME = 0;
    [Header("1 - GAME")]
    public int GAME = 1;
    [Header("2 - SCORE")]
    public int SCORE = 2;

    public void SwitchScene(int sceneNumber)
    {
        Debug.Log("switch to scene " + sceneNumber);
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneNumber);

        // 0 HOME
        // 1 GAME
        // 2 SCORE
    }

    public void SwitchScene(string sceneName)
    {
        Debug.Log("switch to scene " + sceneName);
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);

        // 0 HOME
        // 1 GAME
        // 2 SCORE
    }
    public void RestartScene()
    {
        Debug.Log("restarting scene");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
