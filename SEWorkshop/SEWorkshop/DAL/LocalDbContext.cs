using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DAL
{
    class LocalDbContext : AppDbContext
    {
        public LocalDbContext() : base(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Ravid\Desktop\1st Degree\3rd Year\Semester F\Workshop for SE Project\Code\SEworkshop\SEWorkshop\SEWorkshop\DAL\AzamazonLocal.mdf;Integrated Security=True")
        {

        }
    }
}
