using Nancy;

namespace MonoTest.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ =>
            {
                return Response.AsJson("true");
            };
        }
    }
}