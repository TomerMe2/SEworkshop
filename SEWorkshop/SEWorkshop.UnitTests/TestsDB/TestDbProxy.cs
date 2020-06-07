using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Tests.TestsDB
{
    public class TestDbProxy
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
                        }
                    }
                }
                return instance;
            }
        }
    }
}
