using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private Image progressImage;

    private TimeManager tm;

    public void UpdateImage()
    {
        if (tm == null)
            tm = GetComponent<TimeManager>();

        progressImage.fillAmount = tm.countDownSeconds / tm.initialSecond;
    }
}