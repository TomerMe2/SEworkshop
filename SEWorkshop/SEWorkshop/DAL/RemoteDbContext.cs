using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DAL
{
    class RemoteDbContext : AppDbContext
    {
        private static readonly string server = "db4free.net";
        private static readonly string database = "azamazon";
        private static readonly string uid = "sadnateamuser";
        private static readonly string password = "sadnaTeamPass";
        private static readonly string conString = "SERVER=" + server + ";" + "DATABASE=" +
                    database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

        public RemoteDbContext() : base(conString)
        {

        }
    }
}
