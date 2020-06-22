using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DAL
{
    class RemoteDbContext : AppDbContext
    {
        private static string server = "db4free.net";
        private static string database = "azamazon";
        private static string uid = "sadnateamuser";
        private static string password = "sadnaTeamPass";
        private static string conString = "SERVER=" + server + ";" + "DATABASE=" +
                    database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

        public RemoteDbContext() : base(conString)
        {

        }
    }
}
