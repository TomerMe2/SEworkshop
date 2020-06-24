using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Text;

namespace SEWorkshop.DAL
{
    class TestDbContext : AppDbContext
    {
        private const string DB_NAME = "AzamazonLocal.mdf";
        private static readonly string dbPath =
            "C:\\Users\\Ravid\\Desktop\\1st Degree\\3rd Year\\Semester F\\Workshop for SE Project\\Code\\SEworkshop\\SEWorkshop\\SEWorkshop\\DAL\\AzamazonLocal.mdf";

        public TestDbContext() : base(string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True", dbPath))
        {

        }
    }
}
