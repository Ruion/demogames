using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

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
