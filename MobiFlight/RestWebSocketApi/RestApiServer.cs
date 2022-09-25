using EmbedIO.Routing;
using EmbedIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Swan;
using MobiFlight.RestWebSocketApi.Messages;

namespace MobiFlight.RestWebSocketApi
{
    internal class RestApiServer: EmbedIO.WebApi.WebApiController
    {
        private readonly MessageProcessor messageProcessor;

        public RestApiServer(MessageProcessor aMessageProcessor)
        {
            messageProcessor = aMessageProcessor;
        }


        [Route(HttpVerbs.Post, "/")]
        public async Task<Task> generalEndpoint()
        {
            string data = await HttpContext.GetRequestBodyAsStringAsync();
            return jsonResp(messageProcessor.ProcessJsonMessage(data));
        }

        [Route(HttpVerbs.Post, "/input/{name}")]
        public async Task<Task> inputEndpoint(string name)
        {
            string data = await HttpContext.GetRequestBodyAsStringAsync();
            return jsonResp(messageProcessor.ProcessMessage(new TriggerInputMessage(name, data)));
        }

        [Route(HttpVerbs.Get, "/output/{name}")]
        public Task outputEndpoint(string name)
        {
            return jsonResp(messageProcessor.ProcessMessage(new GetOutputMessage(name)));
        }

        [Route(HttpVerbs.Get, "/output/")]
        public Task getAllOutputs(string name)
        {
            return jsonResp(messageProcessor.ProcessMessage(new GetAllOutputsMessage()));
        }

        private Task jsonResp(string name)
        {
            return HttpContext.SendStringAsync(name, "application/json", Encoding.UTF8);
        }
    }
}
