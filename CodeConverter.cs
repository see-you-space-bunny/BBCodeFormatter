using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace BBCodeFormatter
{
    public class CodeConverter
    {
        /// <summary>
        /// A <c>XmlReaderSettings</c> object for internal use by the xml reader.<br/><br/>
        /// The default configuration is:<br/>
        /// &emsp;• <c>Async</c> = true<br/>
        /// &emsp;• <c>DtdProcessing</c> = <c>DtdProcessing.Parse</c><br/>
        /// <br/>
        /// Currently no constructor overloads to pass different settings.<br/>
        /// Async must always be true regardless.
        /// </summary>
        internal readonly XmlReaderSettings settings;

        /// <summary>
        /// A static <c>ReadOnlyCollection&lt;string&gt;</c> object containing a list of valid BB-Code tags.<br/>
        /// <br/>
        /// This list will later be extracted from a configuration file containing multiple presets of valid BB-Code tags.
        /// </summary>
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

        /// <summary>
        /// Class Constructor using default settings.<br/><br/>
        /// The default configuration is:<br/>
        /// &emsp;• <c>Async</c> = true<br/>
        /// &emsp;• <c>DtdProcessing</c> = <c>DtdProcessing.Parse</c><br/>
        /// </summary>
        public CodeConverter()
        {
            settings = new()
            {
                Async = true,
                DtdProcessing = DtdProcessing.Parse
            };
        }

        /// <summary>
        /// Reads and then Converts an xml file into BB-Code.<br/>
        /// <br/>
        /// If no ./xml directory exists it will instead create one.<br/>
        /// TODO: create an example.xml file within the directory.
        /// </summary>
        /// <param name="filename"><c>string</c> object pointing to an xml file <u><b>in the ./xml directory</b></u>.</param>
        /// <returns></returns>
        public async Task<string?> ReadAndConvert(string filename)
        {
            string cwd = Directory.GetCurrentDirectory();
            if (!Directory.Exists($"{cwd}/xml"))
            {
                Console.WriteLine($"Direcory {cwd}/xml does not exist. Creating it now.");
                Directory.CreateDirectory($"{cwd}/xml");
            }
            else if (File.Exists($"{cwd}/xml/{filename}"))
            {
                using FileStream fs = File.Open($"{cwd}/xml/{filename}", FileMode.Open, FileAccess.Read, FileShare.None);
                {
                    StringBuilder sb = new();
                    using XmlReader reader = XmlReader.Create(fs, settings);
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
                                            if (!string.IsNullOrWhiteSpace(reader.GetAttribute(reader.Name)))
                                                sb.Append($"[{reader.Name}={reader.GetAttribute(reader.Name)}]");
                                            else
                                                sb.Append($"[{reader.Name}]");
                                            if (!string.IsNullOrWhiteSpace(reader.GetAttribute("font")))
                                                Console.WriteLine("TODO: apply unicode font to contained Text Elements");
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
                    Console.WriteLine("Conversion complete!\r\n---------------------\r\n");
                    return sb.ToString();
                }
            }
            return null;
        }
    }
}