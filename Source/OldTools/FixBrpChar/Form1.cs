using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FixBrpChar
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                lbxFiles.Items.AddRange(openFileDialog1.FileNames);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lbxFiles.Items.Clear();
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            foreach (string fname in lbxFiles.Items)
            {
                DoConvert(fname);
            }
        }

        /// <summary>
        ///       1.全部的英文改成小寫(  A B C D  改成  a b c d)。
        ///       2.  ^  符號改成 ~ 符號。
        ///       3.  @  符號改成 ` 符號。
        ///       4.  [  符號改成 { 符號。
        ///       5.  ]  符號改成 } 符號。
        ///       6.  \  符號改成 | 符號。 
        /// </summary>
        /// <param name="inFileName"></param>
        void DoConvert(string inFileName)
        {
            Encoding enc = Encoding.GetEncoding("BIG5");
            string outFileName = Path.ChangeExtension(inFileName, ".BRL");
            string content = File.ReadAllText(inFileName, enc);

            string oldChars = @"^@[]\"; 
            string newChars = @"~`{}|";

            content = content.ToLower();
            for (int i = 0; i < oldChars.Length; i++) 
            {
                content = content.Replace(oldChars[i], newChars[i]);
            }            

            File.WriteAllText(outFileName, content, enc);
        }
    }
}
