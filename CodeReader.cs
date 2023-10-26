using System;
using System.Xml;

namespace BBCodeFormatter
{
    public class CodeReader
    {
        /// <summary>
        /// <c>settings</c><br/>&emsp;&emsp;A <c>XmlReaderSettings</c> object.
        /// </summary>
        XmlReaderSettings settings;

        public CodeReader()
        {
            settings = new()
            {
                Async = true,
                DtdProcessing = DtdProcessing.Parse
            };
        }

        public async Task TestRead(System.IO.Stream stream)
        {
            using XmlReader reader = XmlReader.Create(stream, settings);
            while (await reader.ReadAsync())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        Console.WriteLine("Start Element {0}", reader.Name);
                        break;
                    case XmlNodeType.Text:
                        Console.WriteLine("Text Node: {0}", await reader.GetValueAsync());
                        break;
                    case XmlNodeType.EndElement:
                        Console.WriteLine("End Element {0}", reader.Name);
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                    default:
                        Console.WriteLine("Other node {0} with value {1}", reader.NodeType, reader.Value);
                        break;
                }
            }
        }
    }
}