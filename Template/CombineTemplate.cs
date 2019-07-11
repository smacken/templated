using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

    public class CombineTemplate : RequestHandler<CombineRequest, TemplateResponse>
    {
        protected override TemplateResponse Handle(CombineRequest request)
        {
            var templateFolder = request.FolderName;
            if (string.IsNullOrEmpty(templateFolder))
                throw new ArgumentException("Invalid template folder");
            
            var baseDir = Directory.GetParent(Path.GetDirectoryName(request.TemplatePath)).FullName;
            var folderPath = Path.Combine(baseDir, templateFolder);
            var templateSelected = request.SelectedTemplate;

            var joinFile = Path.Combine(folderPath, "join.yaml");
            if (!File.Exists(joinFile))
                return new TemplateResponse{Status = "Unable to combine files."};
            string content;
            using (StreamReader streamReader = new StreamReader(joinFile, Encoding.UTF8))
            {
                content = streamReader.ReadToEnd();
            }
            var input = new StringReader(content);
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(input);

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
                    if (mergeDocs.Count >= 2)
                        AppendDocument(folderPath, node.Key, mergeDocs.ToArray());
                }
            }

            return new TemplateResponse {Status = "Combine completed."};
        }

        protected void AppendDocument(string outputFolder, string outputFile, params string[] inputfiles)
        {
            using( DocX root = DocX.Load(Path.Combine(outputFolder, inputfiles[0])))
            {
                for (int i = 1; i < inputfiles.Length; i++)
                {
                    using( DocX document2 = DocX.Load(Path.Combine(outputFolder, inputfiles[i])))
                    {
                        root.InsertDocument(document2, true);
                    }
                }
                root.SaveAs(Path.Combine(outputFolder, outputFile));
            }
        }
    }
}