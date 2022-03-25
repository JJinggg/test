using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Data;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebformatInjection;
using WebformatInjection.WebNeedDefualtHeader;

namespace tddoffice
{
    class Program
    {
        static void Main(string[] args)
        {


            Program prog = new Program();

            Task.Run(() => prog.pararuns());

            for (int i = 0; i < 5000; i++)
            {
                System.Diagnostics.Debug.WriteLine($"nonasync +{i}");
            }

            
            Console.ReadKey();
        }
 
       


        class Json
        {
            [JsonProperty("id")]
            public string ID { get; set; }
            [JsonProperty("pwd")]
            public string PWD { get; set; }
        }
        class serverResult
        {
            [JsonProperty("result")]
            public bool result { get; set; }
        }

        public void pararuns()
        {
            Parallel.For(0, 5000, i =>
                  {

                      System.Diagnostics.Debug.WriteLine($"para +{i}");

                  });
        }


        public async void asyncRuns()
        {
            for (int i = 0; i < 5000; i++)
            {
                System.Diagnostics.Debug.WriteLine($"async +{i}");
            }
        }


        public async void run()
        {
            string url = "http://192.168.0.5:8080/user";
            string retext = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            using (HttpWebResponse rep = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode code = rep.StatusCode;
                if (code == HttpStatusCode.OK)
                {
                    Stream res = rep.GetResponseStream();
                    using (StreamReader sr = new StreamReader(res))
                    {
                        retext = sr.ReadToEnd();


                       // Console.WriteLine(retext);
                    }
                }
            }


            //JArray ja = JArray.Parse(retext);
            //Console.WriteLine(ja[0]);


            bool loginResult = await vaildLogin();

            if (loginResult)
            {
                Console.WriteLine("로그인성공");
            }
            else
            {
                Console.WriteLine("로그인실패");
            }

        }

        public async Task<bool> vaildLogin()
        {
            HttpWebRequest request = null;
            HttpWebResponse response= null;

            JObject job = new JObject();
            job.Add("id", "jjykim");
            job.Add("pwd", "jjykim1231");

            using (Webformat webformat = new Webformat("http://192.168.0.5:8080/login"))
            {
                request = webformat.CreateNewFormat(CRUD.Post, Accept.OnlyJson, ContentType.Json, Certificate.True);
                response = await webformat.GetResponseAsync(request, job.ToString());
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string returnJsonString = HttpWebIO.ReturnStrHtml(response.GetResponseStream(), Encoding.UTF8);
                Console.WriteLine(returnJsonString);
                return JsonConvert.DeserializeObject<serverResult>(returnJsonString).result;
            }

            return false;

        }

    }
}
