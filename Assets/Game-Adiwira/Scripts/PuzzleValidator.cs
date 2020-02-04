using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleValidator : MonoBehaviour
{
    public int correctCharacter = 5;

    public static PuzzleValidator instance;

    public List<PuzzleAlphabet> correctAlphabets;

    public List<PuzzleAlphabet> answerAlphabets;

    public HashSet<string> correctChar = new HashSet<string>();
    public List<string> answerChar = new List<string>();

    public UnityEvent onCorrect;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        foreach (PuzzleAlphabet w in correctAlphabets)
        {
            correctChar.Add(w.alphabet);
        }
    }

    public void ValidateAlphabet(PuzzleAlphabet alphabet)
    {
        if (answerAlphabets.Contains(alphabet))
        {
            answerAlphabets.Remove(alphabet);
            answerChar.Remove(alphabet.alphabet);
        }
        else
        {
            answerAlphabets.Add(alphabet);
            answerChar.Add(alphabet.alphabet);
        }

        // stop next checking when alphabet not same
        if (answerChar.Count == correctCharacter && correctChar.All(answerChar.Contains))
        {

            if (onCorrect.GetPersistentEventCount() > 0) onCorrect.Invoke();
            GameManager gm = FindObjectOfType<GameManager>();
            gm.isPass = true;
            gm.GameOver();
        }
    }
}
