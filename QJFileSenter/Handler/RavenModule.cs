using Nancy;

namespace QJ_FileCenter
{
    public abstract class RavenModule : NancyModule
    {
        protected RavenModule()
            : this("")
        {

        }

        protected RavenModule(string path)
            : base(path)
        {
            Before += (ctx) =>
            {
                return null;
            };
        }
    }
}
