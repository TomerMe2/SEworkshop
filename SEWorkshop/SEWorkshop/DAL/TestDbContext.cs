using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace SEWorkshop.DAL
{
    class TestDbContext : AppDbContext
    {
        public TestDbContext() : base(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Ravid\Desktop\1st Degree\3rd Year\Semester F\Workshop for SE Project\Code\SEworkshop\SEWorkshop\SEWorkshop\DAL\AzamazonLocal.mdf;Integrated Security=True")
        {

        }
    }
}
