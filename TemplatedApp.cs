using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace templated
{
    public class TemplatedApp
    {
        public IConfiguration Config { get; set; }
        public ServiceProvider Container { get; set; }
    }
}
