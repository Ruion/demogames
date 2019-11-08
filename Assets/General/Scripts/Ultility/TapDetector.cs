using UnityEngine;
using UnityEngine.Events;

public class TapDetector : MonoBehaviour
{
    public UnityEvent OnTap;

    public bool executeOnce = true;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
           OnTap.Invoke();
           if (executeOnce) enabled = false;
         }
    }
}
