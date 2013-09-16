using System;
using System.Data;
using System.Reflection;
using System.Linq;

namespace MonoTest.Infrastructure
{
    public class DatabaseFactory
    {
        private static readonly Assembly SQLiteAssembly;

        private readonly string _dbPath;
		private readonly string _conStr;
		private readonly Type _sqliteConnectionType;

        static DatabaseFactory()
        {
            // Dynamically load SQLite ADO.NET depending on platform
            SQLiteAssembly = Assembly.Load(Environment.OSVersion.Platform == PlatformID.Unix
                ? "Mono.Data.Sqlite"
                : "System.Data.SQLite");
        }

        public DatabaseFactory(SettingsStore settingsStore)
        {
            _dbPath = settingsStore.Load().DatabasePath;

            Type[] exportedTypes = SQLiteAssembly.GetExportedTypes();

			Type sqliteConnectionStringBuilderType = exportedTypes.Single(t => t.Name == "SqliteConnectionStringBuilder");
			_sqliteConnectionType = exportedTypes.Single(t => t.Name == "SqliteConnection");

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