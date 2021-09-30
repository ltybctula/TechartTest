using System.Xml.Serialization;

namespace TechartTest
{
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class DataSourceLine_ref
    {
        [System.Xml.Serialization.XmlElementAttribute(ElementName = "raw_frame", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RawFrame { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(ElementName = "raw_data", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RawData { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute("direction")]
        public string Direction { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute("address")]
        public string Address { get; set; }
        //public byte Address { get; set; }
        [XmlIgnore]
        public byte Function { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute("command")]
        public string Command { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute("exception")]
        public string Exception { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute("error")]
        public string Error { get; set; }
        [XmlIgnore]
        public bool ErrorCRC { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute("crc")]
        public string CRC { get; set; }
        //public uint CRC { get; set; }

        public uint CRC16(byte[] data, int data_size)
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
