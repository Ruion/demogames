using UnityEngine;

public class PuzzleAlphabet : MonoBehaviour
{
    public string alphabet;
    public GameObject alphabetObj;

    public void NotifyPuzzle()
    {
        PuzzleValidator.instance.ValidateAlphabet(this);

        if (alphabetObj.activeSelf)
            alphabetObj.SetActive(false);
        else alphabetObj.SetActive(true);
    }
}
