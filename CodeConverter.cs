using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;

namespace BBCodeFormatter
{
    public class CodeConverter
    {
        /// <summary>
        /// <c>settings</c><br/>&emsp;&emsp;A <c>XmlReaderSettings</c> object.
        /// </summary>
        readonly XmlReaderSettings settings;
        internal static readonly ReadOnlyCollection<string> bbtags = new(
            new string[] {
                "b",
                "i",
                "s",
                "u",
                "heading",
                "big",
                "small",
                "sub",
                "sup",
                "center",
                "left",
                "right",
                "justify",
                "hr",
                "indent",
                "color",
                "noparse",
                "url",
                "img",
                "quote",
                "collapse",
                "icon",
                "eicon",
                "user",
            }
        );

        public CodeConverter()
        {
            settings = new()
            {
                Async = true,
                DtdProcessing = DtdProcessing.Parse
            };
        }

        public async void ReadXML(string filename)
        {
            string cwd = Directory.GetCurrentDirectory();
            Console.WriteLine("The current directory is {0}", cwd);
            if (!Directory.Exists($"{cwd}/xml"))
            {
                Console.WriteLine($"Direcory {cwd}/xml does not exist. Creating it now.");
                Directory.CreateDirectory($"{cwd}/xml");
            }
            else if (File.Exists($"{cwd}/xml/{filename}"))
            {
                Console.WriteLine("Directory {0}/xml exists.", cwd);
                Console.WriteLine("Attempting to open {0}/xml/{1}", cwd, filename);
                using FileStream fs = File.Open($"{cwd}/xml/{filename}", FileMode.Open, FileAccess.Read, FileShare.None);
                await ReadAndConvert(fs);
            }
        }

        private async Task TestRead(Stream stream)
        {
            using XmlReader reader = XmlReader.Create(stream, settings);
            while (await reader.ReadAsync())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.IsEmptyElement is true)
                        {
                            Console.WriteLine("Empty Element {0}", reader.Name);
                        }
                        else
                        {
                            Console.WriteLine("Start Element {0}", reader.Name);
                        }
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

        private async Task ReadAndConvert(Stream stream)
        {
            StringBuilder sb = new();
            using XmlReader reader = XmlReader.Create(stream, settings);
            {
                while (await reader.ReadAsync())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (bbtags.Contains(reader.Name))
                            {
                                if (reader.HasAttributes)
                                {
                                    Console.WriteLine($"{reader.Name}=\"{reader.GetAttribute(reader.Name)}\"");
                                    if (!string.IsNullOrWhiteSpace(reader.GetAttribute(reader.Name)))
                                        sb.Append($"[{reader.Name}={reader.GetAttribute(reader.Name)}]");
                                    else
                                        sb.Append($"[{reader.Name}]");
                                    if (!string.IsNullOrWhiteSpace(reader.GetAttribute("font")))
                                        Console.WriteLine("TODO: apply font to contained Text Elements");
                                }
                                else
                                    sb.Append($"[{reader.Name}]");
                            }
                            else if (reader.Name == "br")
                                sb.AppendLine();
                            break;
                        case XmlNodeType.Text:
                            sb.Append(await reader.GetValueAsync());
                            break;
                        case XmlNodeType.EndElement:
                            if (bbtags.Contains(reader.Name))
                                sb.Append($"[/{reader.Name}]");
                            else if (reader.Name == "p" || reader.Name == "para" || reader.Name == "paragraph")
                                sb.AppendLine();
                            break;
                        case XmlNodeType.Whitespace:
                            break;
                        default:
                            Console.WriteLine("Other node {0} with value {1}", reader.NodeType, reader.Value);
                            break;
                    }
                }
            }
            Console.WriteLine(sb.ToString());
        }
    }
}