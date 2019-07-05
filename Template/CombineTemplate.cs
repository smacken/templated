

using System;
using System.Collections.Generic;
using System.IO;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Novacode;
using YamlDotNet.Serialization;

namespace templated 
{
    public class CombineRequest : IRequest<TemplateResponse> {
        public string FolderName { get; set; }
        public string SelectedTemplate { get; set; }
        public string TemplatePath { get; set; }
    }

    public class CombineTempalate : RequestHandler<CombineRequest, TemplateResponse>
    {
        protected override TemplateResponse Handle(CombineRequest request)
        {
            var templateFolder = request.FolderName;
            if (string.IsNullOrEmpty(templateFolder))
                throw new ArgumentException("Invalid template folder");
            
            var baseDir = Directory.GetParent(Path.GetDirectoryName(request.TemplatePath)).FullName;
            var folderPath = Path.Combine(baseDir, templateFolder);
            var templateSelected = request.SelectedTemplate;

            var joinFile = Path.Combine(folderPath, templateSelected, "join.yaml");
            if (!File.Exists(joinFile))
                return new TemplateResponse();
            var content = File.ReadAllText(joinFile);
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize<string>(content);

            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(yamlObject);
            dynamic jsonGraph = JsonConvert.DeserializeObject(json);
            var token = JToken.Parse(json);
            foreach (KeyValuePair<string, JToken> node in (JObject)token)
            {
                if (node.Value.Type == JTokenType.Array){
                    var mergeDocs = node.Value.ToObject<List<string>>();
                    if (mergeDocs.Count > 2)
                        AppendDocument(node.Key, mergeDocs[0], mergeDocs[1]);
                }
            }

            return new TemplateResponse();
        }

        //params string[] inputfiles - multiple files
        protected void AppendDocument(string outputFile, string firstFile, string secondFile)
        {
            using( DocX document1 = DocX.Load(firstFile))
            using( DocX document2 = DocX.Load(secondFile))
            {
                document1.InsertDocument(document2, true);
                document1.SaveAs(outputFile);
            }
        }
    }
}