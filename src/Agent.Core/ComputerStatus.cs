using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geekors.Infra.WatchDog.Core
{
    public class ComputerStatus
    {
        /// <summary>
        /// CPU 的編號 (多顆)
        /// </summary>
        public List<string> CPUIds { get; set; }
        /// <summary>
        /// Ram 使用狀況
        /// </summary>
        public float RamUsage { get; set; }


        public float CpuUsage { get; set; }


        public string MachineName { get; set; }


        public int ProcessId { get; set; }

        public OperatingSystem OSVersion { get; set; }

    }
}
