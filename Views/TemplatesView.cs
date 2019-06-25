using System.IO;
using System.Linq;
using MediatR;
using Terminal.Gui;

namespace templated {
    public class TemplatesView 
    {
        private IMediator mediator;
        public TemplatesView(IMediator mediatr)
        {
            mediator = mediatr;
        }
        
        public void Render(View container, string templatePath)
        {
            var appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            var templates = mediator.Send(new AppPathRequest(appPath)).Result;
            var templatesList = new RadioGroup(new Rect(4, 3, container.Frame.Width, 200), templates.ToArray());
            var onRunTemplate = new Button (3, 10 + templates.Count, "Ok");
            var folderName = new TextField (18, 4 + templates.Count, 40, "");

            onRunTemplate.Clicked = () => {
                var templateFolder = folderName.Text.ToString();
                if (string.IsNullOrEmpty(templateFolder)){
                    MessageBox.ErrorQuery(30, 6, "Error", "Please enter a folder.");
                    return;
                }
                var baseDir = Directory.GetParent(Path.GetDirectoryName(templatePath)).FullName;
                var folderPath = Path.Combine(baseDir, templateFolder);
                var templateSelected = templates.ElementAt(templatesList.Selected);
                if (Directory.Exists(folderPath)) folderPath = UniquePath(folderPath);
                try
                {
                    Directory.CreateDirectory(folderPath);
                    var templateFiles = Directory.EnumerateFiles(Path.Combine(templatePath, templateSelected)); 
                    foreach(var templateFile in templateFiles){
                        var fileName = templateFile.Substring(templateFile.LastIndexOf("\\")+1);
                        File.Copy(templateFile, UniquePath(Path.Combine(folderPath, fileName)));
                    }    
                }
                catch (System.Exception)
                {
                    MessageBox.ErrorQuery(30, 6, "Error", "Could not replicate template.");
                }
            };

            container.Add(
                new Label (3, 2, "Select template: "),
                templatesList,
                new Label (2, 4 + templates.Count(), "Folder: "),
                folderName,
                onRunTemplate,
                new Button (10, 10 + templates.Count(), "Cancel")
            );
        }

        public static string UniquePath(string fullPath){
            int count = 1;

            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while(File.Exists(newFullPath)) 
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }
            return newFullPath;
        }
    }
}