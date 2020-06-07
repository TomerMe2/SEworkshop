using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DAL
{
    class LocalDbContext : AppDbContext
    {
        public LocalDbContext() : base(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\tomer\Source\Repos\SEworkshop3\SEWorkshop\SEWorkshop\DAL\AzamazonLocal.mdf;Integrated Security=True")
        {

        }
    }
}
