using System.ComponentModel;

namespace Tools.Kubernetes;

public class KubernetesTools
{
    [Description("Get all Kubernetes pods and their statuses")]
    public async Task<string> GetPods()
    {
        var processInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "kubectl",
            Arguments = "get pods",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new System.Diagnostics.Process
        {
            StartInfo = processInfo
        };

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();

        return output;
    }
}