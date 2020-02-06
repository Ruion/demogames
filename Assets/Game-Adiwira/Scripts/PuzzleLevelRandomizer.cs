using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PuzzleLevelRandomizer : MonoBehaviour
{
    public int puzzleType = 8;

    public void LoadRandomPuzzleLevel()
    {
       FindObjectOfType<SceneSwitcher>().SwitchScene("PUZZLE" + ChooseRandomPuzzle().ToString());
    }

    [Button(ButtonSizes.Large)]
    int ChooseRandomPuzzle()
    {
        int lastPuzzle = PlayerPrefs.GetInt("lastPuzzle", 1);

        lastPuzzle++;

        int puzzle = lastPuzzle;

        if(puzzle > puzzleType) { puzzle = 1; lastPuzzle = 1; }

        Debug.Log(puzzle);

        PlayerPrefs.SetInt("lastPuzzle", puzzle);

        return puzzle;
    }
}
