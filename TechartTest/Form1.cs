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
                string extension = Path.GetExtension(saveFileDialog1.FileName);
                try
                {
                    using (FileStream fileStream = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                    {
                        StreamWriter outputFile;
                        switch (extension.ToLower())
                        {
                            case ".xml":
                                XmlSerializer serializer = new XmlSerializer(typeof(Data));
                                XmlWriter writer = XmlWriter.Create(fileStream);
                                var xsn = new XmlSerializerNamespaces();
                                xsn.Add(string.Empty, string.Empty);
                                serializer.Serialize(writer, data, xsn);
                                break;

                            case ".json":
                                string json = JsonSerializer.Serialize(data);
                                outputFile = new StreamWriter(fileStream);
                                outputFile.WriteLine(json);
                                break;

                            case ".txt":
                                outputFile = new StreamWriter(fileStream);
                                outputFile.WriteLine(data.ToString());
                                break;

                            default:
                                MessageBox.Show("Извините, данный формат не поддреживается", "Warning", MessageBoxButtons.OK);
                                break;
                        }
                        fileStream.Close();
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

