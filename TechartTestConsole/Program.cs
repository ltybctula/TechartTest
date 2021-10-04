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

                foreach(string s in args)
                {
                    Console.WriteLine(s);
                }

                string extension="xml", fileName;

                fileName = args[0].Substring(args[0].LastIndexOf('\\')+1);
                fileName = fileName.Substring(fileName.LastIndexOf('.') + 1);

                foreach (string stringArgs in args)
                {
                    if(stringArgs.Contains("format:"))
                    {
                        extension = stringArgs.Split(':')[1];
                    }
                    else if(stringArgs.Contains("outputfile:"))
                    {
                        fileName = stringArgs.Split(':')[1];
                    }

                    Console.WriteLine(stringArgs);
                }

                Data data = new Data(args[0]);
                switch (extension.ToLower())
                {
                    case "xml":
                        XmlSerializer serializer = new XmlSerializer(typeof(Data));
                        using (FileStream fs = new FileStream(fileName + "." + extension, FileMode.Create))
                        {
                            XmlWriter writer = XmlWriter.Create(fs);
                            var xsn = new XmlSerializerNamespaces();
                            xsn.Add(string.Empty, string.Empty);
                            serializer.Serialize(writer, data, xsn);
                            fs.Close();
                        }
                        break;
                    case "json":
                        string json = JsonSerializer.Serialize(data);
                        using (StreamWriter outputFile = new StreamWriter(fileName + "." + extension))
                        {
                            outputFile.WriteLine(json);
                        }
                        break;
                    case "txt":
                        using (StreamWriter outputFile = new StreamWriter(fileName + "." + extension))
                        {
                            outputFile.WriteLine(data.ToString());
                        }
                        break;
                    default:
                        Console.WriteLine(extension);
                        Console.WriteLine("Извините, данный формат не поддреживается");
                        Help();
                        break;
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
