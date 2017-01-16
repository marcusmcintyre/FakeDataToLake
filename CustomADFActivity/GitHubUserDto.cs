using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomADFActivity
{
    class GitHubUserDto
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string HairColor { get; set; }
        public double Height { get; set; }


        public override string ToString()
        {
            var properties = this.GetType()
                .GetProperties()
                .OrderBy(p => p.Name)
                .ToList();

            var item = new List<string>();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(this, null) != null ? prop.GetValue(this, null).ToString() : null;

                if (value != null)
                {
                    if (prop.PropertyType == typeof(string))
                    {

                        value = value.Replace("\"", "'").Replace("\n", "").Replace("\r", "");
                        item.Add($@"""{value}""");
                        continue;
                    }

                    item.Add(value);
                }
                else
                {
                    item.Add("");
                }

            }


            return string.Join(",", item);
        }
    }
}
