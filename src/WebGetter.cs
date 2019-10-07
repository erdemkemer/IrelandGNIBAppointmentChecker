using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace NotifyIRPAppointment
{
    class WebGetter
    {
        private const string KRegex = @"\<input id\=\""k\"" type\=\""hidden\"" value=\""(.*)\"" \/\>";
        private const string PRegex = @"\<input id\=\""p\"" type\=\""hidden\"" value=\""(.*)\"" \/\>";
        private int Counter = 0; 
        private string K { get; set; }
        private string P { get; set; }

        private async Task GetNewSession()
        {
            var contents = await GetContent("https://burghquayregistrationoffice.inis.gov.ie/Website/AMSREG/AMSRegWeb.nsf/AppSelect?OpenForm",
                "https://burghquayregistrationoffice.inis.gov.ie");
                    
            Match matchK = Regex.Match(contents, KRegex, RegexOptions.IgnoreCase);
            Match matchP = Regex.Match(contents, PRegex, RegexOptions.IgnoreCase);
            
            K = matchK.Groups[1].Value;
            P = matchP.Groups[1].Value;
        }
        
        private async Task<string> GetContent(string url, string referrer)
        {
            var handler = new HttpClientHandler
            {
                UseCookies = false
            };

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), url))
                {
                    request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
                    request.Headers.TryAddWithoutValidation("Accept", "application/json, text/javascript, */*; q=0.01");
                    request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
                    request.Headers.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9,tr;q=0.8");
                    request.Headers.TryAddWithoutValidation("Referer", referrer);
                    request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                    request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "same-origin");
                    request.Headers.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
                    request.Headers.TryAddWithoutValidation("Cookie", "_ga=GA1.3.1992840810.1567979969; _hjid=8cbd6989-f6f4-4868-84b7-a1fbc4db71aa; _gid=GA1.3.1221548346.1570235534; cookieconsent_status=dismiss");

                    var response = await httpClient.SendAsync(request);
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public async Task GetAppointments()
        {
            Counter++;
            
            await GetNewSession();

            var content = await GetContent(
                $"https://burghquayregistrationoffice.inis.gov.ie/Website/AMSREG/AMSRegWeb.nsf/(getAppsNear)?readform&cat=All&sbcat=All&typ=Renewal&k={K}&p={P}&_=1570237940523",
                $"https://burghquayregistrationoffice.inis.gov.ie/Website/AMSREG/AMSRegWeb.nsf/AppSelect?OpenForm");

            Console.WriteLine($"\nExecution: {Counter} {DateTime.Now.ToLongTimeString()} {content}");
            // contents = "{\"slots\":[{\"time\":\"4 November 2019 - 12:00\", \"id\":\"BB770F5CA8763DBB8025848900772FFF\"}]}\n"
            var parsed = JObject.Parse(content);

            if (content == "")
            {
                await GetNewSession();
                return;
            }
            
            var parsedSlots = parsed["slots"];
            if(parsedSlots == null || parsedSlots.Children().First().ToString() == "empty"){
                 return;
            }

            var slots = parsed["slots"].Children().ToList();
            Console.WriteLine($"{slots.Count} slots remaining");
            if (slots.Count > 1 || (slots.Count > 0 && !slots.First().ToString().Contains("BB770F5CA8763DBB8025848900772FFF"))) 
            {
                Process.Start("/Applications/Google Chrome.app/Contents/MacOS/Google Chrome", "https://burghquayregistrationoffice.inis.gov.ie/Website/AMSREG/AMSRegWeb.nsf/AppSelect?OpenForm");
                Console.WriteLine($"Sleeping for 60 seconds");
                Task.Delay(60000).Wait();
            }
                
        }
    }
}