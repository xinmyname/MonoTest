using MonoTest.Infrastructure;
using MonoTest.Models;
using Nancy;
using log4net;

namespace MonoTest.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule(ILog log, ItemStore itemStore)
        {
            Get["/"] = _ =>
            {
                var model = new
                {
                    Items = new[] {new Item("A Test Item", "A dark, frothy item with tangerine overtones.")}
                };

                return View["Index", model];
            };
        }
    }
}