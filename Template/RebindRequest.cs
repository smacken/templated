using MediatR;

namespace templated
{
    public class RebindRequest : IRequest<TemplateResponse>{
        public string FolderName { get; set; }
        public string SelectedTemplate { get; set; }
        public string TemplatePath { get; set; }
        public string DataConvention { get; set; }
    }
}