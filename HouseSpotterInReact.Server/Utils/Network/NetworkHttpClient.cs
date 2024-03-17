using System.Net;

namespace HouseSpotter.Server.Utils
{
    /// <summary>
    /// Represents a network HTTP client.
    /// </summary>
    public class NetworkHttpClient
    {
        /// <summary>
        /// Destroys the network HTTP client.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Destroy()
        {
            if (HtmlClient != null)
            {
                HtmlClient.Dispose();
            }
            HtmlClientInitialized = false;
        }

        /// <summary>
        /// Initializes the network HTTP client.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Initialize()
        {
            // Set up the browser for HtmlClient
            HtmlClientHandler = new HttpClientHandler
            {
                UseProxy = false,
                UseDefaultCredentials = true,
                UseCookies = true
            };

            HtmlClient = new HttpClient(HtmlClientHandler);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            HtmlClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgents.GetRandomUserAgent());
            HtmlClient.DefaultRequestHeaders.Accept.Clear();
            //Client.DefaultRequestHeaders.Add("accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            HtmlClient.DefaultRequestHeaders.Add("upgrade-insecure-requests", "1");
            HtmlClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            HtmlClient.DefaultRequestHeaders.Add("sec-ch-ua-arch", "x86");
            HtmlClient.DefaultRequestHeaders.Add("sec-ch-ua-wow64", "?0");
            HtmlClient.DefaultRequestHeaders.Add("sec-ch-ua-bitness", "112.0.5615.165");
            HtmlClient.DefaultRequestHeaders.Add("sec-ch-ua-full-version", "64");
            HtmlClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "Linux");
            HtmlClient.DefaultRequestHeaders.Add("sec-ch-ua-platform-version", "5.14.0");
            HtmlClient.DefaultRequestHeaders.Add("sec-fetch-site", "none");
            HtmlClient.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
            HtmlClient.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
            HtmlClient.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"112\", \"Google Chrome\";v=\"112\", \"Not:A-Brand\";v=\"99\"");
            //Set up the browser for HtmlClient
            HtmlClientInitialized = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the network HTTP client is initialized.
        /// </summary>
        public bool HtmlClientInitialized = false;

        /// <summary>
        /// Gets or sets the HTTP client handler for the network HTTP client.
        /// </summary>
        public HttpClientHandler? HtmlClientHandler;

        /// <summary>
        /// Gets or sets the HTTP client for the network HTTP client.
        /// </summary>
        public HttpClient? HtmlClient;
    }
}