using System;

namespace Geekors.Infra.WatchDog.Core
{
    public class WorkStartingEventArgs : EventArgs
    {
        private int taskId;
        public int TaskId
        {
            get
            {
                return taskId;
            }
        }


        private object _dataContext;
        public object DataContext
        {
            get
            {
                return _dataContext;
            }

        }

        public WorkStartingEventArgs(int _taskId, object DataContext)
        {
            taskId = _taskId;
            _dataContext = DataContext;
        }

    }
}
