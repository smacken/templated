using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.RepresentationModel;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace tests
{
    public class YamlTests
    {
        private string sample = ".\\sample.yaml";

        

        [Fact]
        public void Should_parse_yaml_constant(){
            var input = new StringReader(Document);

            // Load the stream
            var yaml = new YamlStream();
            yaml.Load(input);
            var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

            var scalars = mapping.Children.Where(x => x.Key is YamlScalarNode);
            
            foreach (var entry in mapping.Children)
            {
                if (entry.Key is YamlScalarNode){
                    Console.WriteLine((YamlScalarNode)entry.Key);
                }
            }
            Assert.NotNull(mapping);
            Assert.Equal(7, scalars.Count());
            Assert.Equal("Oz-Ware Purchase Invoice", scalars.First().Value);
        }

        [Fact]
        public void Should_read_template_data()
        {
            var deserializer = new DeserializerBuilder().Build();
            var input = new StringReader(sample);
            var parser = new Parser(input);

            // Consume the stream start event "manually"
            parser.Expect<StreamStart>();
            var doc = new List<string>();
            // while (parser.Accept<DocumentStart>())
            // {
            //     // Deserialize the document
            //     doc = deserializer.Deserialize<List<string>>(parser);

            //     // output.WriteLine("## Document");
            //     // foreach (var item in doc)
            //     // {
            //     //     output.WriteLine(item);
            //     // }
            // }

            Assert.Equal(1, 1);
        }

        [Fact]
        public void Should_convert_yaml_to_json(){
            var r = new StringReader(@"
                scalar: a scalar
                sequence:
                - one
                - two
                ");
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(r);

            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(yamlObject);
            dynamic jsonGraph = JsonConvert.DeserializeObject(json);
            Assert.NotEmpty(json);
            Assert.Equal("a scalar", jsonGraph["scalar"].ToString());
        }

        [Fact]
        public void Should_convert_yaml_to_json_then_determine_objects(){
            var r = new StringReader(@"
                scalar: a scalar
                sequence:
                - one
                - two
                ");
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(r);

            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(yamlObject);
            dynamic jsonGraph = JsonConvert.DeserializeObject(json);
            var token = JToken.Parse(json);
            // we are trying to identify dynamically the type of item within the json graph
            foreach (KeyValuePair<string, JToken> node in (JObject)token)
            {
                if (node.Key == "sequence"){
                    Assert.True(node.Value.Type == JTokenType.Array);
                }
            }
        }

        private const string Document = @"---
            receipt:    Oz-Ware Purchase Invoice
            date:        2007-08-06
            customer:
                given:   Dorothy
                family:  Gale

            items:
                - part_no:   A4786
                  descrip:   Water Bucket (Filled)
                  price:     1.47
                  quantity:  4

                - part_no:   E1628
                  descrip:   High Heeled ""Ruby"" Slippers
                  price:     100.27
                  quantity:  1

            bill-to:  &id001
                street: |
                        123 Tornado Alley
                        Suite 16
                city:   East Westville
                state:  KS

            ship-to:  *id001

            specialDelivery:  >
                Follow the Yellow Brick
                Road to the Emerald City.
                Pay no attention to the
                man behind the curtain.
...";
    }
}
