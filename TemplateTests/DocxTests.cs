using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
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

        private string ReplaceFunc(Dictionary<string, string> replacePatterns, string findStr)
        {
            if(replacePatterns.ContainsKey(findStr))
            {
                return replacePatterns[findStr];
            }
            return findStr;
        }

        [Fact]
        public void LoadsWordDoc()
        {
            var path = Path.GetFullPath("file-sample_100kB.doc");
            var fileStream = File.Open(path, FileMode.Open);
            using(DocX document = DocX.Load("file-sample_100kB.docx" )) // GetPath("Employment.doc")))
            {
                //"Lorem ipsum Lorem ipsum dolor sit amet,"
                Assert.Equal("Lorem", document.Text.Split(' ')[0]);
            }
        }

        [Fact]
        public void ReplacesTextInWordDoc()
        {
            Dictionary<string, string> replacePatterns = new Dictionary<string, string>()
            {
                { "Lorem", "LOREM" }
            };

            Func<string, string> replace = (findStr) => replacePatterns.ContainsKey(findStr) ? replacePatterns[findStr] : findStr;
            using(DocX document = DocX.Load("file-sample_100kB.docx" ))
            {
                //"Lorem ipsum Lorem ipsum dolor sit amet,"
                foreach (var item in replacePatterns)
                {
                    document.ReplaceText(item.Key, replacePatterns.GetValueOrDefault(item.Key));
                }

                Assert.Equal("LOREM", document.Text.Split(' ')[0]);
            }

            File.Delete(@".\temp.docx");
        }
    }
}
