using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DisableWakeArmedDevices.ClassLibrary
{
    public static class WakeDevice
    {
        public static void DisableDevicesWake(IEnumerable<string> devices)
        {
            foreach (var device in devices)
            {
                var proc = Process.Start(new ProcessStartInfo
                {
                    FileName = "powercfg",
                    Arguments = $"/devicedisablewake \"{device}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                });

                proc!.WaitForExit();
            }
        }

        public static List<string> GetDevicesWithWake()
        {
            var powerCfgProc = Process.Start(new ProcessStartInfo
            {
                FileName = "powercfg",
                Arguments = "/devicequery wake_armed",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });

            var devices = new List<string>();

            while (!powerCfgProc!.StandardOutput.EndOfStream)
            {
                var device = powerCfgProc.StandardOutput.ReadLine();

                if (!string.IsNullOrWhiteSpace(device)
                    && !device.Equals("NONE", StringComparison.InvariantCultureIgnoreCase))
                    devices.Add(device);
            }

            return devices;
        }
    }
}