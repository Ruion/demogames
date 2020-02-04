using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleLevelRandomizer : MonoBehaviour
{
    public int puzzleType = 8;

    public void LoadRandomPuzzleLevel()
    {
       FindObjectOfType<SceneSwitcher>().SwitchScene("PUZZLE" + (ChooseRandomPuzzle() + 1).ToString());
    }

    int ChooseRandomPuzzle()
    {
        int lastPuzzle = PlayerPrefs.GetInt("lastPuzzle", 1);

        int puzzle = Random.Range(0, puzzleType);

        while (puzzle == lastPuzzle)
        {
            puzzle = Random.Range(0, puzzleType);
        }

        PlayerPrefs.SetInt("lastPuzzle", puzzle);

        return puzzle;
    }
}
