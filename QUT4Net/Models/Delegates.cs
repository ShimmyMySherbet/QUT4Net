using System.Threading.Tasks;
using HtmlAgilityPack;

namespace QUT4Net.Models
{
    public delegate Task<HtmlDocument> DocumentProvider(string url);
}