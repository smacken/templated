using MediatR;

namespace templated
{
    public class FileTemplateRequest : IRequest<TemplateResponse> {
        public string FolderName { get; set; }
        public string TemplatePath { get; set; }
        public string SelectedTemplate { get; set; }
    }
}