using System;
using System.Data;
using Community.CsharpSqlite.SQLiteClient;

namespace MonoTest.Infrastructure
{
    public class DatabaseFactory
    {
        private readonly string _dbPath;

        public DatabaseFactory(SettingsStore settingsStore)
        {
            _dbPath = settingsStore.Load().DatabasePath;
        }

        public IDbConnection OpenDatabase()
        {
            var conBuilder = new SqliteConnectionStringBuilder 
                {
                    Uri = new Uri(_dbPath).AbsoluteUri
                };

            string conStr = conBuilder.ConnectionString;

            IDbConnection con = new SqliteConnection(conStr);
            con.Open();
            return con;
        }
    }
}