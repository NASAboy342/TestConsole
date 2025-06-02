using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    public class HtmlStyle
    {
        public string BorderCollapse { get; set; } = "separate";
        public string GetBorderCollapse => $"border-collapse: {BorderCollapse}"; 
        public string GetAllStyle()
        {
            var stingbuilder = new StringBuilder();
            var properties = this.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (!property.Name.StartsWith("Get"))
                    continue;
                stingbuilder.Append(property.GetValue(this)+"; ");
            }
            return stingbuilder.ToString();
        }
    }
}