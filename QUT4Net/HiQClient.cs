using System.Net;
using System.Threading.Tasks;
using QUT4Net.Components;
using QUT4Net.Models;

namespace QUT4Net
{
    public class HiQClient
    {
        private CookieContainer Container { get; } = new CookieContainer();

        public bool LoggedIn => Login.LoggedIn;
        public QUTUnitsClient Units { get; }
        public QUTUnitOutlineClient UnitOutlines { get; }
        public QUTCollectionClient Collection { get; }
        public QUTLoginClient Login { get; }

        public HiQClient()
        {
            Login = new QUTLoginClient(Container);
            Units = new QUTUnitsClient(Container);
            UnitOutlines = new QUTUnitOutlineClient(Container);
            Collection = new QUTCollectionClient(this);
        }

        public async Task<bool> LoginAsync(string username, string password) =>
            await Login.Login(username, password);
    }
}