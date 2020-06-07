using System;
using System.Collections.Generic;
using System.Text;

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
                            try
                            {
                                instance = new RemoteDbContext();
                            }
                            catch
                            {
                                instance = new LocalDbContext();
                            }
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
            //clean the db
            instance.Database.ExecuteSqlCommand(@"DECLARE @Sql NVARCHAR(500) DECLARE @Cursor CURSOR
                SET @Cursor = CURSOR FAST_FORWARD FOR
                SELECT DISTINCT sql = 'ALTER TABLE [' + tc2.TABLE_SCHEMA + '].[' +  tc2.TABLE_NAME + '] DROP [' + rc1.CONSTRAINT_NAME + '];'
                FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc1
                LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc2 ON tc2.CONSTRAINT_NAME =rc1.CONSTRAINT_NAME

                OPEN @Cursor FETCH NEXT FROM @Cursor INTO @Sql

                WHILE (@@FETCH_STATUS = 0)
                BEGIN
                Exec sp_executesql @Sql
                FETCH NEXT FROM @Cursor INTO @Sql
                END

                CLOSE @Cursor DEALLOCATE @Cursor
                

                EXEC sp_MSforeachtable 'DROP TABLE ?'
                ");
        }
    }
}
