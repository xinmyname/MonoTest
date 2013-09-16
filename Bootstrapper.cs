using System;
using System.Diagnostics;
using MonoTest.Infrastructure;
using MonoTest.Models;
using Nancy.Bootstrappers.Autofac;
using Nancy.Hosting.Self;
using log4net;
using Autofac;

namespace MonoTest
{
	class Bootstrapper : AutofacNancyBootstrapper
	{
	    private readonly ILog _log;
	    private readonly ResourceStore _resStore;
	    private readonly string _hostUrl;
	    private readonly NancyHost _host;

	    public static void Main(string[] args)
		{
		    IContainer container = BuildContainer();
	        var log = container.Resolve<ILog>();

	        try
	        {
                var bootstrapper = container.Resolve<Bootstrapper>();
                bootstrapper.Run();
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
            builder.RegisterAssemblyTypes(typeof(Bootstrapper).Assembly);

            return builder.Build();
        }

	    public Bootstrapper(ILog log, SettingsStore settingsStore, ResourceStore resStore)
	    {
	        _log = log;
	        _resStore = resStore;
	        Settings settings = settingsStore.Load(Init);
	        _hostUrl = settings.HostUrl;

            var hostConfig = new HostConfiguration
            {
                UrlReservations = {CreateAutomatically = true}
            };

	        _host = new NancyHost(this, hostConfig, new Uri(_hostUrl));
	    }

        public Settings Init()
        {
            var settings = new Settings
            {
                HostUrl = "http://localhost:8081/",
                DatabasePath = DocumentPath.For("MonoTest", "s3db")
            };

            _resStore.Deploy("MonoTest.empty.s3db", settings.DatabasePath);

            return settings;
        }

        public void Run()
        {
            _host.Start();

            _log.Info("MonoTest host now listening");
            _log.InfoFormat("Navigating to {0}", _hostUrl);
            _log.Info("");
            _log.Info("Press Enter to stop");

            Process.Start(_hostUrl);
            Console.ReadKey();

            _host.Stop();

            _log.Info("Stopped. Goodbye!");
        }


	}
}
