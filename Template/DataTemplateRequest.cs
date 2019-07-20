using MediatR;

namespace templated
{
    public class DataTemplateRequest : IRequest<TemplateResponse>
    {
        public string FolderName { get; set; }
        public string SelectedTemplate { get; set; }
        public string TemplatePath { get; set; }
        public string DataConvention { get; set; }
    }
}