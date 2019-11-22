using UnityEngine;

/// <summary>
/// Destroy a target gameObject after a delay at OnEnable()
/// Tips: Attach to a gameObject, drag target gameObject into "target" field
/// </summary>
public class TimerDestroyer : MonoBehaviour
{
    public float DestroyAfterSeconds;
    public GameObject target;

    private void OnEnable()
    {
        Invoke("DestroyTarget", DestroyAfterSeconds);
    }

    void DestroyTarget()
    {
       Destroy(target);
    }


}
