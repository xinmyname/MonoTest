using System;
using MonoTest.Infrastructure;
using MonoTest.Models;
using log4net;
using Ninject;
using Ninject.Modules;

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
}
