using UnityEngine;
using System.Diagnostics;

public class ExecutableLauncher : MonoBehaviour
{
    public string executablePath;
    private bool launched = false;
    public bool launchOnceOnly = false;

    private Process proc;

    public string arguments;

    public void LaunchExecutable()
    {
        if (launchOnceOnly)
            if (Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(executablePath)).Length > 0)
                return;

        proc = new Process();
        proc.StartInfo.FileName = executablePath;
        proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        proc.StartInfo.CreateNoWindow = true;

        if (!string.IsNullOrEmpty(arguments)) proc.StartInfo.Arguments = arguments;

        proc.Start();
        //  System.Diagnostics.Process.Start(executablePath);
        launched = true;
    }

    private void OnApplicationQuit()
    {
        if (launched)
        {
            if (!proc.WaitForExit(1000))
                proc.Kill();
        }
    }
}