using UnityEngine;
using UnityEngine.Events;

public class SelectableOption : MonoBehaviour
{
    private bool selected = false;

    [SerializeField]
    private GameObject selectedImage;

    [SerializeField]
    private UnityEvent onSelected;

    public void Select()
    {
        if (selected)
        {
            selected = false;
            selectedImage.SetActive(false);
        }
        else
        {
            selected = true;
            selectedImage.SetActive(true);
            if (onSelected.GetPersistentEventCount() > 0) onSelected.Invoke();
        }
    }
}