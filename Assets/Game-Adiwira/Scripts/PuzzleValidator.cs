using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleValidator : MonoBehaviour
{
    public static PuzzleValidator instance;

    public List<PuzzleAlphabet> correctAlphabets;

    public List<PuzzleAlphabet> answerAlphabets;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void ValidateAlphabet(PuzzleAlphabet alphabet)
    {
        if (answerAlphabets.Contains(alphabet))
            answerAlphabets.Remove(alphabet);
        else answerAlphabets.Add(alphabet);

        // stop next checking when alphabet not same
        if (answerAlphabets.Count != correctAlphabets.Count) return;

        // when get same element, win
        if(answerAlphabets.All(correctAlphabets.Contains))
        {
            GameManager gm = FindObjectOfType<GameManager>();
            gm.isPass = true;
            gm.GameOver();
        }
    }
}
