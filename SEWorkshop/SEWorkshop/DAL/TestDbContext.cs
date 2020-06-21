using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Text;

namespace SEWorkshop.DAL
{
    class TestDbContext : AppDbContext
    {
        private const string DB_NAME = "AzamazonTest.mdf";
        private static readonly string dbPath = Path.GetFullPath(DB_NAME);

        public TestDbContext() : base(string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True", dbPath))
        {

        }
    }
}
