using System;
using System.Text.Json;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace TechartTest
{
    public partial class Form1 : Form
    {
        Data data;

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
                try
                {
                    switch (extension.ToLower())
                    {
                        case ".xml":
                            XmlSerializer serializer = new XmlSerializer(typeof(Data));
                            using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                            {
                                XmlWriter writer = XmlWriter.Create(fs);
                                var xsn = new XmlSerializerNamespaces();
                                xsn.Add(string.Empty, string.Empty);
                                serializer.Serialize(writer, data, xsn);
                                fs.Close();
                            }
                            break;
                        case ".json":
                            string json = JsonSerializer.Serialize(data);
                            using (StreamWriter outputFile = new StreamWriter(saveFileDialog1.FileName))
                            {
                                outputFile.WriteLine(json);
                            }
                            //File.WriteAllText(saveFileDialog1.FileName, json);
                            break;
                        case ".txt":
                            using (StreamWriter outputFile = new StreamWriter(saveFileDialog1.FileName))
                            {
                                outputFile.WriteLine(data.ToString());
                            }
                            //File.WriteAllText(saveFileDialog1.FileName, Data_Ref.ToString());
                            break;
                        default:
                            MessageBox.Show("Извините, данный формат не поддреживается", "Warning", MessageBoxButtons.OK);
                            break;
                    }
                }
                catch
                {
                    MessageBox.Show("Произошла ошибка записи в файл.", "Error", MessageBoxButtons.OK);
                }
            }
        }

        private void ButtonOpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                data = new Data(openFileDialog1.FileName);
                richTextBox1.Text = "";
                richTextBox1.Text += data.ToString();
                richTextBox1.Text += new String('-', 20);
                richTextBox1.Text += "\r\n";
                richTextBox1.Text += "Файл прочитан.";
                buttonSaveFile.Enabled = true;
                saveFileDialog1.FileName = openFileDialog1.SafeFileName.Substring(0, openFileDialog1.SafeFileName.LastIndexOf('.')) + ".xml";
            }
        }
    }
}

