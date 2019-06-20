
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace templated
{
    public class AppPathRequest : IRequest<List<string>>
    {
        public string Path { get; }

        public AppPathRequest(string path)
        {
            Path = path;
        }
    }
    public class TemplateHandler : RequestHandler<AppPathRequest, List<string>>
    {
        protected override List<string> Handle(AppPathRequest request)
        {
            var templatePath = Path.GetDirectoryName(request.Path);
            var templates = Directory.EnumerateDirectories(templatePath)
                    .Select(x => x.Substring(x.LastIndexOf("\\") + 1))
                    .ToList();
            return templates;
        }
    }
}