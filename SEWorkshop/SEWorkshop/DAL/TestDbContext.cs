using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DAL
{
    class TestDbContext : AppDbContext
    {
        public TestDbContext() : base(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\tomer\Source\Repos\SEworkshop3\SEWorkshop\SEWorkshop\DAL\AzamazonTest.mdf;Integrated Security=True")
        {

        }
    }
}
