using System.Net;
using System.Threading.Tasks;
using QUT4Net.Models;

namespace QUT4Net.Components
{
    public class QUTStudyClient
    {
        private RequestFactory RequestFactory { get; }

        public QUTStudyClient(CookieContainer container)
        {
            RequestFactory = new RequestFactory(container);
        }

        public async Task<float> GetCurrentGPA()
        {
            var document = await RequestFactory.DownloadDocumentAsync("https://qutvirtual4.qut.edu.au/group/student/study");

            return document.DocumentNode.QueryFloat("//span[@class='gpa-result-mark']");
        }



    }
}