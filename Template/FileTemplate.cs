using System;
using System.IO;
using MediatR;

namespace templated {

    public class FileTemplateRequest : IRequest<TemplateResponse> {
        public string FolderName { get; set; }
        public string TemplatePath { get; set; }
        public string SelectedTemplate { get; set; }
    }

    public class FileTemplate : RequestHandler<FileTemplateRequest, TemplateResponse>
    {
        protected override TemplateResponse Handle(FileTemplateRequest request)
        {
            var templateFolder = request.FolderName;
            if (string.IsNullOrEmpty(templateFolder))
                throw new ArgumentException("Invalid template folder");
            
            var baseDir = Directory.GetParent(Path.GetDirectoryName(request.TemplatePath)).FullName;
            var folderPath = Path.Combine(baseDir, templateFolder);
            var templateSelected = request.SelectedTemplate;
            if (Directory.Exists(folderPath)) 
                folderPath = FileUtils.UniquePath(folderPath);
            try
            {
                Directory.CreateDirectory(folderPath);
                var templateFiles = Directory.EnumerateFiles(Path.Combine(request.TemplatePath, templateSelected)); 
                foreach(var templateFile in templateFiles){
                    var fileName = templateFile.Substring(templateFile.LastIndexOf("\\")+1);
                    File.Copy(templateFile, FileUtils.UniquePath(Path.Combine(folderPath, fileName)));
                }    
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return new TemplateResponse();
        }
    }
}