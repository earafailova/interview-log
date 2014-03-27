using interview_log.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace interview_log.Helpers
{
    public class TagsConverter : CsvHelper.TypeConversion.ITypeConverter
    {

        public bool CanConvertFrom(System.Type type)
        {
            return true;
        }

        public bool CanConvertTo(System.Type type)
        {
            return true;
        }

        public object ConvertFromString(CsvHelper.TypeConversion.TypeConverterOptions options, string text)
        {
            return text.Split(new[] { ',' });
        }

        public string ConvertToString(CsvHelper.TypeConversion.TypeConverterOptions options, object tags)
        {
            StringBuilder strBuidler = new StringBuilder();
            foreach (Tag tag in (HashSet<Tag>)tags)
            {
                strBuidler.Append(tag.Name);
                strBuidler.Append(",");
            }
            return strBuidler.ToString();
        }
    }
}