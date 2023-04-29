using System.IO;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using QUT4Net.Parallel;

namespace QUT4Net.Models
{
    public class RequestFactory
    {
        public CookieContainer Container { get; }

        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:102.0) Gecko/20100101 Firefox/102.0";

        public DocumentCache Cache { get; } = new DocumentCache();

        public RequestManager Manager { get; } = new RequestManager();

        public RequestFactory(CookieContainer container)
        {
            Container = container;
        }

        /// <summary>
        /// Creates a new request to the provided URL, with authentication cookies and headers to imitate the target browser.
        /// </summary>
        /// <param name="url">The URL to the requested resource</param>
        /// <param name="method">The HTTP method for the request</param>
        /// <returns>The HTTP Web Request to send to the server</returns>
        public HttpWebRequest CreateRequest(string url, string method)
        {
            var request = WebRequest.CreateHttp(url);
            request.Method = method;

            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/jxl,image/webp,*/*;q=0.8";
            request.Headers["Accept-Language"] = "en-US,en;q=0.5";
            request.KeepAlive = true;
            request.Headers["DNT"] = "1";
            request.Headers["Pragma"] = "no-cache";
            request.Headers["Sec-Fetch-Dest"] = "document";
            request.Headers["Sec-Fetch-Mode"] = "navigate";
            request.Headers["Sec-Fetch-User"] = "?1";
            request.Headers["Sec-GPC"] = "?1";
            request.Headers["Upgrade-Insecure-Requests"] = "1";
            request.UserAgent = UserAgent;

            request.CookieContainer = Container;

            return request;
        }

        /// <summary>
        /// Downloads the specified HTML document. If the document is present in the cache and is valid for this request, it will be returned instead.
        /// </summary>
        /// <param name="url">The URL to the requested resource</param>
        /// <param name="skipCache">When true, the cache will not be used</param>
        public async Task<HtmlDocument> DownloadDocumentAsync(string url, bool skipCache = false)
        {
            if (!skipCache)
            {
                if (Cache.TryGetDocument(url, out var document))
                {
                    return document;
                }
            }

            var result = await Manager.QueryDocumentAsync(url, DownloadDocumentInternal);

            Cache.CacheDocument(result, url);

            return result;
        }

        protected async Task<HtmlDocument> DownloadDocumentInternal(string url)
        {
            var request = CreateRequest(url, "GET");

            using (var response = await request.GetResponseAsync())
            using (var network = response.GetResponseStream())
            using (var reader = new StreamReader(network))
            {
                var html = await reader.ReadToEndAsync();

                var document = new HtmlDocument();
                document.LoadHtml(html);

                return document;
            }
        }
    }
}