using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Windows.Forms;

namespace TechartTest
{
    public partial class Form1 : Form
    {
        public Dictionary<byte, string> Exceptions = new Dictionary<byte, string>();
        public Dictionary<byte, string> Command = new Dictionary<byte, string>();
        List<Record> Records = new List<Record>();

        public Form1()
        {
            InitializeComponent();

            string line;
            StreamReader fileExceptions = new StreamReader(File.OpenRead("exceptions.vcb"));
            while ((line = fileExceptions.ReadLine()) != null)
            {
                string[] parse = line.Split(" - ");
                Exceptions.Add(Convert.ToByte(parse[0], 16), parse[1]);
            }
            fileExceptions.Close();

            StreamReader fileCommands = new StreamReader(File.OpenRead("commands.vcb"));
            while ((line = fileCommands.ReadLine()) != null)
            {
                string[] parse = line.Split(" - ");
                Command.Add(Convert.ToByte(parse[0], 16), parse[1]);
            }
            fileCommands.Close();
        }

        private void ButtonSaveFile_Click(object sender, EventArgs e)
        {
/*            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string line="";
                StreamWriter file = new StreamWriter(File.OpenWrite(saveFileDialog1.FileName));
                foreach (Record record in Records)
                {
                    if (record.Request.Lenght>= 4)
                    {
                        line += "Device " + record.Device + " ";
                        line += "Request.Addr " + record.Request.Addr.ToString() + " ";
                        line += "Request.Function " + record.Request.Function.ToString() + " is ";
                        if(Command.ContainsKey(record.Request.Function))
                        {
                            line += Command[record.Request.Function];
                        }
                        else
                        {
                            line += "Неизвестная команда";
                        }
                        line += "Request.Data ";
                        foreach (byte b in record.Request.Data)
                        {
                            line += b.ToString();
                        }
                    }
                    else if(record.Request.Lenght == 1)
                    {
                        line += "Device " + record.Device + " ";
                        line += "Request.Addr " + record.Request.Addr.ToString() + "\r\n";
                    }

                    file.WriteLine(line);
                }
                file.Close();
                Records = Records.OrderBy(record => record.Device).ToList();
            }*/
        }

        private void ButtonOpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Text = "";
                Records.Clear();
                string line, device="";
                StreamReader file = new StreamReader(File.OpenRead(openFileDialog1.FileName));
                while ((line = file.ReadLine()) != null)
                {
                    line = line.Trim();
                    Records.Add(new Record(line.Split("\t")));
                }
                file.Close();
                Records = Records.OrderBy(record => record.Address).ToList();
                StringBuilder str = new StringBuilder();
                foreach (Record record in Records)
                {
                    str.Clear();
                    if(!device.Equals(record.Address))
                    {
                        device = record.Address;
                        str.AppendFormat("SourceType:={0}; ", record.Address);
                        str.AppendLine();
                    }
                    str.AppendFormat("\tNumber={0}; ", record.Number);
                    str.AppendFormat("Result={0}; ", record.Result);
                    str.AppendFormat("Request.Addr={0}; ", record.Request.Addr.ToString("X2"));
                    if(record.Request.Lenght == 5)
                    {
                        if (Exceptions.ContainsKey(record.Request.Function))
                        {
                            str.AppendFormat("Request.Function={0} - {1}; ", record.Request.Function.ToString("X2"), Exceptions[record.Request.Function]);
                        }
                        else
                        {
                            str.AppendFormat("Request.Function={0} - неизвестная ошибка; ", record.Request.Function.ToString("X2"));
                        }

                        str.AppendFormat("Request.Data=");
                        foreach (byte b in record.Request.Data)
                        {
                            str.AppendFormat("{0}|", b.ToString("X2"));
                        }
                    }
                    else if (record.Request.Lenght >= 4)
                    {
                        if (Command.ContainsKey(record.Request.Function))
                        {
                            str.AppendFormat("Request.Function={0} - {1}; ", record.Request.Function.ToString("X2"), Command[record.Request.Function]);
                        }
                        else
                        {
                            str.AppendFormat("Request.Function={0} - неизвестная команда; ", record.Request.Function.ToString("X2"));
                        }

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
        string SourceType { get; set; }
        public List<Record> Records { get; set; }
        Source()
        {
            SourceType = "COM";
            Records = new List<Record>();
        }
    }

    public class Record
    {
        public int Number { get; set; }
        public string Time { get; set; }
        public string Source { get; set; }
        public string Address { get; set; }
        public string Speed { get; set; }
        public string Result { get; set; }
        public Request Request { get; set; }
        public Record(string[] line)
        {
            Number = Convert.ToInt32(line[0]);
            Time = line[1];
            Source = "COM";
            Address = line[4];
            Speed = "Unknown";
            Result = line[5];
            Request = new Request(line[6]);
        }
    }

    public class Request
    {
        public int Lenght { get; set; }
        public byte Addr { get; set; }
        public byte Function { get; set; }
        public string Command { get; set; }
        public List<byte> Data { get; set; }
        public uint CRC { get; set; }

        public Request(string _data)
        {
            string[] parse = _data.Split(" ");
            Lenght = Convert.ToInt32(parse[1].TrimEnd(':'));
            if (Lenght >= 4)
            {
                string crc = parse[^2] + parse[^1];
                CRC = Convert.ToUInt32(crc, 16);
                Addr = Convert.ToByte(parse[2], 16);
                Function = Convert.ToByte(parse[3], 16);

                string[] data = parse[4..(parse.Length - 2)];
                Data = new List<byte>();
                for (int i = 0; i < data.Length; i++)
                {
                    Data.Add(Convert.ToByte(data[i], 16));
                }
            }
            else if (Lenght == 1)
            {
                Addr = Convert.ToByte(parse[2], 16);
            }
        }
    }
}

