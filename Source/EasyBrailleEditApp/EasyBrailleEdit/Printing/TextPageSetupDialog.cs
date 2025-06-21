using EasyBrailleEdit.Common;
using Huanlin.Windows.Forms;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace EasyBrailleEdit
{
    /// <summary>
    /// 明眼字紙張設定對話窗。
    /// </summary>
    public partial class TextPageSetupDialog : Form
    {
        private PrintDocument m_PrnDoc;

        private TextPageSetupDialog()
        {
            InitializeComponent();
        }

		public TextPageSetupDialog(string printerName)
			: this()
		{
			m_PrnDoc = new PrintDocument();
			m_PrnDoc.PrinterSettings.PrinterName = printerName;
			this.Text = "設定列印格式 - " + printerName;

			foreach (PaperSize paperSize in m_PrnDoc.PrinterSettings.PaperSizes)
			{
				cboPaper.Items.Add(paperSize.PaperName);
			}
			cboPaper.SelectedIndex = cboPaper.Items.IndexOf(m_PrnDoc.DefaultPageSettings.PaperSize.PaperName);

			foreach (PaperSource paperSrc in m_PrnDoc.PrinterSettings.PaperSources)
			{
				cboPaperSource.Items.Add(paperSrc.SourceName);
			}			
		}

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PaperSourceName
		{
			get { return cboPaperSource.Text; }
			set 
			{
				if (String.IsNullOrEmpty(value))
				{
					cboPaperSource.SelectedIndex = -1;
				}
				else
				{
					cboPaperSource.SelectedIndex = cboPaperSource.Items.IndexOf(value);
				}
			}
		}

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PaperName
		{
			get 
			{
				if (rdoProgramDefinedPaper.Checked || cboPaper.SelectedIndex < 0)
				{
					return "";	// 傳回空字串表示紙張大小由程式自動設定。
				}
				return cboPaper.Text; 
			}
			set 
			{
				if (String.IsNullOrEmpty(value))
				{
					rdoProgramDefinedPaper.Checked = true;
					cboPaper.SelectedIndex = -1;
				}
				else
				{
					rdoUserDefinedPaper.Checked = true;
					cboPaper.SelectedIndex = cboPaper.Items.IndexOf(value);
				}
			}
		}

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FontName
        {
            get { return cboFontName.Text; }
            set 
			{
				if (String.IsNullOrEmpty(value))
				{
					cboFontName.SelectedIndex = -1;
				}
				else
				{
					cboFontName.SelectedIndex = cboFontName.Items.IndexOf(value);
				}
			}
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double FontSize
        {
            get { return (double)numFontSize.Value; }
            set { numFontSize.Value = (decimal) value; }
        }

        /// <summary>
        /// 奇數頁的邊界。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Margins OddPageMargins
        {
            get 
            {
                Margins margins = new Margins();
                margins.Left = Convert.ToInt32(txtTextMarginLeft.Text);
                margins.Top = Convert.ToInt32(txtTextMarginTop.Text);
                margins.Right = Convert.ToInt32(txtTextMarginRight.Text);
                margins.Bottom = Convert.ToInt32(txtTextMarginBottom.Text);
                return margins;
            }
            set 
            {
                txtTextMarginLeft.Text = value.Left.ToString();
                txtTextMarginTop.Text = value.Top.ToString();
                txtTextMarginRight.Text = value.Right.ToString();
                txtTextMarginBottom.Text = value.Bottom.ToString();
            }
        }

        /// <summary>
        /// 偶數頁的邊界。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Margins EvenPageMargins
        {
            get 
            {
                Margins margins = new Margins();
                margins.Left = Convert.ToInt32(txtTextMarginLeftEven.Text);
                margins.Top = Convert.ToInt32(txtTextMarginTopEven.Text);
                margins.Right = Convert.ToInt32(txtTextMarginRightEven.Text);
                margins.Bottom = Convert.ToInt32(txtTextMarginBottomEven.Text);
                return margins;
            }
            set 
            {
                txtTextMarginLeftEven.Text = value.Left.ToString();
                txtTextMarginTopEven.Text = value.Top.ToString();
                txtTextMarginRightEven.Text = value.Right.ToString();
                txtTextMarginBottomEven.Text = value.Bottom.ToString();
            }
        }

        private void TextPageSetupDialog_Load(object sender, EventArgs e)
        {
            if (m_PrnDoc == null)
            {
                MsgBoxHelper.ShowError("尚未指定 PrintDocument 屬性!");
                Close();
                return;
            }

			if (cboPaperSource.SelectedIndex < 0)
			{
				cboPaperSource.SelectedIndex = cboPaperSource.Items.IndexOf(m_PrnDoc.DefaultPageSettings.PaperSource.SourceName);
			}

            // 字型
            cboFontName.Items.Clear();
            cboFontName.Items.Add("新細明體");
            cboFontName.Items.Add("細明體");
            cboFontName.Items.Add("標楷體");
            InstalledFontCollection fonts = new InstalledFontCollection();
            foreach (FontFamily ff in fonts.Families)
            {
                if (cboFontName.Items.IndexOf(ff.Name) < 0)
                    cboFontName.Items.Add(ff.Name);
            }
            cboFontName.SelectedIndex = cboFontName.Items.IndexOf(AppGlobals.Config.Printing.PrintTextFontName);
            if (cboFontName.SelectedIndex < 0)
            {
                cboFontName.SelectedIndex = 0;
            }
            numFontSize.Value = (decimal)AppGlobals.Config.Printing.PrintTextFontSize;
        }

        private void rdoUserDefinedPaper_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoUserDefinedPaper.Checked)
            {
                cboPaper.Enabled = true;
            }
            else
            {
                cboPaper.Enabled = false;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            // 紙張大小
            if (rdoUserDefinedPaper.Checked)
            {
                // 由 user 指定
				if (cboPaper.SelectedIndex < 0)
				{
					MsgBoxHelper.ShowInfo("請選擇紙張大小!");
					return;
				}
            }
            else
            {
                // 由程式指定
				//m_PrnDoc.DefaultPageSettings.PaperSize = new PaperSize("custom", 
				//    DualPrintHelper.PaperWidth, DualPrintHelper.PaperHeight);
            }

            // 紙張來源
			if (cboPaperSource.SelectedIndex < 0)
			{
				MsgBoxHelper.ShowInfo("請選擇紙張來源!");
				cboPaperSource.Focus();
				return;
			}

			DialogResult = DialogResult.OK;
			Close();
        }

        private void numFontSize_ValueChanged(object sender, EventArgs e)
        {
            // 不能自動調整! 以免把 user 設定的奇偶頁邊界弄亂。

            // 根據字型大小自動調整上邊界。
            //int[] topMargins = { 17, 16, 15, 13, 11, 10 };

            //if (numFontSize.Value >= 10 && numFontSize.Value <= 15)
            //{
            //    txtTextMarginTop.Text = topMargins[(int)numFontSize.Value - 10].ToString();
            //}
        }
    }
}