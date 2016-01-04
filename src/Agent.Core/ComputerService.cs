using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace Geekors.Infra.WatchDog.Core
{
    public class ComputerService
    {
        PerformanceCounter ramCounter;
        PerformanceCounter cpuCounter;
        Process currentProcess;
        ManagementObjectSearcher wmiSearcher;

        public ComputerService()
        {
            InitEnvironmentSetting();
        }

        void InitEnvironmentSetting()
        {
            wmiSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes", true);
            cpuCounter = new PerformanceCounter();
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            cpuCounter.CategoryName = "Processor";

            currentProcess = Process.GetCurrentProcess();
        }

        public IEnumerable<string> GetCPUs()
        {
            foreach (ManagementObject obj in wmiSearcher.Get())
            {
                // 取得CPU 序號
                //Console.WriteLine("CPU{0} ID:\t{1}", i++, obj["ProcessorId"].ToString());
                yield return obj["ProcessorId"].ToString();
            }

            yield break;
        }

        /// <summary>
        /// 取得目前電腦狀態及程序狀態
        /// </summary>
        /// <returns></returns>
        public ComputerStatus GetStatus()
        {
            ComputerStatus status = new ComputerStatus();
            status.CPUIds = GetCPUs().ToList();
            status.MachineName = Environment.MachineName;
            status.CpuUsage = cpuCounter.NextValue();
            status.RamUsage = ramCounter.NextValue();
            status.ProcessId = currentProcess.Id;
            status.OSVersion = Environment.OSVersion;

            return status;
        }
    }
}
