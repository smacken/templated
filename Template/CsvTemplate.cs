namespace tempalted {
    using System.IO;
    using System.Collections.Generic;
    public class CsvTemplate {
        public List<string> ReadCsv(string name, string folder){
            string csv = Path.Combine(folder, name);
            var rows = new List<string>();
            using(var reader = new StreamReader(csv))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    rows.Add(values[0]);
                }
            }
            return rows;
        }
    }
}