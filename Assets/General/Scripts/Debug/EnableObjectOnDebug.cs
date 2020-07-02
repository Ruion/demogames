using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjectOnDebug : MonoBehaviour
{
    public GameObject[] gameObjectsToEnable;

    // Start is called before the first frame update
    private void Start()
    {
        if (FindObjectOfType<GameSettingEntity>().gameSettings.debugMode)
        {
            for (int g = 0; g < gameObjectsToEnable.Length; g++)
            {
                if (!gameObjectsToEnable[g].activeSelf)
                    gameObjectsToEnable[g].SetActive(true);
            }
        }
        else
        {
            for (int g = 0; g < gameObjectsToEnable.Length; g++)
            {
                if (gameObjectsToEnable[g].activeSelf)
                    gameObjectsToEnable[g].SetActive(false);
            }
        }
    }
}