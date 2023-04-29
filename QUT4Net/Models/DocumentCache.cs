using System;
using System.Collections.Concurrent;
using HtmlAgilityPack;

namespace QUT4Net.Models
{
    public class DocumentCache
    {
        /// <summary>
        /// The Maximum amount of time a document in the cache is valid
        /// </summary>
        public TimeSpan AgeLimit { get; set; } = TimeSpan.FromMinutes(1);

        private ConcurrentDictionary<string, CachedDocument> m_Cache = new();

        public bool TryGetDocument(string url, out HtmlDocument document)
        {
            if (m_Cache.TryGetValue(url, out var cache))
            {
                if (DateTime.Now.Subtract(cache.Cached) < AgeLimit)
                {
                    document = cache.Document;
                    return true;
                }
                m_Cache.TryRemove(url, out _);
            }
            document = null;
            return false;
        }

        public void CacheDocument(HtmlDocument document, string url)
        {
            m_Cache[url] = new CachedDocument(document, DateTime.Now);
        }
    }
}