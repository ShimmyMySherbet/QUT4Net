using System.Net;
using System.Threading.Tasks;
using QUT4Net.Components;
using QUT4Net.Models;

namespace QUT4Net
{
    public class HiQClient
    {
        public CookieContainer Container { get; } = new CookieContainer();

        public bool LoggedIn => Login.LoggedIn;
        public QUTUnitsClient Units { get; }
        public QUTUnitOutlineClient UnitOutlines { get; }
        public QUTCollectionClient Collection { get; }
        public QUTLoginClient Login { get; }
        public QUTStudyClient Study { get; }
        public QUTStudentProfileClient Profile { get; }

        public HiQClient()
        {
            Login = new QUTLoginClient(Container);
            Profile = new QUTStudentProfileClient(Container);
            Units = new QUTUnitsClient(Container);
            Study = new QUTStudyClient(Container);
            UnitOutlines = new QUTUnitOutlineClient(Container);
            Collection = new QUTCollectionClient(this);
        }

        public async Task<bool> LoginAsync(string username, string password) =>
            await Login.Login(username, password);



    }
}