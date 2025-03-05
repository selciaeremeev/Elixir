using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Elixir.Helpers
{
    public static class AdbHelper
    {
        public static string[] GetConnectedDevices()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C adb devices",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process process = new Process { StartInfo = psi };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                var lines = output.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                return lines.Skip(1)
                            .Where(line => line.EndsWith("device"))
                            .Select(line => line.Split('\t')[0])
                            .ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving ADB devices: {ex.Message}");
                return new string[0];
            }
        }

        public static bool CaptureScreen(string deviceId, string savePath)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C adb -s {deviceId} shell screencap -p /sdcard/screenshot.png && adb -s {deviceId} pull /sdcard/screenshot.png \"{savePath}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process process = new Process { StartInfo = psi };
                process.Start();
                process.WaitForExit();

                return File.Exists(savePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error capturing screen: {ex.Message}");
                return false;
            }
        }

        public static Process StartLogcat(string deviceId)
        {
            try
            {
                var logcatProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "adb",
                        Arguments = $"-s {deviceId} logcat",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                logcatProcess.Start();
                logcatProcess.BeginOutputReadLine();

                return logcatProcess;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error starting logcat: {ex.Message}");
                return null;
            }
        }

        // Logcat をクリア
        public static bool ClearLogcat(string deviceId)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C adb -s {deviceId} logcat -c",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process process = new Process { StartInfo = psi };
                process.Start();
                process.WaitForExit();
                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error clearing logcat: {ex.Message}");
                return false;
            }
        }

        // Logcat を終了
        public static bool StopLogcat(Process logcatProcess)
        {
            try
            {
                if (logcatProcess != null && !logcatProcess.HasExited)
                {
                    logcatProcess.Kill();
                    logcatProcess.WaitForExit();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping logcat: {ex.Message}");
                return false;
            }
        }
    }
}
