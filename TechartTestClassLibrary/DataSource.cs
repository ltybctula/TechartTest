using System;
using System.Collections.Generic;
using System.Xml.Serialization;
//using System.Text;

namespace TechartTest
{
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class DataSource
    {
        [System.Xml.Serialization.XmlAttributeAttribute("Address")]
        public string Address { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute("Speed")]
        public string Speed { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("line", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<DataSourceLine> Line { get; set; }
        public DataSource()
        {
            Line = new List<DataSourceLine>();
        }
        public DataSourceLine AddDataSourceLine(string[] lineSplit)
        {
            DataSourceLine dataSourceLine = new DataSourceLine();

            string[] lineRequest = lineSplit[6].Split(' ');
            int lenghtRequest = Convert.ToInt32(lineRequest[1].TrimEnd(':'));
            if (!lineSplit[5].Equals("TIMEOUT"))
            {
                string[] directionRequest = lineSplit[3].Split('_');
                switch (directionRequest[^1])
                {
                    case "READ":
                        dataSourceLine.Direction = "Response";
                        break;
                    case "WRITE":
                        dataSourceLine.Direction = "Request";
                        break;
                    default:
                        dataSourceLine.Direction = "Unknown";
                        break;
                }

                dataSourceLine.RawFrame = lineSplit[6].Substring(lineSplit[6].IndexOf(':') + 1).Trim();
                string[] parseRawFrame = dataSourceLine.RawFrame.Split(' ');
                //------------------------------------------------------------------------------------
                #region Request
                Exceptions exceptions = new Exceptions();
                Commands commands = new Commands();
                dataSourceLine.Address = lineRequest[2];

                if (lenghtRequest > 1)
                {
                    string crc = lineRequest[^1] + lineRequest[^2];
                    dataSourceLine.CRC = crc;
                    //------------------------------------------------------------------------------------
                    //Вычисление CRC Modbus, сравнение со считанным.
                    byte[] data = new byte[parseRawFrame.Length - 2];
                    for (int i = 0; i < parseRawFrame.Length - 2; i++)
                    {
                        data[i] = Convert.ToByte(parseRawFrame[i], 16);
                    }
                    uint calc_crc = dataSourceLine.CRC16(data, data.Length);
                    if (Convert.ToUInt32(crc, 16) != calc_crc)
                    {
                        dataSourceLine.ErrorCRC = true;
                    }
                }

                if (lenghtRequest == 5)
                {
                    dataSourceLine.Function = Convert.ToByte(lineRequest[3], 16);
                    if (exceptions.Exception.ContainsKey(dataSourceLine.Function))
                    {
                        dataSourceLine.Exception = dataSourceLine.Function.ToString("X2") + ": " + exceptions.Exception[dataSourceLine.Function];
                    }
                    else
                    {
                        dataSourceLine.Exception = dataSourceLine.Function.ToString("X2") + ": Неизвестная ошибка";
                    }
                }
                else if (lenghtRequest >= 4)
                {
                    dataSourceLine.RawData = string.Join(" ", lineRequest[4..(lineRequest.Length - 2)]);
                    dataSourceLine.Function = Convert.ToByte(lineRequest[3], 16);
                    if (commands.Command.ContainsKey(dataSourceLine.Function))
                    {
                        dataSourceLine.Command = dataSourceLine.Function.ToString("X2") + ": " + commands.Command[dataSourceLine.Function];
                    }
                    else
                    {
                        dataSourceLine.Command = dataSourceLine.Function.ToString("X2") + ": Неизвестная команда";
                    }
                }
                #endregion Request
            }
            else
            {
                dataSourceLine.Error = lineSplit[5];
            }

            return dataSourceLine;
        }
    }
}
