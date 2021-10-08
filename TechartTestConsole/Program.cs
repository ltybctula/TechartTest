using System;
using System.IO;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using TechartTest;

namespace TechartTest
{
    class Program
    {

        //inputfile: [format:xml|txt|json] [outputfile:]

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {

                string extension = "xml", fileName;

                fileName = args[0].Substring(args[0].LastIndexOf('\\') + 1);
                fileName = fileName.Substring(0, fileName.LastIndexOf('.'));

                foreach (string stringArgs in args)
                {
                    if (stringArgs.Contains("format:"))
                    {
                        extension = stringArgs.Split(':')[1];
                    }
                    else if (stringArgs.Contains("outputfile:"))
                    {
                        fileName = stringArgs.Split(':')[1];
                    }
                }

                Data data = new Data(args[0]);
                using (FileStream fileStream = new FileStream(fileName + "." + extension, FileMode.Create))
                {
                    StreamWriter outputFile = new StreamWriter(fileStream);
                    switch (extension.ToLower())
                    {
                        case "xml":
                            XmlSerializer serializer = new XmlSerializer(typeof(Data));
                            XmlWriter writer = XmlWriter.Create(fileStream);
                            var xsn = new XmlSerializerNamespaces();
                            xsn.Add(string.Empty, string.Empty);
                            serializer.Serialize(writer, data, xsn);
                            break;

                        case "json":
                            string json = JsonSerializer.Serialize(data);
                            outputFile.Write(json);

                            break;

                        case "txt":
                            outputFile.Write(data.ToString());
                            break;

                        default:
                            Console.WriteLine(extension);
                            Console.WriteLine("Извините, данный формат не поддреживается");
                            Help();
                            break;
                    }
                    outputFile.Close();
                }
            }
            else
            {
                Help();
            }
        }

        static void Help()
        {
            Console.WriteLine("inputfile: [format:xml|txt|json] [outputfile:]");
            Console.WriteLine("inputfile: входной файл.");
            Console.WriteLine("format: формат выходного файла. по умолчанию xml.");
            Console.WriteLine("outputfile: выходной файл. если не задан - имя_входного_файла.format.");
        }
    }
}
