using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using BrailleToolkit;
using BrailleToolkit.Converters;
using Huanlin.Common.Helpers;
using Huanlin.Windows.Forms;
using NChinese.Phonetic;

namespace EasyBrailleEdit
{
    public enum EditCellMode {Edit, Insert, Display};

    /// <summary>
    /// 編輯一個中英文點字。
    /// </summary>
    public partial class EditCellForm : Form
    {
        private BrailleProcessor m_BrProcessor;
        private ChineseWordConverter m_ChtWordCvt;

        private EditCellMode m_Mode;
        private BrailleWord m_BrWord;

        private bool m_IsUpdatingUI;

        public EditCellForm()
        {
            InitializeComponent();

            Mode = EditCellMode.Edit;

            m_BrWord = new BrailleWord(String.Empty);

            // 以下動作不可移到 Form_Load 做，因為某些用到以下變數的事件會比它更早觸發。
            m_BrProcessor = BrailleProcessor.GetInstance();

            m_ChtWordCvt = m_BrProcessor.ChineseConverter;
            Debug.Assert(m_ChtWordCvt != null);
        }

        public EditCellMode Mode
        {
            get
            {
                return m_Mode;
            }
            set 
            {
                m_Mode = value;
                switch (value)
                {
                    case EditCellMode.Edit:
                        Text = "修改點字";
                        break;
                    case EditCellMode.Insert:
                        Text = "插入點字";
                        break;
                    case EditCellMode.Display:
                        Text = "點字內容";
                        break;
                }
            }
        }

        public BrailleWord BrailleWord
        {
            get { return m_BrWord; }
            set
            {
                if (value != null)
                {
                    m_BrWord.Copy(value);
                    UpdateUI();
                }
            }
        }

        private void UpdateUI()
        {
            m_IsUpdatingUI = true;
            try
            {
                txtChar.Text = m_BrWord.Text;
                cboPhCode.Items.Clear();

                if (m_BrWord.IsPolyphonic)
                {
                    string[] zhuyinCodes = ZhuyinQueryHelper.GetZhuyinSymbols(txtChar.Text, true);
                    cboPhCode.Items.AddRange(zhuyinCodes);
                }
                
                cboPhCode.SelectedIndex = cboPhCode.Items.IndexOf(m_BrWord.PhoneticCode);
                txtBraille.Text = BrailleFontConverter.ToString(m_BrWord);
            }
            finally
            {
                m_IsUpdatingUI = false;
            }
        }

        private void EditCellForm_Load(object sender, EventArgs e)
        {
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            // 如果未指定任何點字碼，則繼續編輯。
            if (txtChar.Text == String.Empty || txtBraille.Text == String.Empty)
            {
                return;
            }

            // 判斷點字碼是否有更動，若有，則設定新的點字碼。

            string fontStr = StrHelper.ToHexString(txtBraille.Text);

            if (!m_BrWord.CellList.ToString().Equals(fontStr))  // 有更動?
            {                
                m_BrWord.CellList.Clear();                

                string brCode;
                for (int i = 0; i < fontStr.Length; i += 2)
                {
                    brCode = BrailleFontConverter.ToBrailleCode(fontStr.Substring(i, 2));
                    m_BrWord.CellList.Add(brCode);
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void EditCellForm_KeyUp(object sender, KeyEventArgs e)
        {
            // 處理點字按鍵            
        }

        // 當明眼字有變動
        private void txtChar_TextChanged(object sender, EventArgs e)
        {
            if (m_IsUpdatingUI)
                return;

            if (String.IsNullOrEmpty(txtChar.Text))
            {
                cboPhCode.Items.Clear();
                cboPhCode.SelectedIndex = -1;
                txtBraille.Text = "";
                return;
            }

            string word = txtChar.Text;
            Stack<char> charStack = new Stack<char>(word);
            List<BrailleWord> brWordList = m_BrProcessor.ConvertWord(charStack);
            if (brWordList != null && brWordList.Count > 0)
            {
                // 成功轉換成點字，字元會從串流中取出
                m_BrWord.Clear();
                m_BrWord = brWordList[0];
                UpdateUI();
            }
            else
            {
                // 字元無法判斷和處理，它會留存在串流中。
                char ch = charStack.Pop();
                MsgBoxHelper.ShowError("無法處理字元: " + ch);
                txtChar.Text = "";
            }
        }

        // 當注音符號有變動
        private void cboPhCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_IsUpdatingUI || m_BrWord == null)
                return;
            if (cboPhCode.SelectedIndex < 0)
                return;

            m_BrWord.PhoneticCode = cboPhCode.Text;
            BrailleCellList cellList = m_ChtWordCvt.CreatePhoneticCellList(m_BrWord.PhoneticCode);
            m_BrWord.CellList.Assign(cellList);
            txtBraille.Text = BrailleFontConverter.ToString(cellList);
        }

        private void txtBraille_TextChanged(object sender, EventArgs e)
        {
            if (m_IsUpdatingUI)
                return;          
            // 這裡不即時轉換成點字碼，等到使用者按 OK 鈕時才轉，
            // 以減少因為一直修改而反覆釋放及配置記憶體的動作。
        }

        private void btnPickBraille_Click(object sender, EventArgs e)
        {
            PickBrailleForm form = new PickBrailleForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                int newLen = txtBraille.Text.Length - txtBraille.SelectionLength + form.BrailleText.Length;
                if (newLen > 3)
                {
                    MsgBoxHelper.ShowInfo("點字最多只能輸入三方!");
                    return;
                }

                ControlHelper.InputText(txtBraille, form.BrailleText);
            }
        }

        private void txtBraille_Validating(object sender, CancelEventArgs e)
        {
            if (txtBraille.Text.Equals(String.Empty))
            {
                errorProvider1.SetError(txtBraille, "點字必須輸入");
            }
            else
            {
                errorProvider1.SetError(txtBraille, "");
            }
        }

        private void txtChar_Validating(object sender, CancelEventArgs e)
        {
            if (txtChar.Text.Equals(String.Empty))
            {
                errorProvider1.SetError(txtChar, "明眼字必須輸入");
            }
            else
            {
                errorProvider1.SetError(txtChar, "");
            }
        }

    }
}