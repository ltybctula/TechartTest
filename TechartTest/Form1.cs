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

                var extension = Path.GetExtension(saveFileDialog1.FileName);

                switch (extension.ToLower())
                {
                    case ".xml":
                        XmlSerializer serializer = new XmlSerializer(typeof(Data));
                        FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                        XmlWriter writer = XmlWriter.Create(fs);
                        var xsn = new XmlSerializerNamespaces();
                        xsn.Add(string.Empty, string.Empty);
                        serializer.Serialize(writer, myData, xsn);
                        fs.Close();
                        break;
                    case ".json":
                        string json = JsonSerializer.Serialize(myData);
                        File.WriteAllText(saveFileDialog1.FileName, json);
                        break;
                    case ".txt":
                        StringBuilder str = new StringBuilder();
                        str.AppendFormat("SourceType: {0}\r\n", myData.Source_type);
                        foreach (DataSource ds in myData.Source)
                        {
                            str.AppendFormat("\tSource: Address= {0} Speed={1}\r\n", ds.Address, ds.Speed);
                            foreach (DataSourceLine dsl in ds.Line)
                            {
                                if (String.IsNullOrEmpty(dsl.error))
                                {
                                    str.AppendFormat("\t\tLine: Direction={0} Address={1}", dsl.direction, dsl.address);
                                    if (!String.IsNullOrEmpty(dsl.command))
                                    {
                                        str.AppendFormat(" Command='{0}'", dsl.command);
                                    }
                                    if (!String.IsNullOrEmpty(dsl.exception))
                                    {
                                        str.AppendFormat(" Exception='{0}'", dsl.exception);
                                    }
                                    if (dsl.error_crc)
                                    {
                                        str.AppendFormat(" Error='Wrong CRC'");
                                    }
                                    if (!String.IsNullOrEmpty(dsl.crc))
                                    {
                                        str.AppendFormat(" CRC='{0}'", dsl.crc);
                                    }
                                    str.AppendLine();

                                    str.AppendFormat("\t\t\tRawFrame: {0}\r\n", dsl.raw_frame);
                                    str.AppendFormat("\t\t\tRawData: {0}\r\n", dsl.raw_data);
                                }
                                else
                                {
                                    str.AppendFormat("\t\tLine: Direction={0} Error='{1}'\r\n", dsl.direction, dsl.error);
                                }
                            }
                        }
                        File.WriteAllText(saveFileDialog1.FileName, str.ToString());
                        break;
                    default:
                        MessageBox.Show("Извините, данный формат не поддреживается", "Warning", MessageBoxButtons.OK);
                        break;
                }
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
                        device = record.Address;
                        dataSource = new DataSource();
                        dataSource.Address = record.Address;
                        dataSource.Speed = record.Speed;
                        myData.Source.Add(dataSource);
                    }
                    dataSourceLine.direction = record.Direction;
                    if (String.IsNullOrEmpty(record.Error))
                    {
                        dataSourceLine.address = record.Request.Address.ToString("X2");
                        if (record.Request.Lenght > 1)
                        {
                            if(String.IsNullOrEmpty(record.Request.Exception))
                            {
                                dataSourceLine.command = record.Request.Function.ToString("X2") + ":" + record.Request.Command;
                            }
                            else
                            {
                                dataSourceLine.exception = record.Request.Function.ToString("X2") + ":" + record.Request.Exception;
                            }
                            dataSourceLine.error_crc = record.Error_CRC;
                            dataSourceLine.crc = record.Request.CRC.ToString("X2");
                            dataSourceLine.raw_data = record.Request.Raw_data;
                            dataSourceLine.raw_frame = record.Raw_frame;
                        }
                    }
                    else
                    {
                        dataSourceLine.error = record.Error;
                    }
                    dataSource.Line.Add(dataSourceLine);
                }

                #region richText
                StringBuilder str = new StringBuilder();
                str.AppendFormat("SourceType: {0}\r\n", myData.Source_type);
                foreach (DataSource ds in myData.Source)
                {
                    str.AppendFormat("\tSource: Address= {0} Speed={1}\r\n", ds.Address, ds.Speed);
                    foreach (DataSourceLine dsl in ds.Line)
                    {
                        if (String.IsNullOrEmpty(dsl.error))
                        {
                            str.AppendFormat("\t\tLine: Direction={0} Address={1}", dsl.direction, dsl.address);
                            if (!String.IsNullOrEmpty(dsl.command))
                            {
                                str.AppendFormat(" Command='{0}'", dsl.command);
                            }
                            if (!String.IsNullOrEmpty(dsl.exception))
                            {
                                str.AppendFormat(" Exception='{0}'", dsl.exception);
                            }
                            if (dsl.error_crc)
                            {
                                str.AppendFormat(" Error='Wrong CRC'");
                            }
                            if (!String.IsNullOrEmpty(dsl.crc))
                            {
                                str.AppendFormat(" CRC='{0}'", dsl.crc);
                            }
                            str.AppendLine();

                            str.AppendFormat("\t\t\tRawFrame: {0}\r\n", dsl.raw_frame);
                            str.AppendFormat("\t\t\tRawData: {0}\r\n", dsl.raw_data);
                        }
                        else
                        {
                            str.AppendFormat("\t\tLine: Direction={0} Error='{1}'\r\n", dsl.direction, dsl.error);
                        }
                    }
                }
                richTextBox1.Text += str;
                richTextBox1.Text += new String('-', 20);
                richTextBox1.Text += "\r\n";
                richTextBox1.Text += "Файл прочитан.";
                #endregion

                buttonSaveFile.Enabled = true;
                saveFileDialog1.FileName = openFileDialog1.SafeFileName;
            }
        }
    }

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
        public bool Error_CRC { get; set; }
        public Request Request { get; set; }
        public Record(string[] line)
        {
            string[] direction = line[3].Split('_');
            Number = Convert.ToInt32(line[0]);
            Time = line[1];
            Address = line[4];
            Speed = "Unknown";
            Result = line[5];
            Error_CRC = false;
            if (!line[5].Equals("TIMEOUT"))
            {
                Raw_frame = line[6].Substring(line[6].IndexOf(':')+1).Trim();
                Request = new Request(line[6]);
                string[] parse = Raw_frame.Split(' ');
                if (parse.Length >= 4)
                {
                    byte[] data = new byte[parse.Length - 2];
                    for (int i = 0; i < parse.Length - 2; i++)
                    {
                        data[i] = Convert.ToByte(parse[i],16);
                    }
                    uint calc_crc = CRC16(data, data.Length);
                    if (Request.CRC != calc_crc)
                    {
                        Error_CRC = true;
                    }
                }
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

    public class Request
    {
        public int Lenght { get; set; }
        public byte Address { get; set; }
        public byte Function { get; set; }
        public string Command { get; set; }
        public string Exception { get; set; }
        public string Raw_data { get; set; }
        public List<byte> Data { get; set; }
        public uint CRC { get; set; }

        public Request(string _data)
        {
            string[] parse = _data.Split(" ");
            Exceptions exceptions = new Exceptions();
            Commands commands = new Commands();
            Lenght = Convert.ToInt32(parse[1].TrimEnd(':'));
            Address = Convert.ToByte(parse[2], 16);
            if (Lenght == 5)
            {
                string crc = parse[^1] + parse[^2];
                CRC = Convert.ToUInt32(crc, 16);
                Function = Convert.ToByte(parse[3], 16);
                string[] data = parse[4..(parse.Length - 2)];
                Data = new List<byte>();
                for (int i = 0; i < data.Length; i++)
                {
                    Data.Add(Convert.ToByte(data[i], 16));
                }

                if (exceptions.Exception.ContainsKey(Function))
                {
                    Exception = exceptions.Exception[Function];
                }
                else
                {
                    Exception = "неизвестная ошибка";
                }
            }
            else if (Lenght >= 4)
            {
                Raw_data = String.Join(" ", parse[4..(parse.Length - 2)]);
                string crc = parse[^1] + parse[^2];
                CRC = Convert.ToUInt32(crc, 16);
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
        [XmlIgnore]
        public bool error_crc { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string crc { get; set; }
    }
}

