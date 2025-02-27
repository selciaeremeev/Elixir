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

                // Extract device list from ADB output
                var lines = output.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                return lines.Skip(1) // Ignore the first line (header)
                            .Where(line => line.EndsWith("device"))
                            .Select(line => line.Split('\t')[0]) // Get only the device ID
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
    }
}
