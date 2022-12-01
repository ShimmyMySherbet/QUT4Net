using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using QUT4Net.Models;

namespace QUT4Net.Components
{
    public class QUTLoginClient
    {

        private RequestFactory RequestFactory { get; }

        public bool LoggedIn { get; private set; } = false;

        public QUTLoginClient(CookieContainer container)
        {
            RequestFactory = new RequestFactory(container);
        }


        /// <summary>
        /// Logs into QUT with the specified username and password.
        /// Resulting auth cookies are stored in <seealso cref="Container"/>
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Login(string username, string password)
        {
            // We need to get the login form URL, this is where we need to send the login credentials to
            var loginEndpoint = await GetOpenIDLoginEndpoint();

            // Send the login form. The resulting response will set our auth cookies in the cookie container
            LoggedIn = await LoginInternal(username, password, loginEndpoint);

            return LoggedIn;
        }

        private async Task<bool> LoginInternal(string username, string password, string endpoint)
        {
            var request = RequestFactory.CreateRequest(endpoint, "POST");
            request.AllowAutoRedirect = true;

            // The url form encoded payload
            var payload = $"username={WebUtility.UrlEncode(username)}&password={WebUtility.UrlEncode(password)}";

            // The payload encoded in UTF-8
            var payloadBuffer = Encoding.UTF8.GetBytes(payload);

            // Mandatory headers for the url encoded form POST
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = payloadBuffer.Length;

            // Open the request stream, and write the contents of our POST data (the login form data)
            using (var network = await request.GetRequestStreamAsync())
            {
                // Write the login form data to the network
                await network.WriteAsync(payloadBuffer, 0, payloadBuffer.Length);

                // Ensure all data is written to the network before we close the stream
                await network.FlushAsync();
            }

            // Read the response for the login request
            // This will set the auth cookies in the cookie container if login was succeeded
            // Here we don't need to read the response payload, only the HTTP header, so we don't call .GetResponseStream()
            using (var response = await request.GetResponseAsync())
            {
                // If the response url (where we were ultimatley redirected) is the login page, the login failed
                if (response.ResponseUri.AbsoluteUri.Contains("login-actions/authenticate"))
                {
                    return false;
                }
            }
            return true;
        }

        private async Task<string> GetOpenIDLoginEndpoint()
        {
            // This url will redirect to the Open ID Login portal
            var request = RequestFactory.CreateRequest("https://qutvirtual4.qut.edu.au/c/portal/login?redirect=%2Fgroup%2Fguest%2Fhome&refererPlid=13&p_l_id=8", "GET");

            // The prev url will redirect multiple times.
            // One of these redirects is when it's creating the OpenID login instance
            // We need to login using the OpenID Instance.
            request.AllowAutoRedirect = true;

            using (var response = await request.GetResponseAsync())
            using (var network = response.GetResponseStream())
            using (var reader = new StreamReader(network))
            {
                // Read the login page HTML
                var html = await reader.ReadToEndAsync();

                var document = new HtmlDocument();
                document.LoadHtml(html);

                // Find the login form element
                var form = document.DocumentNode.SelectSingleNode("//form[@id='loginSuccessful']");

                // Return the action url for the form. This is the endpoint we have to submit our login credentials to
                // This url will be different on every login, per the OpenID login instance
                return form.GetAttributeValue("action", string.Empty);
            }
        }
    }
}