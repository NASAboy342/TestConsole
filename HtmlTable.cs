using System.Text;

namespace TestConsole
{
    public class HtmlTable
    {
        public List<Tr> Trs { get; set; } = new List<Tr>();
        public HtmlStyle Style { get; set; } = new HtmlStyle();
        public string Class { get; set; }

        public void ToTable<T>(T req) where T : IEnumerable<object>
        {
            GetTableHeader(req);
            GetTableBody(req);
        }

        public string GetHtml()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"<table style=\"{Style.GetAllStyle()}\">");
            stringBuilder.Append("<tr>");
            foreach (var th in Trs[0].Ths)
            {
                stringBuilder.Append("<th>" + th.Value + "</th>");
            }
            stringBuilder.Append("</tr>");
            foreach (var tr in Trs)
            {
                if (Trs.IndexOf(tr) < 1)
                    continue;
                stringBuilder.Append("<tr>");
                foreach (var td in tr.Tds)
                {
                    stringBuilder.Append("<td>" + td.Value + "</td>");
                }
                stringBuilder.Append("</tr>");
            }
            stringBuilder.Append("</table>");
            return stringBuilder.ToString();
        }

        private void GetTableBody<T>(T req) where T : IEnumerable<object>
        {
            foreach (var item in req)
            {
                var properties = item.GetType().GetProperties();
                Trs.Add(new Tr());
                Trs[Trs.Count - 1].Tds = new List<Td>();
                foreach (var property in properties)
                {
                    Trs[Trs.Count - 1].Tds.Add(new Td
                    {
                        Value = property.GetValue(item)?.ToString() ?? "",
                        Class = ""
                    });
                }
            }
        }

        public void GetTableHeader<T>(T req) where T : IEnumerable<object>
        {
            var firstObject = req.FirstOrDefault();
            var properties = firstObject.GetType().GetProperties();
            Trs.Add(new Tr());
            Trs[0].Ths = new List<Th>();
            foreach (var property in properties)
            {
                Trs[0].Ths.Add(new Th
                {
                    Value = property.Name,
                    Class = ""
                });
            }
        }
    }

    public class Tr
    {
        public List<Th> Ths { get; set; }
        public List<Td> Tds { get; set; }
    }

    public class Th
    {
        public string Value { get; set; }
        public string Class { get; set; }
        public HtmlStyle Style { get; set; }
    }

    public class Td
    {
        public string Value { get; set; }
        public string Class { get; set; }
        public HtmlStyle Style { get; set; }
    }
}