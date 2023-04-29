using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using QUT4Net.Models;

namespace QUT4Net.Components
{
    public class QUTStudentProfileClient
    {
        private RequestFactory RequestFactory { get; }

        public QUTStudentProfileClient(CookieContainer container)
        {
            RequestFactory = new RequestFactory(container);
        }

        public async Task<string> GetQUTEmail()
        {
            var document = await RequestFactory.DownloadDocumentAsync("https://qutvirtual4.qut.edu.au/group/student/personal-profile");
            return document.DocumentNode.QueryText("//div[@class='form-group email-form-group row'][./b[contains(text(), 'QUT')]]//span[@class='email-address']/a");
        }

        public async Task<string> GetPersonalEmail()
        {
            var document = await RequestFactory.DownloadDocumentAsync("https://qutvirtual4.qut.edu.au/group/student/personal-profile");
            return document.DocumentNode.QueryText("//div[@class='form-group email-form-group row'][./b[contains(text(), 'Personal')]]//span[@class='email-address']/a");
        }

        public async Task<string> GetPersonalPhoneNumber()
        {
            var document = await RequestFactory.DownloadDocumentAsync("https://qutvirtual4.qut.edu.au/group/student/personal-profile");
            return document.DocumentNode.QueryText("//a[@id='contact-phone-primary']");
        }

        public async Task<string> GetHomeAddress()
        {
            var document = await RequestFactory.DownloadDocumentAsync("https://qutvirtual4.qut.edu.au/group/student/personal-profile");
            var node = document.DocumentNode.SelectSingleNode("//div[./b[contains(text(), 'Home Address')]]/div");
            return node.InnerHtml.Replace("<br>", " ");
        }

        public async Task<string> GetStudentTitle()
        {
            var document = await RequestFactory.DownloadDocumentAsync("https://qutvirtual4.qut.edu.au/group/student/personal-profile");
            var nodes = document.DocumentNode.SelectNodes("//div[@class='profile-header-title']/h2");

            var sb = new StringBuilder();

            foreach (var element in nodes)
            {
                if (!string.IsNullOrEmpty(element.InnerText))
                {
                    sb.Append(element.InnerText + ' ');
                }
            }

            return sb.ToString().Trim();
        }

        public async Task<string> GetStudentType()
        {
            var document = await RequestFactory.DownloadDocumentAsync("https://qutvirtual4.qut.edu.au/group/student/personal-profile");
            return document.DocumentNode.QueryText("//div[@class='profile-header-badge profile-header-from']");
        }

        public async Task<float> GetPrintCredits()
        {
            var document = await RequestFactory.DownloadDocumentAsync("https://qutvirtual4.qut.edu.au/group/student/personal-profile");
            return document.DocumentNode.QueryFloat("//span[@class='print-quota-amount-balance']");
        }

        public async Task<float> GetStudentPrintCredits(string studentID)
        {
            var document = await RequestFactory.DownloadDocumentAsync($"https://qutvirtual4.qut.edu.au/group/student/personal-profile?p_p_id=PrintBalance_WAR_printbalance&p_p_lifecycle=2&p_p_state=normal&p_p_mode=view&p_p_resource_id=qps&p_p_cacheability=cacheLevelPage&username={studentID}&studentSupport=false");
            return document.DocumentNode.QueryFloat("//span[@class='print-quota-amount-balance']", -1);
        }

        public async Task RR()
        {
            try
            {
         var request = RequestFactory.CreateRequest("https://qutvirtual3.qut.edu.au/ords/qv/qv_common_image.display?p_id=11526572&p_role_cd=STU", "GET");
            using (var response = await request.GetResponseAsync())
            using (var network = response.GetResponseStream())
            using (var file = new FileStream("result.png", FileMode.Create, FileAccess.Write))
            {
                await network.CopyToAsync(file);
                await file.FlushAsync();
            }
            }
            catch (System.Exception ex)
            {

                throw;
            }
   
        }
    }
}