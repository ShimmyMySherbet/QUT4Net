using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;
using QUT4Net.Models;

namespace QUT4Net.Components
{
    public class QUTUnitsClient
    {
        private RequestFactory RequestFactory { get; }

        public QUTUnitsClient(CookieContainer container)
        {
            RequestFactory = new RequestFactory(container);
        }

        public async IAsyncEnumerable<UnitInfo> ReadUnitsAsync(int limit = -1, int skip = 0)
        {
            var index = skip;
            var count = 0;
            while (true)
            {
                var url = GetSearchURL(index);
                var document = await RequestFactory.DownloadDocumentAsync(url);

                var nodes = document.DocumentNode.SelectNodes("//div[@class='class-result-item result-item']");

                if (nodes == null || nodes.Count == 0)
                {
                    break;
                }

                index += nodes.Count;

                foreach (var unit in ExtractUnits(nodes))
                {
                    yield return unit;
                    count++;

                    if (limit != -1 && count >= limit)
                    {
                        yield break;
                    }
                }
            }
        }

        private IEnumerable<UnitInfo> ExtractUnits(HtmlNodeCollection nodes)
        {
            foreach (var node in nodes)
            {
                var unitInfo = new UnitInfo();

                var title = node.QueryText(".//h4[@class='content-title']/a");
                unitInfo.UnitCode = title.Split('-')[0].Trim();
                unitInfo.UnitTitle = title.Split('-')[1].Trim();

                unitInfo.TeachingPeriod = node.QueryText(".//li[@class='unit-details-tp']", direct: true);
                unitInfo.StartDate = node.QueryText(".//li[@class='unit-details-startDate']", direct: true);
                unitInfo.StartDate = node.QueryText(".//li[@class='unit-details-startDate']", direct: true);
                unitInfo.EndDate = node.QueryText(".//li[@class='unit-details-endDate']", direct: true);
                unitInfo.Location = node.QueryText(".//li[@class='unit-details-location']", direct: true);
                unitInfo.AttendanceModes = node.QueryArray(".//li[@class='unit-details-attendance-mode']", direct: true);

                unitInfo.OutlineURL = "https://qutvirtual4.qut.edu.au" + node.QueryAttribute(".//a[contains(text(), 'Unit outline')]", "href");
                yield return unitInfo;
            }
        }

        private string GetSearchURL(int index) =>
            $"https://qutvirtual4.qut.edu.au/group/qut/search?params.query=*" +
            $"&profile=UNIT&params.sortKey=5&params.filterKey=year%3A2022&params.showOldUnits=False&params.showInactivePeople=False&params.start={index}";
    }
}