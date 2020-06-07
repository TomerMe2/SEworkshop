using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Tests.TestsDB
{
    class LocalDbContext : AppDbContext
    {
        public LocalDbContext() : base(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Ravid\Desktop\1st Degree\3rd Year\Semester F\Workshop for SE Project\Code\SEworkshop\SEWorkshop\SEWorkshop.UnitTests\TestsDB\AzamazonLocal.mdf;Integrated Security=True")
        {

        }
    }
}
