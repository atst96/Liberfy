using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Liberfy
{
    internal static class LinqToXmlHtmlExtensions
    {
        public static IEnumerable<string> GetAttributeValues(this XElement element, string attributeName)
        {
            return element.Attribute(attributeName)?.Value?.Split(' ') ?? Enumerable.Empty<string>();
        }

        public static IEnumerable<string> GetClassNames(this XElement element)
        {
            return element.GetAttributeValues("class");
        }
    }
}
