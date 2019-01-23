using System;
using System.Windows.Forms;
using BrailleToolkit;

namespace EasyBrailleEdit
{
    public partial class InvalidCharForm : Form
    {
        private MainForm m_MainForm;

        private InvalidCharForm()
        {
            InitializeComponent();
        }

        public InvalidCharForm(MainForm form) 
            : this()
        {
            m_MainForm = form;
        }

        public void Clear()
        {
            lbxInvalidChars.Items.Clear();
        }

        public void Add(CharPosition charPos)
        {
            var s = $"({charPos.LineNumber},{charPos.CharIndex}) : {charPos.CharValue}";
            lbxInvalidChars.Items.Add(s);
        }

        private void InvalidCharForm_Load(object sender, EventArgs e)
        {

        }

        private void InvalidCharForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void lbxInvalidChars_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxInvalidChars.Items.Count < 1 || lbxInvalidChars.SelectedIndex < 0)
                return;
            string s = lbxInvalidChars.Items[lbxInvalidChars.SelectedIndex].ToString();

            s = s.Substring(1, s.IndexOf(')') - 1);

            string[] ary = s.Split(',');
            int lineIdx = Convert.ToInt32(ary[0]) - 1;
            int charIdx = Convert.ToInt32(ary[1]) - 1;

            m_MainForm.SelectChar(lineIdx, charIdx);
            m_MainForm.Focus();
        }
    }
}