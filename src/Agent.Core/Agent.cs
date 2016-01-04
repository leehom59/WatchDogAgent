namespace Geekors.Infra.WatchDog.Core
{
    public class Agent : BaseAgent
    {
        public IObjectProvider DataProvider { get; set; }
        public Agent() { }
        public Agent(IObjectProvider _Provider)
        {
            DataProvider = _Provider;
        }

        /// <summary>
        /// 每一次心跳時要做的事情
        /// </summary>
        /// <param name="Status"></param>
        public override void OnHeartBeating(ComputerStatus Status) { }

        /// <summary>
        /// 觸發事件
        /// </summary>
        /// <returns></returns>
        public override object EventTrigger()
        {
            if (DataProvider != null)
                return DataProvider.GetObject();
            else
                return null;
        }
        
        /// <summary>
        /// 當 EventTrigger 回傳物件時，開始處理物件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnWorkStarting(object sender, WorkStartingEventArgs e) { }

        /// <summary>
        /// 當結束處理物件時，要做的工作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnWorkFinished(object sender, WorkFinishedEventArgs e) { }
    }
}
