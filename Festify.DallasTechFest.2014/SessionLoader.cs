using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using UpdateControls.Correspondence.FileStream;
using Festify.Logic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Festify.DallasTechFest._2014
{
    public class SessionLoader
    {
        private readonly Device _device;
        private readonly HttpClient _httpClient;

        public SessionLoader()
        {
            _device = new Device(
                new FileStreamStorageStrategy(
                    Path.Combine(
                        Environment.CurrentDirectory,
                        "Correspondence")));

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://dallastechfest.com/api/", UriKind.Absolute);
        }

        public void Load()
        {
            LoadAsync().Wait();
        }

        private async Task LoadAsync()
        {
            HttpResponseMessage sessionsResponse = await _httpClient.GetAsync("sessions");
            if (!sessionsResponse.IsSuccessStatusCode)
            {
                Output(String.Format("Failed: {0} {1}",
                    sessionsResponse.ReasonPhrase,
                    sessionsResponse.RequestMessage));
            }

            HttpContent content = sessionsResponse.Content;
            var sessionsJson = await content.ReadAsStringAsync();

            var sessions = JsonConvert.DeserializeObject<Session[]>(sessionsJson);

            foreach (var session in sessions)
            {
                Output(session.title);
            }

            Output("Success!");
        }

        private void Output(string line)
        {
            Console.WriteLine(line);
        }
    }
}
