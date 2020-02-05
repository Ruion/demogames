using UnityEngine;
using UnityEngine.UI;

public class PuzzleAlphabet : MonoBehaviour
{
    public string alphabet;
    public GameObject alphabetObj;
    public GameObject wrongImage;

    public void NotifyPuzzle()
    {
        PuzzleValidator.instance.ValidateAlphabet(this);
    }

    public void CorrectAnsweHandler()
    {
        alphabetObj.SetActive(true);

        FindObjectOfType<PuzzleInteraction>().Interact(transform);
        GetComponent<Button>().enabled = false;
    }

    public void WrongAnsweHandler()
    {
        alphabetObj.SetActive(false);
        wrongImage.SetActive(true);
        GetComponent<Button>().enabled = false;

    }
}
