using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleAlphabet : MonoBehaviour, IPointerEnterHandler
{
    public string alphabet;
    public GameObject alphabetObj;
    public GameObject wrongImage;
    private bool validated = false;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (validated) return;

        PuzzleValidator.instance.ValidateAlphabet(this);

        validated = true;
    }
}
