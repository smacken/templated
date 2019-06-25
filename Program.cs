using System;
using System.Reflection;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using System.IO;
using System.Linq;
using Terminal.Gui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MediatR;
using MediatR.Pipeline;

namespace templated
{
    public class TemplatedApp
    {
        public IConfiguration Config { get; set; }
        public ServiceProvider Container { get; set; }
    }

    class Program
    {
        public static TemplatedApp Bootstrap()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging();
            ConfigureServices(serviceProvider);
            var provider = serviceProvider.BuildServiceProvider();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json");
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("config.json", true, true).Build();
            return new TemplatedApp {
                Container = provider,
                Config = config
            };
        }

        static void Main(string[] args)
        {
            var app = Bootstrap();
            var mediator = app.Container.GetRequiredService<IMediator>();

            Application.Init();
            var top = Application.Top;
            var tframe = top.Frame;
            var win = new Window (new Rect (0, 1, top.Frame.Width, top.Frame.Height-1), "Templates");
            var menu = new MenuBar (new MenuBarItem [] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_Open", "", null),
                    new MenuItem ("_Close", "", () => Close ()),
                    new MenuItem ("_Quit", "", () => { 
                        if (!Quit ()) return;
                        top.Running = false; 
                        Application.RequestStop (); 
                    })
                }),
                new MenuBarItem ("_Edit", new MenuItem [] {
                    new MenuItem ("_Copy", "", null),
                    new MenuItem ("C_ut", "", null),
                    new MenuItem ("_Paste", "", null)
                })
            });
            
            var appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var templatePath = args.Count() == 0 || string.IsNullOrEmpty(args[0]) 
                ? Path.GetDirectoryName(appPath) 
                : args[0];
                        
            var templatesView = new TemplatesView(mediator);
            templatesView.Render(win, templatePath);

            top.Add(win, menu);
            Application.Run();
        }

        static bool Quit ()
        {
            var n = MessageBox.Query (50, 7, "Quit Demo", "Are you sure you want to quit this demo?", "Yes", "No");
            return n == 0;
        }

        static void Close ()
        {
            MessageBox.ErrorQuery (50, 5, "Error", "There is nothing to close", "Ok");
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole());              
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddScoped<IMediator, Mediator>();
            services.AddScoped<ServiceFactory>(p => p.GetService);
            
            //Pipeline
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
        }
    }
}
