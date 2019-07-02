using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Novacode;
using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace templated {

    public enum DataConvention
    {
        Yaml = 1,
        Json = 2
    }

    public class DataTemplateRequest : IRequest<TemplateResponse>
    {
        public string FolderName { get; set; }
        public string SelectedTemplate { get; set; }
        public string TemplatePath { get; set; }
        public string DataConvention { get; set; }
    }

    public class DataTemplate : RequestHandler<DataTemplateRequest, TemplateResponse>
    {
        private string _templatePrefix = "[";
        private string _templateSuffix = "]";
        protected override TemplateResponse Handle(DataTemplateRequest request)
        {
            var templateFolder = request.FolderName;
            if (string.IsNullOrEmpty(templateFolder))
                throw new ArgumentException("Invalid template folder");
            
            var baseDir = Directory.GetParent(Path.GetDirectoryName(request.TemplatePath)).FullName;
            var folderPath = Path.Combine(baseDir, templateFolder);
            var templateSelected = request.SelectedTemplate;

            try
            {
                if (!Directory.Exists(folderPath))
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

            CreateDataFiles(folderPath);

            // data bind to docs in templated folder
            var templatedFiles = Directory.GetFiles(Path.Combine(folderPath, templateSelected), 
                "!(*@(.yaml|.json))", 
                SearchOption.AllDirectories);
            foreach(var templateFile in templatedFiles)
            {
                // get the equivalent data template
                var filename = Path.GetFileNameWithoutExtension(templateFile);
                var dataTemplate = Path.Combine(folderPath, templateSelected, filename, ".yaml");
                var fileExtension = Path.GetExtension(templateFile);
                var mergeExtensions = new List<string>{".doc", ".docx"};
                if (!File.Exists(dataTemplate) || !mergeExtensions.Contains(fileExtension)) continue;

                var input = new StringReader(dataTemplate);
                var deserializer = new DeserializerBuilder().Build();
                var yamlObject = deserializer.Deserialize(input);

                var serializer = new SerializerBuilder()
                    .JsonCompatible()
                    .Build();

                var json = serializer.Serialize(yamlObject);
                dynamic jsonGraph = JsonConvert.DeserializeObject(json);
                var token = JToken.Parse(json);
                var replacePatterns = new Dictionary<string, string>();
                // we are trying to identify dynamically the type of item within the json graph
                foreach (KeyValuePair<string, JToken> node in (JObject)token)
                {
                    switch (node.Value.Type)
                    {
                        case JTokenType.String:
                        case JTokenType.Float:
                        case JTokenType.Integer:
                            replacePatterns.Add($"{_templatePrefix}{node.Key}{_templateSuffix}", node.Value.ToObject<string>());
                            break;
                        case JTokenType.Array:

                            break;
                        case JTokenType.None:
                        case JTokenType.Null:
                        default:
                            break;
                    }
                }

                // data merge template file with data
                try
                {
                    using(DocX document = DocX.Load(templateFile))
                    {
                        foreach (var item in replacePatterns)
                        {
                            document.ReplaceText(item.Key, replacePatterns.GetValueOrDefault(item.Key));
                        }
                    }
                }
                catch (System.Exception)
                {
                    return new TemplateResponse{ Status = "Could not create template."};
                }
                
            }

            return new TemplateResponse{ Status = "Completed." };
        }

        private void CreateDataFiles(string folderPath)
        {
            // create data files if they don't yet exist
            try
            {
                foreach(var templateFile in Directory.EnumerateFiles(folderPath, "*.docx"))
                {
                    var filename = Path.GetFileNameWithoutExtension(templateFile);
                    var yaml = Path.Combine(filename, ".yaml");
                    if (!File.Exists(yaml))
                        File.Create(filename);
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }

    public class RebindRequest : IRequest<TemplateResponse>{
        public string Folder { get; set; }
        public string Template { get; set; }
    }

    public class RebindDataTemplate : RequestHandler<DataTemplateRequest, TemplateResponse>
    {
        protected override TemplateResponse Handle(DataTemplateRequest request)
        {
            // run the template without recreating documents but updated based upon data files
            throw new System.NotImplementedException();
        }
    }
}