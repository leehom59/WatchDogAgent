using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geekors.Infra.WatchDog.Core;

using RestSharp;

namespace Geekors.WatchDog.Client
{
    /// <summary>
    /// Watch Dog 代理員
    /// </summary>
    public class WatchDogAgent : Agent
    {
        const string CONFIG_BASEURL_KEY = "WatchDog_BaseUrl";
        string BaseUrl = string.Empty;
        public WatchDogAgent()
        {
            BaseUrl = System.Configuration.ConfigurationManager.AppSettings[CONFIG_BASEURL_KEY].ToString();
        }

        /// <summary>
        /// 覆寫心跳的事件，在這個事件可以處理每一次心跳時，要做的事
        /// </summary>
        /// <param name="Status">電腦狀態</param>
        public override void OnHeartBeating(ComputerStatus Status)
        {
            string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(Status);
            RestClient client = new RestClient(BaseUrl);
            RestRequest req = new RestRequest("wdapi/report", Method.POST);
            req.AddParameter("data", strJson, ParameterType.GetOrPost);
            var result = client.Execute(req);
            Console.WriteLine("report done.");
        }
    }
}
