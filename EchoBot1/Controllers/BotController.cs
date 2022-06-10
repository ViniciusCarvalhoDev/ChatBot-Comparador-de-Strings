// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.15.2

using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EchoBot1.Controllers
{
    // This ASP Controller is created to handle a request. Dependency Injection will provide the Adapter and IBot
    // implementation at runtime. Multiple different IBot implementations running at different endpoints can be
    // achieved by specifying a more specific type for the bot constructor argument.
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IBot _bot;

        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            _adapter = adapter;
            _bot = bot;
        }

        [HttpPost]
        [HttpGet]
        public async Task PostAsync()
        {
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //Response.ContentType = "application/json";

            string bot = Newtonsoft.Json.JsonConvert.SerializeObject(Response.Body.ToString());


            byte[] bytes = Encoding.ASCII.GetBytes(bot);

            await Response.BodyWriter.WriteAsync(bytes);
            await _adapter.ProcessAsync(Request, Response, _bot);
        }

        private async Task<string> GetSearchAsync(string userQuery)
        {
            string uri = "https://google-search3.p.rapidapi.com/api/v1/search/q=" + userQuery + "&num=10";
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri),
                Headers =
                {
                    { "x-rapidapi-key", "<Your Rapid API Key>" },
                    { "x-rapidapi-host", "google-search3.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return body;
            }
        }
    }
}
