using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Geekors.Infra.WatchDog.Core
{
    public abstract class BaseAgent
    {
        /// <summary>
        /// Agnet 心跳時間，單位毫秒 (預設3秒)
        /// </summary>
        public int CONFIG_HEARTBEAT_PERIOD_MS = 3000;

        /// <summary>
        /// 執行工作的間隔時間 (預設1秒)
        /// </summary>
        public int CONFIG_LOOP_WORKER_PERIOD_MS = 1000;

        /// <summary>
        /// 任務最大數量限制 (預設20個)
        /// </summary>
        public int CONFIG_TASK_MAX_LIMIT = 20;

        /// <summary>
        /// 電腦狀態
        /// </summary>
        private ComputerService computerService;
       
        public BaseAgent()
        {
            computerService = new ComputerService();
        }

        public Task MainTask;
        protected CancellationTokenSource m_cts;
        public Dictionary<int, Task> TaskPool = new Dictionary<int, Task>();

        private void HeartBeat()
        {
            Task HeartBeatTask = Task.Factory.StartNew(() => {
                while (true)
                {
                    ComputerStatus status = null;
                    if (computerService != null)
                        status = computerService.GetStatus();
                    OnHeartBeating(status);
                    Thread.Sleep(CONFIG_HEARTBEAT_PERIOD_MS);
                }
            });
        }

        private object IsTaskPoolCount = new object();
        private bool IsTaskPoolLimitExceed()
        {
            lock (IsTaskPoolCount)
            {
                return TaskPool.Count >= CONFIG_TASK_MAX_LIMIT;
            }
        }

        /// <summary>
        /// 開始執行 Start
        /// </summary>
        public void Start()
        {
            HeartBeat();

            MainTask = Task.Factory.StartNew(() => {
                while (true)
                {
                    if (IsTaskPoolLimitExceed())
                    {
                        Console.WriteLine("執行緒數量已達上限，請稍候...");
                        Thread.Sleep(CONFIG_LOOP_WORKER_PERIOD_MS);
                        continue;
                    }
                    object _data = EventTrigger();
                    if (_data != null)
                    {
                        var SubTask = Task.Factory.StartNew(() => {
                            int _taskId = Task.CurrentId.Value;
                            DoWork(_taskId, _data);
                        });
                        
                        TaskPool.Add(SubTask.Id, SubTask);
                    }
                    Thread.Sleep(CONFIG_LOOP_WORKER_PERIOD_MS);
                }
            });
            try
            {
                MainTask.Wait();
            }
            catch (Exception)
            {
                //handle error 

                //then restart
                Start();
            }
        }

        private void DoWork(int _taskId, object data)
        {
            //Console.WriteLine("開始執行第" + i + "個 sub task.... 10 秒");
            WorkStartingEventArgs s = new WorkStartingEventArgs(_taskId, data);
            OnWorkStarting(this, s);
            WorkFinishedEventArgs e = new WorkFinishedEventArgs(_taskId, data);
            OnWorkFinished(this, e);
            //Console.WriteLine("第" + i + "個 sub task 結束了");
            if (TaskPool != null)
            {
                if (TaskPool.ContainsKey(_taskId))
                    TaskPool.Remove(_taskId);
            }
        }

        /// <summary>
        /// 觸發工作的事件，若有工作要執行，請回傳待執行物件，
        /// 接著會執行 OnWorkStarting
        /// </summary>
        /// <returns></returns>
        public abstract object EventTrigger();

        /// <summary>
        /// 當事件觸發時，需執行的工作
        /// </summary>
        /// <param name="e"></param>
        public abstract void OnWorkStarting(object sender, WorkStartingEventArgs e);

        /// <summary>
        /// 執行工作完成時，所需執行的工作
        /// </summary>
        /// <param name="e"></param>
        public abstract void OnWorkFinished(object sender, WorkFinishedEventArgs e);

        /// <summary>
        /// 每次心跳時，需執行的工作，通常是將 Task 狀態傳回中控台
        /// </summary>
        public abstract void OnHeartBeating(ComputerStatus Status);
    }
}
