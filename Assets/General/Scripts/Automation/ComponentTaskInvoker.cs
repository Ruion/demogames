using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ComponentTaskInvoker : MonoBehaviour
{
    protected bool isDebugging;

    public UnityEvent autoTasks;

    public void TriggerButtonClick(Button btn)
    {
        //if (!VerifyDebugMode()) return;

        btn.onClick.Invoke();
    }

    public void ExecuteAutoTasks()
    {
        //if (!VerifyDebugMode()) return;

        if (autoTasks.GetPersistentEventCount() > 0) autoTasks.Invoke();
    }

    protected bool VerifyDebugMode()
    {
        isDebugging = FindObjectOfType<GameSettingEntity>().gameSettings.debugMode;
        return isDebugging;
    }
}