using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Data;
using Mono.Data.Sqlite;
using log4net;
using Ninject;
using Ninject.Modules;
using Ninject.Activation;

namespace MonoTest
{
	class Bootstrapper
	{
		public static void Main(string[] args)
		{
			var kernel = new StandardKernel(new BootstrapModule());
			var log = kernel.Get<ILog>();

			try
			{
				log.Info("Starting...");

				var resStore = kernel.Get<ResourceStore>();
				var settingsStore = kernel.Get<SettingsStore>();
				Settings settings = settingsStore.Load();

				if (String.IsNullOrEmpty(settings.DatabasePath))
				{
					settings.DatabasePath = DocumentPath.For("MonoTest","s3db");
					resStore.Deploy("MonoTest.empty.s3db", settings.DatabasePath);
					settingsStore.Save(settings);
				}

				var itemStore = kernel.Get<ItemStore>();
				itemStore.Add(new Item("Test Item", "A robust, swarthy test item."));

				log.Info("Done!");
			}
			catch (Exception ex)
			{
				log.Fatal(ex);
			}
		}
	}

	internal class BootstrapModule : NinjectModule
	{
		public override void Load()
		{
			Bind<ILog>().ToMethod(x => LogManager.GetLogger("MonoTest"));
			Bind<DatabaseFactory>().ToSelf();
			Bind<SettingsStore>().ToSelf();
			Bind<ItemStore>().ToSelf();
			Bind<ResourceStore>().ToSelf();
		}
	}



	public class ItemStore
	{
		private readonly DatabaseFactory _dbFactory;

		public ItemStore(DatabaseFactory dbFactory)
		{
			_dbFactory = dbFactory;
		}

		public Item Add(Item item)
		{
			using (IDbConnection con = _dbFactory.OpenDatabase())
			{
				con.Execute("INSERT INTO Item (Name,Description) VALUES (?,?)", new {item.Name, item.Description});
				item.Id = con.ExecuteScalar<long>("SELECT MAX(Id) FROM Item");
			}

			return item;
		}
	}

	public class Item
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public Item(string name, string description)
		{
			Name = name;
			Description = description;
		}
	}

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

	[DataContract]
	public class Settings
	{
		[DataMember]
		public string DatabasePath { get; set; }
	}

	public class SettingsStore
	{
		private readonly string _settingsPath;
		private readonly DataContractJsonSerializer _serializer;

		public SettingsStore()
		{
			_settingsPath = DocumentPath.For("MonoTest", "settings");
			_serializer = new DataContractJsonSerializer(typeof(Settings));
		}

		public Settings Load()
		{
			Settings settings;

			if (File.Exists(_settingsPath))
			{
				using (var stream = new FileStream(_settingsPath, FileMode.Open))
					settings = (Settings)_serializer.ReadObject(stream);
			}
			else
				settings = new Settings();

			return settings;
		}

		public void Save(Settings settings)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath));

			using (var stream = new FileStream(_settingsPath, FileMode.Create)) 
				_serializer.WriteObject(stream, settings);
		}
	}

	public static class DocumentPath
	{
		public static string Get()
		{
			if (Environment.OSVersion.Platform == PlatformID.Unix)
				return String.Format("{0}{1}Documents",
				                     Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				                     Path.DirectorySeparatorChar);

			return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		}

		public static string For(string name, string extension)
		{
			return String.Format("{1}{0}{4}{0}{2}.{3}",
			                      Path.DirectorySeparatorChar,
			                      DocumentPath.Get(),
			                      name,
			                      extension,
			                      typeof(DocumentPath).Assembly.GetName().Name);

		}

		public static string For(string subFolder, string name, string extension)
		{
			return String.Format("{1}{0}{4}{0}{5}{0}{2}.{3}",
			                      Path.DirectorySeparatorChar,
 			                      DocumentPath.Get(),
			                      name,
			                      extension,
			                      typeof(DocumentPath).Assembly.GetName().Name,
			                      subFolder);
		}

		public static string For(string subFolder1, string subFolder2, string name, string extension)
		{
			return String.Format("{1}{0}{4}{0}{5}{0}{6}{0}{2}.{3}",
			                      Path.DirectorySeparatorChar,
 			                      DocumentPath.Get(),
			                      name,
			                      extension,
			                      typeof(DocumentPath).Assembly.GetName().Name,
			                      subFolder1,
			                      subFolder2);
		}
	}
}
