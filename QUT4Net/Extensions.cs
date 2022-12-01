using System;
using System.Linq;
using HtmlAgilityPack;

namespace QUT4Net
{
    public static class Extensions
    {
        public static int QueryInt(this HtmlNode document, string xPath, int def = 0)
        {
            var node = document.SelectSingleNode(xPath);

            if (node == null)
            {
                return def;
            }

            var cleaned = node.InnerText.Replace("$", "").Replace(",", "").Trim();

            if (int.TryParse(cleaned, out var value))
            {
                return value;
            }
            return def;
        }

        public static string[] QueryArray(this HtmlNode document, string xPath, string delimiter = ",", bool direct = false)
        {
            var node = document.SelectSingleNode(xPath);

            if (node == null)
            {
                return Array.Empty<string>();
            }

            var innerText = direct ? node.GetDirectInnerText() : node.InnerText;

            var items = innerText.Split(delimiter);

            return items.Select(x => x.Trim()).ToArray();
        }

        public static string QueryText(this HtmlNode document, string xPath, string def = null, bool direct = false)
        {
            var node = document.SelectSingleNode(xPath);

            if (node == null)
            {
                return def;
            }

            var innerText = direct ? node.GetDirectInnerText() : node.InnerText;

            return innerText.Trim();
        }

        public static string QueryAttribute(this HtmlNode document, string xPath, string attribute, string def = null)
        {
            var node = document.SelectSingleNode(xPath);

            if (node == null)
            {
                return def;
            }

            return document.GetAttributeValue(attribute, def);
        }
    }
}