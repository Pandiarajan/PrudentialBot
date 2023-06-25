using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Net.Mail;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace WindowsService
{
    [RunInstaller(true)]
    public partial class Service1 : ServiceBase
    {
        public Thread worker = null;
        public System.Timers.Timer timer = new System.Timers.Timer(60000);
        SmtpClient smtpClient;
        public Service1()
        {
            InitializeComponent();
        }
        private static readonly HttpClient client = new HttpClient();

        protected override void OnStart(string[] args)
        {
            try
            {
                smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("pandiarajan.it@gmail.com", "PasswordRemoved"),
                    EnableSsl = true,
                };
                timer.Elapsed += Timer_Elapsed;
                timer.AutoReset = true;
                timer.Enabled = true;
                timer.Start();                
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Working();
        }

        public async void Working()
        {
            string path = "C:\\Log.txt";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                try
                {
                    #region API Key
                    client.DefaultRequestHeaders.Add("api-key", "e10..");
                    #endregion API Key
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                    //while (true)
                    {
                        try
                        {
                            writer.WriteLine(string.Format("PrudentialBot Called on " + DateTime.Now.ToString("dd /MM/yyyy hh:mm:ss tt")));
                            string news = new WebClient().DownloadString("http://localhost:59028/api/NewsAgencies/GetNewAgenciesData");
                            string portfolioData = new WebClient().DownloadString("http://localhost:59028/api/NewsAgencies/GetPortfolioData");
                            JObject dataset = GetJsonData(news, portfolioData);
                            var jsonObject = JsonConvert.SerializeObject(dataset);
                            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
                            var response = await client.PostAsync("https://openai01devtest.openai.azure.com/openai/deployments/chatPlaygroundDeployment/chat/completions?api-version=2023-05-15", content );
                            var responseStringAI = await response.Content.ReadAsStringAsync();
                            var jsonResult = JsonConvert.DeserializeObject<JObject>(responseStringAI);
                            if (jsonResult["choices"][0]["finish_reason"].ToString() == "stop")
                            {
                                writer.WriteLine("Result: " + jsonResult["choices"][0]["message"]["content"].ToString()
                                    + Environment.NewLine +
                                    "News: "+ JsonConvert.DeserializeObject<JArray>(news)[0]["content"]
                                    + Environment.NewLine);
                            }
                            else
                            {
                                writer.WriteLine("Result: " + jsonResult["choices"][0].ToString());
                            }
                            //smtpClient.Send("pandiarajan.it@gmail.com", "pandiarajan.it@gmail.com", "Stock Updates Alert" + DateTime.Now.ToLongDateString(), responseStringAI);                        
                            writer.Close();
                            await Task.Delay(60000); //Every minute instead of actual requirement of every hour.
                        }
                        catch (Exception ex)
                        {
                            writer.WriteLine(ex.ToString());
                            writer.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    writer.WriteLine(ex.ToString());
                    writer.Close();
                }
            }
        }

        private static JObject GetJsonData(string news, string portfolioData)
        {
            var dataset = new JObject();
            JArray jArray = new JArray();
            JObject system = new JObject();
            system.Add("role", "system");
            system.Add("content", "I'm stock market risk mitigator. I averse risk for the investment and I'm financial advisor. I give sharp answer when to reduce your stocks by reading the news and your portfolio in less than 100 words. I will include the stock details such as NumberOfShares, InvestedAmount and Symbol.");
            jArray.Add(system);

            JObject user = new JObject();
            user.Add("role", "user");
            user.Add("content", "Here's my investments:\r\n" +
                portfolioData
                + " Here's the news: \r\n" +
                news);
            jArray.Add(user);

            dataset.Add("messages", jArray);
            return dataset;
        }

        public void onDebug()
        {
            OnStart(null);
        }
        protected override void OnStop()
        {
            try
            {
                if ((worker != null) & worker.IsAlive)
                {
                    worker.Abort();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        
    }
}
