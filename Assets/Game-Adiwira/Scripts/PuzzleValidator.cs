using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class PuzzleValidator : SerializedMonoBehaviour
{
    public int correctCharacter = 5;

    public static PuzzleValidator instance;

    public List<PuzzleAlphabet> correctAlphabets;

    public Dictionary<string, int> correctAlphabetDiction = new Dictionary<string, int>();

    public List<string> answerChar = new List<string>();

    public UnityEvent onCorrect;

    private SoundManager sm;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        sm = FindObjectOfType<SoundManager>();
    }

    public void ValidateAlphabet(PuzzleAlphabet alphabet)
    {
        if(correctAlphabets.Contains(alphabet) && correctAlphabetDiction[alphabet.alphabet] > 0)
        {
            // if click true alphabet
            alphabet.CorrectAnsweHandler();
            sm.AddScore();

            // add correct alphabet to list
            answerChar.Add(alphabet.alphabet);

            // exclude the true alphabet count
            correctAlphabetDiction[alphabet.alphabet]--;

        }
        else
        {
            // click on wrong alphabet
            alphabet.WrongAnsweHandler();
            sm.MinusScore();
        }

        // stop next checking when alphabet not same
        // if (answerChar.Count == correctCharacter && correctChar.All(answerChar.Contains))
        if (answerChar.Count == correctCharacter)
        {

            if (onCorrect.GetPersistentEventCount() > 0) onCorrect.Invoke();
            GameManager gm = FindObjectOfType<GameManager>();
            gm.isPass = true;
            gm.GameOver();
        }
    }
}
