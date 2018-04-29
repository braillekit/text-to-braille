using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using EasyBrailleEdit.Common;
using Huanlin.Common.Helpers;
using Huanlin.Windows.Forms;

namespace EasyBrailleEdit
{
    public partial class ConversionDialog : Form
    {
        private ArrayList m_SelectedPhraseFileNames;
        private const char PhraseFileNameSeparator = '|';

        public ConversionDialog()
        {
            m_SelectedPhraseFileNames = new ArrayList();

            InitializeComponent();
        }

        private void ConvertionDialog_Load(object sender, EventArgs e)
        {
            numLinesPerPage.Value = AppGlobals.Config.Braille.LinesPerPage;
            numCellsPerLine.Value = AppGlobals.Config.Braille.CellsPerLine;

            List<KeyValuePair<string, string>> fileUsages = StrHelper.SplitToKeyValuePairs(
                AppGlobals.Config.PhraseFiles, PhraseFileNameSeparator, '=');

            clbPhraseTbl.Items.Clear();
            AddPhraseFiles(fileUsages);

            if (clbPhraseTbl.Items.Count > 0)
            {
                clbPhraseTbl.SelectedIndex = 0;
            }

            EnableButtons();            
        }

        private void AddPhraseFiles(List<KeyValuePair<string, string>> fileUsages)
        {
            string fname;
            int index;
            bool used = false;

            foreach (KeyValuePair<string, string> pair in fileUsages)
            {
                fname = pair.Key.ToLower();
                if (String.IsNullOrEmpty(fname))
                    continue;
                if (!File.Exists(fname))	// 檔案如果不存在，就不加入
                {
                    MsgBoxHelper.ShowWarning("詞庫檔案不存在: " + fname + Environment.NewLine + "已自動將此檔案排除!");
                    continue;
                }
                used = pair.Value.Equals("1");
                index = clbPhraseTbl.Items.IndexOf(fname);
                if (index < 0)  // 清單中沒有的項目才加入。
                {
                    clbPhraseTbl.Items.Add(fname, used);
                }
                else
                {
                    clbPhraseTbl.SetItemChecked(index, used);   // 已存在的項目只更改其勾選狀態。
                }
            }
        }

        private void AddPhraseFiles(string[] fileNames)
        {
            string fname;
            foreach (string s in fileNames)
            {
                fname = s.Trim().ToLower();
                if (String.IsNullOrEmpty(fname))
                    continue;
                if (!File.Exists(fname))    // 檔案如果不存在，就不加入
                    continue;
                if (clbPhraseTbl.Items.IndexOf(fname) < 0)  // 清單中沒有的檔案才加入。
                {
                    clbPhraseTbl.Items.Add(fname, true);
                }
            }
        }

        private void ConvertionDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = false;

            if (chkRemember.Checked)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < clbPhraseTbl.Items.Count; i++)
                {
                    sb.Append(clbPhraseTbl.Items[i]);
                    sb.Append("=");
                    if (clbPhraseTbl.GetItemChecked(i))
                        sb.Append("1");
                    else
                        sb.Append("0");
                    sb.Append(PhraseFileNameSeparator);
                }
                if (sb.Length > 0 && sb[sb.Length - 1] == PhraseFileNameSeparator)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                AppGlobals.Config.PhraseFiles = sb.ToString();

                AppGlobals.Config.Braille.LinesPerPage = (int) numLinesPerPage.Value;
                AppGlobals.Config.Braille.CellsPerLine = (int) numCellsPerLine.Value;                
            }
        }

        private void EnableButtons()
        {
            if (clbPhraseTbl.SelectedIndex >= 0)
            {
                btnRemovePhraseTbl.Enabled = true;
                btnMoveUp.Enabled = true;
                btnMoveDown.Enabled = true;
            }
            else
            {
                btnRemovePhraseTbl.Enabled = false;
                btnMoveUp.Enabled = false;
                btnMoveDown.Enabled = false;
            }
        }

        private void clbPhraseTbl_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void btnAddPhraseTbl_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                AddPhraseFiles(openFileDialog1.FileNames);
            }
        }

        private void btnRemovePhraseTbl_Click(object sender, EventArgs e)
        {
            if (clbPhraseTbl.SelectedIndex < 0)
            {
                MsgBoxHelper.ShowInfo("請先選擇欲移除的詞庫檔案!");
                return;
            }

            clbPhraseTbl.Items.RemoveAt(clbPhraseTbl.SelectedIndex);
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (clbPhraseTbl.SelectedIndex < 1)
                return;

            string s = clbPhraseTbl.Items[clbPhraseTbl.SelectedIndex].ToString();
            clbPhraseTbl.Items.RemoveAt(clbPhraseTbl.SelectedIndex);
            clbPhraseTbl.Items.Insert(clbPhraseTbl.SelectedIndex - 1, s);
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (clbPhraseTbl.SelectedIndex >= clbPhraseTbl.Items.Count-1)
                return;

            string s = clbPhraseTbl.Items[clbPhraseTbl.SelectedIndex].ToString();
            clbPhraseTbl.Items.RemoveAt(clbPhraseTbl.SelectedIndex);
            clbPhraseTbl.Items.Insert(clbPhraseTbl.SelectedIndex + 1, s);
        }

        /// <summary>
        /// 傳回使用者勾選的詞庫檔名清單。
        /// </summary>
        public string[] SelectedPhraseFileNames
        {
            get
            {
                m_SelectedPhraseFileNames.Clear();
                for (int i = 0; i < clbPhraseTbl.Items.Count; i++)
                {
                    if (clbPhraseTbl.GetItemChecked(i)) 
                    {
                        m_SelectedPhraseFileNames.Add(clbPhraseTbl.Items[i].ToString());
                    }
                }
                return (string[]) m_SelectedPhraseFileNames.ToArray(typeof(string));
            }
        }
    }
}