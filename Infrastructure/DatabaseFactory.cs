using System;
using System.Data;
//using Community.CsharpSqlite.SQLiteClient;
using System.Reflection;
using System.Linq;

namespace MonoTest.Infrastructure
{
    public class DatabaseFactory
    {
        private readonly string _dbPath;
		private readonly Assembly _sqliteAssembly;
		private readonly string _conStr;
		private readonly Type _sqliteConnectionType;

        public DatabaseFactory(SettingsStore settingsStore)
        {
            _dbPath = settingsStore.Load().DatabasePath;

			if (Environment.OSVersion.Platform == PlatformID.Unix)
				_sqliteAssembly = Assembly.Load("Mono.Data.Sqlite");
			else
				_sqliteAssembly = Assembly.Load("Community.CsharpSqlite.SQLiteClient");

			Type[] exportedTypes = _sqliteAssembly.GetExportedTypes();

			Type sqliteConnectionStringBuilderType = exportedTypes.Where(t => t.Name == "SqliteConnectionStringBuilder").Single();
			_sqliteConnectionType = exportedTypes.Where(t => t.Name == "SqliteConnection").Single();

			object conBuilder = Activator.CreateInstance(sqliteConnectionStringBuilderType);
			sqliteConnectionStringBuilderType.GetProperty("Uri").SetValue(conBuilder, new Uri(_dbPath).AbsoluteUri, null);
			_conStr = (string)sqliteConnectionStringBuilderType.GetProperty("ConnectionString").GetValue(conBuilder, null);
		}

        public IDbConnection OpenDatabase()
        {
			var con = (IDbConnection)Activator.CreateInstance(_sqliteConnectionType);
			con.ConnectionString = _conStr;
			con.Open();

			return con;
        }
    }
}