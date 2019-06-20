using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Novacode;
using Xunit;

namespace TemplateTests
{
    public class DocxTests
    {
        private string GetPath(string relativePath){
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            return Path.GetFullPath(Path.Combine(dirPath, @"../../../")) + relativePath;
        }

        [Fact]
        public void Should_load_word_doc()
        {
            using(DocX document = DocX.Load(GetPath("Employment.doc")))
            {
                
                Assert.Equal(1, document.Text.Length);
            }
        }

        [Fact]
        public void Should_replace_text_in_word_doc()
        {
            // private Dictionary<string, string> _replacePatterns = new Dictionary<string, string>
            // {
            //     { "OPPONENT", "Pittsburgh Penguins" },
            //     { "GAME_TIME", "19h30" },
            //     { "GAME_NUMBER", "161" },
            //     { "DATE", "October 18 2016" }
            // };
            
        }

        [Fact]
        public void Should_template_merge_from_yaml()
        {

        }
    }
}
