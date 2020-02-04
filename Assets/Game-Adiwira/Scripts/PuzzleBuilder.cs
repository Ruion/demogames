using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class PuzzleBuilder : SerializedMonoBehaviour
{
    public Dictionary<string, Sprite> alphabetImages = new Dictionary<string, Sprite>();
    [SerializeField]private PuzzleAlphabet[] puzzleAlphabets;

    private void Start()
    {
        Build();
    }

    [Button(ButtonSizes.Large)]
    void Build()
    {
        puzzleAlphabets = GetComponentsInChildren<PuzzleAlphabet>();
        foreach (PuzzleAlphabet p in puzzleAlphabets)
        {
          //  Debug.Log(p.alphabet);
            p.transform.GetChild(0).GetComponent<Image>().sprite = alphabetImages[p.alphabet];

            p.name = "Button-" + p.alphabet;
        }
    }

}
