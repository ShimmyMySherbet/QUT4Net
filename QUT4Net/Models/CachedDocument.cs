using System;
using HtmlAgilityPack;

namespace QUT4Net.Models
{
    public struct CachedDocument
    {
        public HtmlDocument Document { get; }

        public DateTime Cached { get; }

        public CachedDocument(HtmlDocument document, DateTime cached)
        {
            Document = document;
            Cached = cached;
        }
    }
}