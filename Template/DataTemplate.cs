using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Novacode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace templated
{
    public class DataTemplate : RequestHandler<DataTemplateRequest, TemplateResponse>
    {
        private string _templatePrefix = "%";
        private string _templateSuffix = "%";
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

            var templatedFiles = Directory
                .EnumerateFiles(folderPath)
                .Where(file => !file.ToLower().EndsWith(".yaml"))
                .ToList();
            foreach(var templateFile in templatedFiles)
            {
                // get the equivalent data template
                var filename = Path.GetFileNameWithoutExtension(templateFile);
                var dataTemplate = Path.Combine(folderPath, filename + ".yaml");
                var fileExtension = Path.GetExtension(templateFile);
                var mergeExtensions = new List<string>{".doc", ".docx"};
                if (!File.Exists(dataTemplate) || !mergeExtensions.Contains(fileExtension)) continue;

                var replacePatterns = ParseReplacePattern(dataTemplate);

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

        protected Dictionary<string, string> ParseReplacePattern(string dataTemplate){
            var replacePatterns = new Dictionary<string, string>
            {
                {"%Date%" , DateTime.Now.ToShortDateString()},
                {"%date%" , DateTime.Now.ToShortDateString()}
            };
            string readContents;
            using (StreamReader streamReader = new StreamReader(dataTemplate, Encoding.UTF8))
            {
                readContents = streamReader.ReadToEnd();
            }
            var input = new StringReader(readContents);
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(input);
            input = null;
            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(yamlObject);
            //dynamic jsonGraph = JsonConvert.DeserializeObject(json);
            var token = JToken.Parse(json);
            
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
            return replacePatterns;
        }

        private void CreateDataFiles(string folderPath)
        {
            // create data files if they don't yet exist
            foreach(var templateFile in Directory.EnumerateFiles(folderPath, "*.docx"))
            {
                var filename = Path.GetFileNameWithoutExtension(templateFile);
                var yaml = Path.Combine(folderPath, filename + ".yaml");
                if (!File.Exists(yaml))
                    File.Create(yaml);
            }
        }
    }
}