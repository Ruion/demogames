using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderTextVisualizer : MonoBehaviour
{
    public TextMeshProUGUI text_;
    private Slider slider_;

    private void OnEnable()
    {
        slider_ = GetComponent<Slider>();
    }

    public void VisualizeSliderValue()
    {
        text_.text = slider_.value.ToString();
    }
}
