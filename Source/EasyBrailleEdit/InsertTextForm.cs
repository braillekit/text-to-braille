using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrailleToolkit;
using BrailleToolkit.Converters;
using Huanlin.Windows.Forms;

namespace EasyBrailleEdit
{
    public partial class InsertTextForm: Form
    {
        private TextToBrailleConverter _converter = new TextToBrailleConverter();
        private bool _isUsedForPageTitle;

        public bool IsUsedForPageTitle
        {
            get => _isUsedForPageTitle;
            set
            {
                _isUsedForPageTitle = value;
                btnSymbolDocTitle.Visible = !value;
            }
        }

        public BrailleLine OutputLine { get; private set; }

        public InsertTextForm()
        {
            InitializeComponent();
        }

        private void InsertTextForm_Load(object sender, EventArgs e)
        {
            lblErrorTitle.Visible = false;
            lblError.Visible = false;
            lblError.Text = "";
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            DoConvert();
        }


        private void DoConvert()
        {         
            var brLine = _converter.Convert(txtInput.Text);
            UpdateUI(brLine);

            OutputLine = brLine;
        }

        private void UpdateUI(BrailleLine brLine)
        {
            try
            {
                lblError.Text = "";

                txtBraille.Text = BrailleFontConverter.ToString(brLine);

                lblErrorTitle.Visible = _converter.HasError;
                lblError.Visible = _converter.HasError;
                lblError.Text = _converter.GetInvalidChars();
                if (String.IsNullOrWhiteSpace(lblError.Text))
                {
                    lblError.Text = _converter.Processor.ErrorMessage;
                }
            }
            finally
            {
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtInput.Text))
            {
                MsgBoxHelper.ShowInfo("您沒有輸入任何文字!");
                return;
            }

            if (OutputLine == null || OutputLine.IsEmpty())
            {
                MsgBoxHelper.ShowInfo("目前沒有轉出任何點字！");
                return;
            }
            DialogResult = DialogResult.OK;
        }


        private void InsertSymbol(ToolStripItem item)
        {
            // 插入符號
            if (item.Tag == null)
            {
                txtInput.SelectedText = item.Text;
                return;
            }
            string s = item.Tag.ToString();
            if (String.IsNullOrEmpty(s))
            {
                txtInput.SelectedText = s;
                return;
            }

            if (s == "NA")
            {
                return;
            }

            // 處理換行符號
            s = s.Replace(@"\n", "\n");

            int i = s.IndexOf('|');
            if (i >= 0)
            {
                int selectionLength = txtInput.SelectedText.Length;
                var leftPart = s.Substring(0, i);
                var rightPart = s.Substring(i + 1);
                txtInput.SelectedText = leftPart + txtInput.SelectedText + rightPart;
                txtInput.Update();
                Application.DoEvents();

                if (selectionLength < 1)
                {
                    txtInput.SelectionStart -= (s.Length - i - 1);
                }                
            }
            else
            {
                txtInput.SelectedText = s;
            }
        }

        private void toolBarSymbol1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            InsertSymbol(e.ClickedItem);
        }

        private void InsertTextForm_Shown(object sender, EventArgs e)
        {
            txtInput.Focus();
        }

        private void toolBarMath_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            InsertSymbol(e.ClickedItem);
        }
    }
}
