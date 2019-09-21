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

            var templatesList = new RadioGroup(new Rect(3, 2, container.Frame.Width, 200), templates.ToArray());
            var folderName = new TextField (14, 4 + templates.Count, 40, "");
            var isDataTemplate = new CheckBox(3, 8, "Use data template");
            var onRunTemplate = new Button (3, 10 + templates.Count, "Ok");
            var onRebindTemplate = new Button (21, 10 + templates.Count, "Rebind");
            var progress = new ProgressBar (new Rect (68, 1, 10, 1));
            var console = new Label (3, 11 + templates.Count, "");

            onRunTemplate.Clicked = async () => {
                console.Text = "";
                console.Text = "Creating template...";
                progress.Pulse();
                TemplateResponse response;
                if (isDataTemplate.Checked){
                    response = await mediator.Send(new DataTemplateRequest{
                        FolderName = folderName.Text.ToString(),
                        TemplatePath = templatePath,
                        SelectedTemplate = templates.ElementAt(templatesList.Selected)
                    });
                } else {
                    response = await mediator.Send(new FileTemplateRequest{
                        FolderName = folderName.Text.ToString(),
                        TemplatePath = templatePath,
                        SelectedTemplate = templates.ElementAt(templatesList.Selected)
                    });
                }

                progress.Pulse();
                console.Text = response.Status ?? "";

                response = await mediator.Send(new CombineRequest{
                    FolderName = folderName.Text.ToString(),
                    TemplatePath = templatePath,
                    SelectedTemplate = templates.ElementAt(templatesList.Selected)
                });
                                
                progress.Pulse();
                console.Text = response.Status ?? "";
            };

            onRebindTemplate.Clicked = async () => {
                console.Text = "";
                var folder = folderName.Text.ToString(); 
                if (string.IsNullOrEmpty(folder)){
                    console.Text = "Please select a folder.";
                    return;
                }
                console.Text = "Binding data templates...";
                progress.Pulse();
                try
                {
                    var response = await mediator.Send(new RebindRequest{
                        FolderName = folderName.Text.ToString(),
                        TemplatePath = templatePath,
                        SelectedTemplate = templates.ElementAt(templatesList.Selected)
                    });
                    console.Text = response.Status ?? "";
                }
                catch (System.Exception)
                {
                    console.Text = "Unable to rebind data template.";
                }
                
                progress.Pulse();
            };

            container.Add(
                new Label (3, 1, "Select template: "),
                templatesList,
                new Label (3, 4 + templates.Count(), "Folder: "),
                folderName,
                isDataTemplate,
                onRunTemplate,
                new Button (10, 10 + templates.Count(), "Cancel"),
                onRebindTemplate,
                progress,
                console
            );
        }
    }
}