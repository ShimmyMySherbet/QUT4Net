using System.Net;
using System.Threading.Tasks;
using QUT4Net.Models;

namespace QUT4Net.Components
{
    public class QUTUnitOutlineClient
    {
        private RequestFactory RequestFactory { get; }

        public QUTUnitOutlineClient(CookieContainer container)
        {
            RequestFactory = new RequestFactory(container);
        }

        public async Task<UnitOutline> GetUnitOutlineAsync(string url)
        {
            var document = await RequestFactory.DownloadDocumentAsync(url);
            var node = document.DocumentNode;

            var outline = new UnitOutline();

            outline.UnitCode               = node.QueryText("//tr[./th[contains(text(), 'Unit code')]]/td");
            outline.CreditPoints           = node.QueryInt("//tr[./th[contains(text(), 'Credit points')]]/td");

            outline.Prerequisites          = node.QueryArray("//tr[./th[contains(text(), 'Prerequisite')]]/td", "or");
            outline.Antirequisites         = node.QueryArray("//tr[./th[contains(text(), 'Antirequisite')]]/td");
            outline.Equivalents            = node.QueryArray("//tr[./th[contains(text(), 'Equivalent')]]/td");

            outline.CSPStudentContribution = node.QueryInt("//tr[./th[contains(text(), 'CSP student contribution')]]/td");
            outline.DomesticTuitionUnitFee = node.QueryInt("//tr[./th[contains(text(), 'Domestic tuition unit fee')]]/td");
            outline.InternationalUnitFee   = node.QueryInt("//tr[./th[contains(text(), 'International unit fee')]]/td");

            var coordinator                = node.QueryText("//tr[./th[contains(text(), 'Coordinator')]]/td");

            if (!string.IsNullOrWhiteSpace(coordinator))
            {
                outline.CoordinatorName = coordinator.Split('|')[0].Trim();
                outline.CoordinatorEmail = coordinator.Split('|')[1].Trim();
            }

            return outline;
        }

    }
}