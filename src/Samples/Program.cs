namespace Geekors.WatchDog.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            WatchDogAgent watchDog = new WatchDogAgent();
            //watchDog.CONFIG_HEARTBEAT_PERIOD_MS = 10000;
            watchDog.Start();
        }
    }
}
