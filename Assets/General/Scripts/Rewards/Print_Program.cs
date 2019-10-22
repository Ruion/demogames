using UnityEngine;
using System.Linq;

class Print_Program : MonoBehaviour
{
    public void Print()
    {
        var proc = new System.Diagnostics.Process();
        proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        proc.StartInfo.Verb = "print";

        // Using PDFtoPrinter
        proc.StartInfo.FileName = @"C:\Unity_Print\PDFtoPrinter.exe";
        proc.StartInfo.Arguments = string.Format(@"c:\Unity_Print\test.pdf");

        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.CreateNoWindow = true;

        proc.Start();

        if (proc.HasExited == false)
        {
            proc.WaitForExit(3000);
        }

        proc.EnableRaisingEvents = true;

        proc.Close();
    }

    private static bool KillAdobe(string name)
    {
        foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses().Where(
            process => process.ProcessName.StartsWith(name)))
        {
            process.Kill();
            return true;
        }
        return false;
    }
}