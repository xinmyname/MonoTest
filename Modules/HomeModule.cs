using MonoTest.Infrastructure;
using MonoTest.Models;
using Nancy;
using log4net;

namespace MonoTest.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule(ILog log)
        {
            Get["/"] = _ =>
            {
                log.Info("Doing something...");
                //itemStore.Add(new Item("Special Item", "A difficult to find item"));
                return Response.AsJson("true");
            };
        }
    }
}