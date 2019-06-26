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
                mediator.Send(new FileTemplateRequest{
                    FolderName = folderName.Text.ToString(),
                    TemplatePath = templatePath,
                    SelectedTemplate = templates.ElementAt(templatesList.Selected)
                });
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
    }
}