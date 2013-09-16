using System;
using System.Configuration;
using System.Diagnostics;
using MonoTest.Infrastructure;
using MonoTest.Models;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Hosting.Self;
using Nancy.ViewEngines.Razor;
using log4net;
using Autofac;

namespace MonoTest
{
	class Bootstrapper : AutofacNancyBootstrapper
	{
	    private readonly ILog _log;
	    private readonly string _hostUrl;
	    private readonly NancyHost _host;

	    public static void Main(string[] args)
	    {
	        var log = LogManager.GetLogger("MonoTest");

	        try
	        {
	            var bootstrapper = new Bootstrapper(
	                log,
	                ConfigurationManager.AppSettings["HostUrl"]);

                bootstrapper.Run();
	        }
	        catch (Exception ex)
	        {
	            log.Fatal(ex);
	        }
		}

	    public Bootstrapper(ILog log, string hostUrl)
	    {
	        _log = log;
	        _hostUrl = hostUrl;

            var hostConfig = new HostConfiguration
            {
                UrlReservations = {CreateAutomatically = true}
            };

	        _host = new NancyHost(this, hostConfig, new Uri(_hostUrl));
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

	    protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
	    {
	        base.ConfigureApplicationContainer(existingContainer);

	        var builder = new ContainerBuilder();

	        builder.RegisterInstance(_log).As<ILog>();
	        builder.RegisterAssemblyTypes(GetType().Assembly);

	        builder.Update(existingContainer.ComponentRegistry);
	    }

	    protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
	    {
	        base.ApplicationStartup(container, pipelines);

	        var settingsStore = container.Resolve<SettingsStore>();
	        var resStore = container.Resolve<ResourceStore>();

	        if (settingsStore.Load() == null)
	        {
                var settings = new Settings
                {
                    DatabasePath = DocumentPath.For("MonoTest", "s3db")
                };

                resStore.Deploy("MonoTest.empty.s3db", settings.DatabasePath);

	            settingsStore.Save(settings);
	        }
	    }
	}
}
