using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using HtmlAgilityPack;
using QUT4Net.Models;

namespace QUT4Net.Parallel
{
    /// <summary>
    /// Allows multiple GET requests to the same resource to share the same request and document
    /// </summary>
    public class RequestManager
    {
        private ConcurrentDictionary<string, TaskCompletionSource<HtmlDocument>> m_Requests { get; } = new();

        /// <summary>
        /// Sends a GET request to the document using the specified provider.
        /// If a GET request is already active for the same resource, it will wait for the existing request instead.
        /// </summary>
        /// <param name="url">The URL to teh requested resource</param>
        /// <param name="provider">The provider to run new GET requests through</param>
        /// <returns>The requested resource</returns>
        public async Task<HtmlDocument> QueryDocumentAsync(string url, DocumentProvider provider)
        {
            if (m_Requests.TryGetValue(url, out var currentTask))
            {
                return await currentTask.Task;
            }

            var newTask = new TaskCompletionSource<HtmlDocument>();
            m_Requests[url] = newTask;

            try
            {
                var result = await provider(url);
                m_Requests.TryRemove(url, out _);

                newTask.SetResult(result);

                return result;
            }
            catch (Exception ex)
            {
                newTask.SetException(ex);
                throw;
            }
        }
    }
}