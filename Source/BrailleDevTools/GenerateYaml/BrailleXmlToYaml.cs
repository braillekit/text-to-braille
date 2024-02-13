using BrailleToolkit.Data;
using BrailleToolkit.Helpers;
using Huanlin.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace GenerateYaml
{
    public static class BrailleXmlToYaml
    {      
        /// <summary>
        /// 此方法將 XmlBrailleTable 物件轉換成 YAML 格式的字串。
        /// </summary>
        /// <param name="brlTable"></param>
        /// <returns></returns>
        public static string ToYamlString(this XmlBrailleTable brlTable)
        {
            var list = new List<BrailleSymbol>();            
            brlTable.Table.Rows.Cast<DataRow>().ToList().ForEach(row =>
            {              
                var brSymbol = new BrailleSymbol 
                {
                    Text = row["Text"].ToString()!,
                    //ShortText = row["short"] == null ? "" : row["short"].ToString()!,
                    Description = row["Description"].ToString()! 
                };

                if (String.IsNullOrEmpty(brSymbol.Description))
                {
                    brSymbol.Description = null;
                }

                string codeString = row["code"].ToString()!;
                var sb = new StringBuilder();
                for (int i = 0; i < codeString.Length; i += 2)
                {
                    var oneCode = codeString.Substring(i, 2);
                    brSymbol.Dots.Add(BrailleCellHelper.HexStringToPositionNumberString(oneCode));
                }

                list.Add(brSymbol);
            });

            var serializer = new SerializerBuilder()
                .WithIndentedSequences()
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull);
            var yaml = serializer.Build().Serialize(list);
            return yaml;
        }
    }
}
