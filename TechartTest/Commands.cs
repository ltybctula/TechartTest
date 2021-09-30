using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TechartTest
{
    public class Commands
    {
        public Dictionary<int, string> Command { get; set; }
        public Commands()
        {
            string line;
            Command = new Dictionary<int, string>();
            try
            {
                using (StreamReader fileCommands = new StreamReader(File.OpenRead("commands.vcb")))
                {
                    while ((line = fileCommands.ReadLine()) != null)
                    {
                        string[] parse = line.Split(" - ");
                        Command.Add(Convert.ToByte(parse[0], 16), parse[1]);
                    }
                    fileCommands.Close();
                }
            }
            catch
            {
                MessageBox.Show("Не удалось прочитать файл с описание команд.", "Error", MessageBoxButtons.OK);
            }
        }
    }
}
