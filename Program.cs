﻿using System;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using System.IO;
using System.Linq;
using Terminal.Gui;

namespace templated
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.Init();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json");
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("config.json", true, true).Build();
            var top = Application.Top;
            var menu = new MenuBar (new MenuBarItem [] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_Quit", "", () => { 
                        Application.RequestStop (); 
                    })
                }),
            });
            top.Add(menu);
            var win = new Window (new Rect (0, 1, top.Frame.Width, top.Frame.Height-1), "Templates");
            top.Add (win);

            var appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var templatePath = args.Count() == 0 || string.IsNullOrEmpty(args[0]) 
                ? Path.GetDirectoryName(appPath) 
                : args[0];
            var templates = Directory.EnumerateDirectories(templatePath)
                .Select(x => x.Substring(x.LastIndexOf("\\") + 1));
            var templatesList = new RadioGroup(new Rect(4, 3, top.Frame.Width, 200), templates.ToArray());
            var onRunTemplate = new Button (3, 14 + templates.Count(), "Ok");
            var folderName = new TextField (18, 4 + templates.Count(), 40, "");

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
            win.Add(
                new Label (3, 2, "Select template: "),
                templatesList,
                new Label (2, 4 + templates.Count(), "Folder: "),
                folderName,
                onRunTemplate,
                new Button (10, 14 + templates.Count(), "Cancel")
            );

            Application.Run();
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
