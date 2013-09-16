using System;
using MonoTest.Infrastructure;
using MonoTest.Models;
using log4net;
using Autofac;

namespace MonoTest
{
	class Bootstrapper
	{
		public static void Main(string[] args)
		{
		    IContainer container = BuildContainer();
            var log = container.Resolve<ILog>();

			try
			{
				log.Info("Starting...");

                var resStore = container.Resolve<ResourceStore>();
                var settingsStore = container.Resolve<SettingsStore>();
				Settings settings = settingsStore.Load();

				if (String.IsNullOrEmpty(settings.DatabasePath))
				{
					settings.DatabasePath = DocumentPath.For("MonoTest","s3db");
					resStore.Deploy("MonoTest.empty.s3db", settings.DatabasePath);
					settingsStore.Save(settings);
				}

                var itemStore = container.Resolve<ItemStore>();
				itemStore.Add(new Item("Test Item", "A robust, swarthy test item."));

				log.Info("Done!");
			}
			catch (Exception ex)
			{
				log.Fatal(ex);
			}
		}

        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(LogManager.GetLogger("MonoTest"));
            builder.RegisterType<DatabaseFactory>();
            builder.RegisterType<SettingsStore>();
            builder.RegisterType<ItemStore>();
            builder.RegisterType<ResourceStore>();

            return builder.Build();
        }
	}
}
