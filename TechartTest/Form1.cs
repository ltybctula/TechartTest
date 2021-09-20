using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace TechartTest
{
    public class Record
    {
        public int Number { get; set; }
        public string Time { get; set; }
        public string Device { get; set; }
        public string Result { get; set; }
        public Request Request { get; set; }
        public Record(string[] line)
        {
            Number = Convert.ToInt32(line[0]);
            Time = line[1];
            Device = line[4];
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
        /*
        184	11:26:50	AppName IRP_MJ_WRITE    Serial0 SUCCESS Length 7: FE 46 00 3B 01 CA 6C 	
        185	11:26:50	AppName IRP_MJ_READ Serial0 TIMEOUT Length 0: 	
        534	11:26:50	AppName	IRP_MJ_WRITE	Serial3 SUCCESS	Length 7: 68 46 01 6A 02 AE 20 	
        535	11:26:50	AppName	IRP_MJ_READ	Serial3	SUCCESS	Length 1: 68 	
        */
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
                //Data = new byte[data.Length];
                Data = new List<byte>();
                for (int i = 0; i < data.Length; i++)
                {
                    //Data[i] = Convert.ToByte(data[i], 16);
                    Data.Add(Convert.ToByte(data[i], 16));
                }
            }
            else if (Lenght==1)
            {
                Addr = Convert.ToByte(parse[2], 16);
            }
        }
    }

    public partial class Form1 : Form
    {
        public Dictionary<int, string> Exceptions = new Dictionary<int, string>();
        public Dictionary<int, string> Command = new Dictionary<int, string>();
        List<Record> Records = new List<Record>();

        public Form1()
        {
            InitializeComponent();

            string line;
            StreamReader fileExceptions = new StreamReader(File.OpenRead("exceptions.vcb"));
            while ((line = fileExceptions.ReadLine()) != null)
            {
                string[] parse = line.Split(" - ");
                Exceptions.Add(Convert.ToInt32(parse[0]), parse[1]);
            }
            fileExceptions.Close();

            StreamReader fileCommands = new StreamReader(File.OpenRead("commands.vcb"));
            while ((line = fileCommands.ReadLine()) != null)
            {
                string[] parse = line.Split(" - ");
                Command.Add(Convert.ToInt32(parse[0]), parse[1]);
            }
            fileCommands.Close();
        }

        private void ButtonSaveFile_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void ButtonOpenFile_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                string line;
                StreamReader file = new StreamReader(File.OpenRead(openFileDialog1.FileName));
                while ((line = file.ReadLine()) != null)
                {
                    line = line.Trim();
                    Records.Add(new Record(line.Split("\t")));
                }
                file.Close();
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
}
