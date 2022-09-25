using EmbedIO;
using EmbedIO.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MobiFlight.RestWebSocketApi
{
    internal class RestApiManager
    {
        public static string serial = "RestAPI";
        public List<string> apiInputEndpoints = new List<string>();
        private readonly Dictionary<string, string> outputCache = new Dictionary<string, string>();

        public readonly RestApiServer restApiServer;
        public readonly WebSocketApiServer webSocketApiServer;

        private WebServer restServer;
        private WebServer websocketServer;
        private CancellationTokenSource cancellationToken;
        public event ButtonEventHandler OnButtonPressed;

        private bool isRunning = false;
        

        private readonly MessageProcessor messageProcessor;

        public RestApiManager()
        {
            messageProcessor = new MessageProcessor(outputCache);

            restApiServer = new RestApiServer(messageProcessor);
            webSocketApiServer = new WebSocketApiServer("/", true, messageProcessor);
        }

        public void Start(List<string> apiInputEndpoints)
        {
            if (apiInputEndpoints.Count == 0) { return; }
            if (isRunning) {
                cancellationToken.Cancel();
            }


            RestApiServerSettings settings = RestApiServerSettings.Load();

            restServer = new WebServer(o => o.WithUrlPrefix($"http://{settings.restServerInterface}:{settings.restPort}/").WithMode(HttpListenerMode.EmbedIO))
            .WithWebApi("/", m => m.WithController<RestApiServer>(() =>
            {
                return restApiServer;
            }));

            websocketServer = new WebServer(o => o.WithUrlPrefix($"http://{settings.websocketServerInterface}:{settings.websocketPort}/"))
                .WithModule(webSocketApiServer);


            isRunning = true;
            messageProcessor.TriggerInputEvent += OnButtonPressed;

            cancellationToken = new CancellationTokenSource();
            restServer.Start(cancellationToken.Token);
            websocketServer.Start(cancellationToken.Token);
        }

        public void UpdateRestApiEndpoints(List<string> apiInputEndpoints)
        {
            messageProcessor.InputEndpoints = apiInputEndpoints;
        }

        public void Stop()
        {
            if (cancellationToken != null)
            {
                cancellationToken.Cancel();
            }
        }

        public void PublishOutput(string endpoint, string value)
        {
            if (!outputCache.ContainsKey(endpoint) || value != outputCache[endpoint])
            {
                outputCache[endpoint] = value;
                webSocketApiServer.OutputChanged(endpoint, value);
            }
        }

    }
}
