using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InternetChecker : MonoBehaviour
{
    public UnityEvent OnInternetConnection;
    public UnityEvent OnInternetConnectionLost;

    public void CheckInternet()
    {
        StartCoroutine(CheckInternetRoutine());
    }

    public void CheckInternetRepeat()
    {
        StartCoroutine(CheckInternetRoutineRepeat());
    }

    private IEnumerator CheckInternetRoutine()
    {
        yield return StartCoroutine(NetworkExtension.CheckForInternetConnectionRoutine());

        if (NetworkExtension.internet == false)
            if (OnInternetConnectionLost.GetPersistentEventCount() > 0)
                OnInternetConnectionLost.Invoke();
            else
           if (OnInternetConnection.GetPersistentEventCount() > 0)
                OnInternetConnection.Invoke();

        yield return null;
    }

    private IEnumerator CheckInternetRoutineRepeat()
    {
        yield return StartCoroutine(NetworkExtension.CheckForInternetConnectionRoutine());

        if (NetworkExtension.internet == false)
            if (OnInternetConnectionLost.GetPersistentEventCount() > 0)
                OnInternetConnectionLost.Invoke();

        yield return null;
    }
}