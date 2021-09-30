using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace TechartTest
{
    [XmlRoot("data")]
    public class Data_ref
    {
        [XmlAttributeAttribute("source_type")]
        public string SourceType { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("source", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<DataSource_ref> Source { get; set; }
        public Data_ref()
        {

        }
        public Data_ref(string _logFile)
        {
            SourceType = "COM";
            Source = new List<DataSource_ref>();
            DataSource_ref dataSource_Ref_tmp;
            string line;
            string[] lineSplit;
            try
            {
                using (StreamReader file = new StreamReader(File.OpenRead(_logFile)))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        line = line.Trim();
                        lineSplit = line.Split('\t');
                        dataSource_Ref_tmp = new DataSource_ref();

                        string[] lineRequest = lineSplit[6].Split(' ');
                        int lenghtRequest = Convert.ToInt32(lineRequest[1].TrimEnd(':'));

                        if (lenghtRequest == 1)
                        {
                            line = file.ReadLine();
                            line = line.Trim();
                            lineSplit = line.Split('\t');
                            lineSplit[6] = lineSplit[6].Replace(":", ": " + lineRequest[2]);
                        }

                        if (Source.Exists(a => a.Address.Equals(lineSplit[4])))
                        {
                            dataSource_Ref_tmp = Source.Find(a => a.Address.Equals(lineSplit[4]));
                            dataSource_Ref_tmp.Line.Add(dataSource_Ref_tmp.AddDataSourceLine(lineSplit));
                        }
                        else
                        {
                            dataSource_Ref_tmp.Address = lineSplit[4];
                            dataSource_Ref_tmp.Speed = "Unknown";
                            dataSource_Ref_tmp.Line.Add(dataSource_Ref_tmp.AddDataSourceLine(lineSplit));
                            Source.Add(dataSource_Ref_tmp);
                        }
                    }
                    file.Close();
                }
            }
            catch
            {
                MessageBox.Show("Не удалось прочитать файл логов.", "Error", MessageBoxButtons.OK);
            }
        }

        public override string ToString()
        {
            StringBuilder toString = new StringBuilder();
            toString.AppendFormat("SourceType: {0}\r\n", this.SourceType);
            foreach (DataSource_ref dataSource in this.Source)
            {
                toString.AppendFormat("\tSource: Address={0} Speed={1}\r\n", dataSource.Address, dataSource.Speed);
                foreach (DataSourceLine_ref dataSourceLine in dataSource.Line)
                {
                    if (String.IsNullOrEmpty(dataSourceLine.Error))
                    {
                        toString.AppendFormat("\t\tLine: Direction={0} Address={1}", dataSourceLine.Direction, dataSourceLine.Address.ToString("X2"));
                        if (!String.IsNullOrEmpty(dataSourceLine.Command))
                        {
                            toString.AppendFormat(" Command='{0}'", dataSourceLine.Command);
                        }
                        if (!String.IsNullOrEmpty(dataSourceLine.Exception))
                        {
                            toString.AppendFormat(" Exception='{0}'", dataSourceLine.Exception);
                        }
                        if (dataSourceLine.ErrorCRC)
                        {
                            toString.AppendFormat(" Error='Wrong CRC'");
                        }
                        if (!String.IsNullOrEmpty(dataSourceLine.CRC.ToString()))
                        {
                            toString.AppendFormat(" CRC='{0}'", dataSourceLine.CRC.ToString("X2"));
                        }
                        toString.AppendLine();
                        toString.AppendFormat("\t\t\tRawFrame: {0}\r\n", dataSourceLine.RawFrame);
                        toString.AppendFormat("\t\t\tRawData: {0}\r\n", dataSourceLine.RawData);
                    }
                    else
                    {
                        toString.AppendFormat("\t\tLine: Direction={0} Error='{1}'\r\n", dataSourceLine.Direction, dataSourceLine.Error);
                    }
                }
            }
            return toString.ToString();
        }
    }
}
