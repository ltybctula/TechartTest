using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TechartTest
{
    public class Data_ref
    {
        public string SourceType { get; set; }
        public List<DataSource_ref> Source { get; set; }
        public Data_ref(string _logFile)
        {
            Source = new List<DataSource_ref>();
            string line;

            StreamReader file = new StreamReader(File.OpenRead(_logFile));
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                Source.Add(new DataSource_ref(line.Split("\t")));
            }
            file.Close();
        }
    }

    public class DataSource_ref
    {
        public string Address { get; set; }
        public string Speed { get; set; }
        public List<DataSourceLine_ref> Line { get; set; }
        public DataSource_ref(string[] _line)
        {
            Line = new List<DataSourceLine_ref>();
        }
    }

    public class DataSourceLine_ref
    {
        public string RawFrame { get; set; }
        public string RawData { get; set; }
        public string Direction { get; set; }
        public byte Address { get; set; }
        public byte Command { get; set; }
        public string Exception { get; set; }
        public string Error { get; set; }
        public bool ErrorCRC { get; set; }
        public uint CRC { get; set; }

        public DataSourceLine_ref()
        {

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
