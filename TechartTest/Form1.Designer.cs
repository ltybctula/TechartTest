
namespace TechartTest
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.buttonOpenFile = new System.Windows.Forms.Button();
            this.buttonSaveFile = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "log files (*.log)|*.log|All files (*.*)|*.*";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xml";
            this.saveFileDialog1.Filter = "XML files(*.xml)|*.xml|JSON files(*.json)|*.json|Text files(*.txt)|*.txt";
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.Location = new System.Drawing.Point(12, 12);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(107, 23);
            this.buttonOpenFile.TabIndex = 0;
            this.buttonOpenFile.Text = "Открыть файл";
            this.buttonOpenFile.UseVisualStyleBackColor = true;
            this.buttonOpenFile.Click += new System.EventHandler(this.ButtonOpenFile_Click);
            // 
            // buttonSaveFile
            // 
            this.buttonSaveFile.Location = new System.Drawing.Point(12, 42);
            this.buttonSaveFile.Name = "buttonSaveFile";
            this.buttonSaveFile.Size = new System.Drawing.Size(107, 23);
            this.buttonSaveFile.TabIndex = 1;
            this.buttonSaveFile.Text = "Сохранить файл";
            this.buttonSaveFile.UseVisualStyleBackColor = true;
            this.buttonSaveFile.Click += new System.EventHandler(this.ButtonSaveFile_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(125, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1022, 827);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1159, 851);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.buttonSaveFile);
            this.Controls.Add(this.buttonOpenFile);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button buttonOpenFile;
        private System.Windows.Forms.Button buttonSaveFile;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}

