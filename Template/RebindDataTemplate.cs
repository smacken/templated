using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Novacode;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace templated
{
    public class RebindDataTemplate : RequestHandler<RebindRequest, TemplateResponse>
    {
        private string _templatePrefix = "%";
        private string _templateSuffix = "%";
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

        protected override TemplateResponse Handle(RebindRequest request)
        {
            // run the template without recreating documents but updated based upon data files
            var templateFolder = request.FolderName;
            if (string.IsNullOrEmpty(templateFolder))
                throw new ArgumentException("Invalid template folder");
            
            var baseDir = Directory.GetParent(Path.GetDirectoryName(request.TemplatePath)).FullName;
            var folderPath = Path.Combine(baseDir, templateFolder);
            var templateSelected = request.SelectedTemplate;

            var templatedFiles = Directory
                .EnumerateFiles(folderPath)
                .Where(file => !file.ToLower().EndsWith(".yaml"))
                .ToList();
            var exceptions = new ConcurrentQueue<Exception>();
            Parallel.ForEach(templatedFiles, templateFile => {
                // get the equivalent data template
                var filename = Path.GetFileNameWithoutExtension(templateFile);
                var dataTemplate = Path.Combine(folderPath, filename + ".yaml");
                var fileExtension = Path.GetExtension(templateFile);
                var mergeExtensions = new List<string>{".doc", ".docx"};
                if (!File.Exists(dataTemplate) || !mergeExtensions.Contains(fileExtension)) return;

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
                catch (System.Exception ex)
                {
                    exceptions.Enqueue(ex);
                }
            });

            if (exceptions.Any())
                return new TemplateResponse{ Status = "Could not create template."};
            return new TemplateResponse{ Status = "Rebind data template completed." };
        }
    }
}