using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DAL
{
    public class DatabaseProxy
    {
        private static readonly object padlock = new object();
        private static AppDbContext? instance = null;

        public static AppDbContext Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new LocalDbContext();
                            /*try
                            {
                                instance = new RemoteDbContext();
                            }
                            catch
                            {
                                instance = new LocalDbContext();
                            }*/
                        }
                    }
                }
                return instance;
            }
        }

        public static void MoveToTestDb()
        {
            if (!(instance is TestDbContext))
            {
                instance = new TestDbContext();
            }
        }

        //Clear all the data that the tables contains. Doesn't delete the tables themselves
        public static void ClearDB()
        {
            //Clear the DB only if it's a test context
            if(Instance is TestDbContext)
            {
                var tableNames = Instance.Database.SqlQuery<string>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME NOT LIKE '%Migration%'").ToList();
                foreach (var tableName in tableNames)
                {
                    Instance.Database.ExecuteSqlCommand(string.Format("DELETE FROM {0}", tableName));
                }
                Instance.SaveChanges();
            }
        }
    }
}
