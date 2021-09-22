﻿using System;
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
            else if (Lenght==1)
            {
                Addr = Convert.ToByte(parse[2], 16);
            }
        }
    }

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
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string line="";
                StreamWriter file = new StreamWriter(File.OpenWrite(saveFileDialog1.FileName));
                foreach (Record record in Records)
                {
                    if (record.Request.Lenght>= 4)
                    {
                        line += "Device " + record.Device + " ";
                        line += "Request.Addr " + record.Request.Addr.ToString() + " ";
                        line += "Request.Function" + record.Request.Function.ToString() + " ";
                        line += "Request.Data" + record.Request.Data.ToString();
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
            }
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
                Records = Records.OrderBy(record => record.Device).ToList();
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