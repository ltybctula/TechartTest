using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace TechartTest
{
    public partial class Form1 : Form
    {
        public Source Source = new Source();
        Data myData;

        public Form1()
        {
            InitializeComponent();
            buttonSaveFile.Enabled = false;
        }

        private void ButtonSaveFile_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Data));
                FileStream fs = new FileStream(saveFileDialog1.FileName+".xml", FileMode.Create);
                XmlWriter writer = XmlWriter.Create(fs);
                var xns = new XmlSerializerNamespaces();
                xns.Add(string.Empty, string.Empty);
                serializer.Serialize(writer, myData, xns);
                writer.Dispose();
                fs.Close();

                string json = JsonSerializer.Serialize(myData);
                File.WriteAllText(saveFileDialog1.FileName+".json", json);
            }
        }

        private void ButtonOpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Text = "";
                Source.Records.Clear();
                string line, device = "";
                StreamReader file = new StreamReader(File.OpenRead(openFileDialog1.FileName));
                while ((line = file.ReadLine()) != null)
                {
                    line = line.Trim();
                    Source.Records.Add(new Record(line.Split("\t")));
                }
                file.Close();
                Source.Records = Source.Records.OrderBy(record => record.Address).ToList();

                /*#region richText
                StringBuilder str = new StringBuilder();
                foreach (Record record in Source.Records)
                {
                    str.Clear();
                    if (!device.Equals(record.Address))
                    {
                        device = record.Address;
                        str.AppendFormat("SourceType:={0}; ", record.Address);
                        str.AppendLine();
                    }
                    str.AppendFormat("\tNumber={0}; ", record.Number);
                    str.AppendFormat("Result={0}; ", record.Result);
                    str.AppendFormat("Request.Addr={0}; ", record.Request.Address.ToString("X2"));
                    if (record.Request.Lenght == 5)
                    {
                        str.AppendFormat("Request.Function={0}:{1}; ", record.Request.Function.ToString("X2"), record.Request.Command);

                        str.AppendFormat("Request.Data=");
                        foreach (byte b in record.Request.Data)
                        {
                            str.AppendFormat("{0}|", b.ToString("X2"));
                        }
                    }
                    else if (record.Request.Lenght >= 4)
                    {
                        str.AppendFormat("Request.Function={0}:{1}; ", record.Request.Function.ToString("X2"), record.Request.Command);

                        str.AppendFormat("Request.Data=");
                        foreach (byte b in record.Request.Data)
                        {
                            str.AppendFormat("{0}|", b.ToString("X2"));
                        }
                    }
                    str.AppendLine();
                    richTextBox1.Text += str.ToString();
                }
                richTextBox1.Text += new String('-', 20);
                richTextBox1.Text += "\n";
                richTextBox1.Text += "Файл прочитан.";
                #endregion*/

                myData = new Data();
                myData.Source_type = Source.SourceType;
                DataSource dataSource = new DataSource();
                DataSourceLine dataSourceLine;
                device = "";
                foreach (Record record in Source.Records)
                {
                    dataSourceLine = new DataSourceLine();
                    if (!device.Equals(record.Address))
                    {
                        dataSource = new DataSource();
                        device = record.Address;
                        dataSource.Address = record.Address;
                        dataSource.Speed = record.Speed;
                        myData.Source.Add(dataSource);
                    }
                    dataSourceLine.direction = record.Direction;
                    if (String.IsNullOrEmpty(record.Error))
                    {
                        dataSourceLine.address = record.Request.Address.ToString("X2");
                        dataSourceLine.command = record.Request.Function.ToString("X2") + ":" + record.Request.Command;
                        dataSourceLine.crc = record.Request.CRC.ToString("X2");
                        dataSourceLine.raw_data = record.Request.Raw_data;
                        dataSourceLine.raw_frame = record.Raw_frame;
                    }
                    else
                    {
                        dataSourceLine.error = record.Error;
                    }

                    //dataSourceLine.exception = record.Request.Exception;

                    dataSource.Line.Add(dataSourceLine);
                }
                buttonSaveFile.Enabled = true;
            }
        }
        uint CRC16(byte[] data, int data_size)
        {
            const uint MODBUS_CRC_CONST = 0xA001;
            uint CRC = 0xFFFF;

            for (int i = 0; i < data_size; i++)
            {
                CRC ^= (uint)data[i];
                for (int k = 0; k < 8; k++)
                {
                    if ((CRC & 0x01) == 1)
                    {
                        CRC >>= 1;
                        CRC ^= MODBUS_CRC_CONST;
                    }
                    else
                        CRC >>= 1;
                }
            }

            return CRC;
        }
    }
    /*
    <!-- Типы тегов и атрибутов: -->
    <!-- M(mandatory) - обязательный -->
    <!-- O(optional) - необязательный, в зависимости от содержания данных -->
    <!-- S(single) - тег может повторяться только один раз -->
    <!-- P(plural) - тег может повторяться более одного раза -->

    <!-- [M, S] data - корневой элемент выходного файла.Может содержать 0 и более элементов source -->
    <!-- [M, S] тип источника данных: { com, unknown } -->
    <data source_type = "" >

        < !-- [M, P] source - элемент источника, который содержит все данные для данного источника из всего файла -->
        <!-- [M, S] address - адрес источника, если доступен.Например, COM2 или unknown -->
        <!-- [O, S] speed - скорость передачи данных(Например, 9600, 19200, 115200) -->
        <source address = "" speed="">
            <!-- [O, P] line - элемент данных, считанный из лога.Полный запрос/ответ обычно занимают во входном логе одну строку -->
            <!-- [M, S] direction - направление данных: request, response, unknown -->
            <!-- [O, S] address - адрес запрашиваемого/отвечающего устройства (1 байт в HEX формате. Например, address= "68") -->
            <!-- [O, S] command - команда к(от) устройству(формат: <номер команды>:<описание>. Например, command= "46:Write EEPROM")-->
            <!-- [O, S] exception - атрибут описания исключения(только для типов direction = "response" или direction = "unknown"). 
            Формат: <номер исключения>:<описание>. Например, exception="04:Data error" -->
            <!-- [O, S] error - атрибут описания ошибки(только для типов direction = "response" или direction = "unknown"). 
            Формат: <описание>. Например, error="Frame length error", error="Timeout". 
            При наличии данного атрибута остальные атрибуты и тег raw_data могут быть опущены -->
            <!-- [O, S] crc - CRC16 контроль целостности данных -->
            <line
                direction = ""

                address=""
                command=""
                exception=""
                error=""
                crc="">
                <!-- raw_frame - полный пакет данных(HEX данные) -->
                <raw_frame>frame</raw_frame>
                <!-- raw_data - данные без заголовка и CRC(HEX данные). 
                Т.е.данные без address, command и crc.
               Данный тег не нужен в случае, если в пакете содержится ошибка или нарушена целостность(по CRC) -->
                <raw_data>data</raw_data>
            </line>
        </source>
    </data>
    */

    public class Source
    {
        public string SourceType { get; set; }
        public List<Record> Records { get; set; }
        public Source()
        {
            SourceType = "COM";
            Records = new List<Record>();
        }
    }

    public class Record
    {
        public int Number { get; set; }
        public string Time { get; set; }
        public string Direction { get; set; }
        public string Address { get; set; }
        public string Speed { get; set; }
        public string Result { get; set; }
        public string Raw_frame { get; set; }
        public string Error { get; set; }
        public Request Request { get; set; }
        public Record(string[] line)
        {
            string[] direction = line[3].Split('_');
            Number = Convert.ToInt32(line[0]);
            Time = line[1];
            Address = line[4];
            Speed = "Unknown";
            Result = line[5];
            if (!line[5].Equals("TIMEOUT"))
            {
                Raw_frame = line[6];
                Request = new Request(line[6]);
            }
            else
            {
                Error = line[5];
            }
            switch (direction[^1])
            {
                case "READ":
                    Direction = "response";
                    break;
                case "WRITE":
                    Direction = "request";
                    break;
                default:
                    Direction = "unknown";
                    break;
            }
        }
    }

    public class Request
    {
        public int Lenght { get; set; }
        public byte Address { get; set; }
        public byte Function { get; set; }
        public string Command { get; set; }
        public string Raw_data { get; set; }
        public List<byte> Data { get; set; }
        public uint CRC { get; set; }

        public Request(string _data)
        {
            string[] parse = _data.Split(" ");
            Exceptions exceptions = new Exceptions();
            Commands commands = new Commands();
            Lenght = Convert.ToInt32(parse[1].TrimEnd(':'));
            if (Lenght == 5)
            {
                string crc = parse[^2] + parse[^1];
                CRC = Convert.ToUInt32(crc, 16);
                Address = Convert.ToByte(parse[2], 16);
                Function = Convert.ToByte(parse[3], 16);
                string[] data = parse[4..(parse.Length - 2)];
                Data = new List<byte>();
                for (int i = 0; i < data.Length; i++)
                {
                    Data.Add(Convert.ToByte(data[i], 16));
                }

                if (exceptions.Exception.ContainsKey(Function))
                {
                    Command = commands.Command[Function];
                }
                else
                {
                    Command = "неизвестная ошибка";
                }
            }
            else if (Lenght >= 4)
            {
                Raw_data = String.Join(" ", parse[4..(parse.Length - 2)]);
                string crc = parse[^2] + parse[^1];
                CRC = Convert.ToUInt32(crc, 16);
                Address = Convert.ToByte(parse[2], 16);
                Function = Convert.ToByte(parse[3], 16);
                string[] data = parse[4..(parse.Length - 2)];
                Data = new List<byte>();
                for (int i = 0; i < data.Length; i++)
                {
                    Data.Add(Convert.ToByte(data[i], 16));
                }

                if (commands.Command.ContainsKey(Function))
                {
                    Command = commands.Command[Function];
                }
                else
                {
                    Command = "неизвестная команда";
                }
            }
            else if (Lenght == 1)
            {
                Address = Convert.ToByte(parse[2], 16);
            }
        }
    }

    public class Commands
    {
        public Dictionary<int, string> Command { get; set; }
        public Commands()
        {
            string line;
            Command = new Dictionary<int, string>();
            StreamReader fileCommands = new StreamReader(File.OpenRead("commands.vcb"));
            while ((line = fileCommands.ReadLine()) != null)
            {
                string[] parse = line.Split(" - ");
                Command.Add(Convert.ToByte(parse[0], 16), parse[1]);
            }
            fileCommands.Close();
        }
    }

    public class Exceptions
    {
        public Dictionary<int, string> Exception { get; set; }
        public Exceptions()
        {
            string line;
            Exception = new Dictionary<int, string>();
            StreamReader fileExceptions = new StreamReader(File.OpenRead("exceptions.vcb"));
            while ((line = fileExceptions.ReadLine()) != null)
            {
                string[] parse = line.Split(" - ");
                Exception.Add(Convert.ToByte(parse[0], 16), parse[1]);
            }
            fileExceptions.Close();
        }
    }

    [XmlRoot("data")]
    public partial class Data
    {
        [System.Xml.Serialization.XmlElementAttribute("source", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<DataSource> Source { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute("source_type")]
        public string Source_type { get; set; }
        public Data()
        {
            Source = new List<DataSource>();
        }
    }

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class DataSource
    {

        [System.Xml.Serialization.XmlElementAttribute("line", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<DataSourceLine> Line { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute("Address")]
        public string Address { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute("Speed")]
        public string Speed { get; set; }
        public DataSource()
        {
            Line = new List<DataSourceLine>();
        }
    }

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class DataSourceLine
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string raw_frame { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string raw_data { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string direction { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string address { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string command { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string exception { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string error { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string crc { get; set; }
    }

    /*[System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class NewDataSet
    {
        [System.Xml.Serialization.XmlElementAttribute("data")]
        public List<Data> Items { get; set; }
    }*/
}

