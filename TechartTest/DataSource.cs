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
    public class DataSource_ref
    {
        [System.Xml.Serialization.XmlAttributeAttribute("Address")]
        public string Address { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute("Speed")]
        public string Speed { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("line", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<DataSourceLine_ref> Line { get; set; }
        public DataSource_ref()
        {
            Line = new List<DataSourceLine_ref>();
        }
        public DataSourceLine_ref AddDataSourceLine(string[] lineSplit)
        {
            DataSourceLine_ref dataSourceLine_Ref = new DataSourceLine_ref();

            string[] lineRequest = lineSplit[6].Split(' ');
            int lenghtRequest = Convert.ToInt32(lineRequest[1].TrimEnd(':'));
            if (!lineSplit[5].Equals("TIMEOUT"))
            {
                dataSourceLine_Ref.RawFrame = lineSplit[6].Substring(lineSplit[6].IndexOf(':') + 1).Trim();
                string[] parseRawFrame = dataSourceLine_Ref.RawFrame.Split(' ');
                //------------------------------------------------------------------------------------
                #region Request
                Exceptions exceptions = new Exceptions();
                Commands commands = new Commands();
                dataSourceLine_Ref.Address = Convert.ToByte(lineRequest[2], 16);

                if (lenghtRequest > 1)
                {
                    string crc = lineRequest[^1] + lineRequest[^2];
                    dataSourceLine_Ref.CRC = Convert.ToUInt32(crc, 16);
                    //------------------------------------------------------------------------------------
                    //Вычисление CRC Modbus, сравнение со считанным.
                    byte[] data = new byte[parseRawFrame.Length - 2];
                    for (int i = 0; i < parseRawFrame.Length - 2; i++)
                    {
                        data[i] = Convert.ToByte(parseRawFrame[i], 16);
                    }
                    uint calc_crc = dataSourceLine_Ref.CRC16(data, data.Length);
                    if (dataSourceLine_Ref.CRC != calc_crc)
                    {
                        dataSourceLine_Ref.ErrorCRC = true;
                    }
                }

                if (lenghtRequest == 5)
                {
                    dataSourceLine_Ref.Function = Convert.ToByte(lineRequest[3], 16);
                    if (exceptions.Exception.ContainsKey(dataSourceLine_Ref.Function))
                    {
                        dataSourceLine_Ref.Exception = dataSourceLine_Ref.Function.ToString("X2") + ": " + exceptions.Exception[dataSourceLine_Ref.Function];
                    }
                    else
                    {
                        dataSourceLine_Ref.Exception = dataSourceLine_Ref.Function.ToString("X2") + ": Неизвестная ошибка";
                    }
                }
                else if (lenghtRequest >= 4)
                {
                    dataSourceLine_Ref.RawData = string.Join(" ", lineRequest[4..(lineRequest.Length - 2)]);
                    dataSourceLine_Ref.Function = Convert.ToByte(lineRequest[3], 16);
                    if (commands.Command.ContainsKey(dataSourceLine_Ref.Function))
                    {
                        dataSourceLine_Ref.Command = dataSourceLine_Ref.Function.ToString("X2") + ": " + commands.Command[dataSourceLine_Ref.Function];
                    }
                    else
                    {
                        dataSourceLine_Ref.Command = dataSourceLine_Ref.Function.ToString("X2") + ": Неизвестная команда";
                    }
                }
                else if (lenghtRequest == 1)
                {

                }
                #endregion Request
            }
            else
            {
                dataSourceLine_Ref.Error = lineSplit[5];
            }
            string[] directionRequest = lineSplit[3].Split('_');
            switch (directionRequest[^1])
            {
                case "READ":
                    dataSourceLine_Ref.Direction = "Response";
                    break;
                case "WRITE":
                    dataSourceLine_Ref.Direction = "Request";
                    break;
                default:
                    dataSourceLine_Ref.Direction = "Unknown";
                    break;
            }
            return dataSourceLine_Ref;
        }
    }
}
