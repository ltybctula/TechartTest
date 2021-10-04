using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TechartTest
{
    public class Exceptions
    {
        public Dictionary<int, string> Exception { get; set; }
        public Exceptions()
        {
            string line;
            Exception = new Dictionary<int, string>();
            try
            {
                using (StreamReader fileExceptions = new StreamReader(File.OpenRead("exceptions.vcb")))
                {
                    while ((line = fileExceptions.ReadLine()) != null)
                    {
                        string[] parse = line.Split(" - ");
                        Exception.Add(Convert.ToByte(parse[0], 16), parse[1]);
                    }
                    fileExceptions.Close();
                }
            }
            catch
            {
                //MessageBox.Show("Не удалось прочитать файл с описание исключений.", "Error", MessageBoxButtons.OK);
                throw new Exception("Не удалось прочитать файл с описание исключений.");
            }
        }
    }
}
