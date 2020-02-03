using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleLevelRandomizer : MonoBehaviour
{
    public int puzzleType = 8;

    public void LoadRandomPuzzleLevel()
    {
        int puzzle = Random.Range(0, puzzleType);
        FindObjectOfType<SceneSwitcher>().SwitchScene("PUZZLE" + (puzzle+1).ToString());
    }
}
