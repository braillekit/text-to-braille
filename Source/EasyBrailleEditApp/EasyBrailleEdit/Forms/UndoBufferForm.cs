using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyBrailleEdit.Common;
using Huanlin.Windows.Forms;

namespace EasyBrailleEdit.Forms
{
    public partial class UndoBufferForm: Form
    {
        private int _maxBufSize;

        public UndoBufferForm()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaxBufferSize
        {
            get => _maxBufSize;
            set
            {
                if (_maxBufSize != value)
                {
                    _maxBufSize = value;
                    Text = $"最近{_maxBufSize}個修改";
                }
            }
        }

        private void UndoBufferForm_Load(object sender, EventArgs e)
        {
            Opacity = 0.8;
            Width = 170;
            Height = 200;
            if (Owner != null)
            {
                Left = Owner.Width - Width - 24;
                if (Left < 0)
                {
                    Left = 0;
                }
                Top = Owner.Top + 40;
            }

            chkAlwaysShowThis.Checked = AppGlobals.Config.BrailleEditor.ShowUndoWindow;
        }

        public void UpdateUI(List<string> operations)
        {
            lbxUndoableOperations.Items.Clear();

            foreach (var s in operations)
            {
                lbxUndoableOperations.Items.Add(s);
            }
        }

        private void UndoBufferForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                Owner.Activate();
                Owner.BringToFront();
            }
        }

        private void FocusOwnerForm()
        {
            if (Owner != null)
            {
                Owner.Activate();
                Owner.Focus();
            }
        }

        private void UndoBufferForm_Shown(object sender, EventArgs e)
        {
            FocusOwnerForm();
        }

        private void lbxUndoableOperations_Click(object sender, EventArgs e)
        {
            MsgBoxHelper.ShowInfo("按 Ctrl+Z 可復原上一次的修改操作。\r\n按 Ctrl+Y 可取消上一次的復原操作。");
            FocusOwnerForm();
        }

        private void chkAlwaysShowThis_CheckedChanged(object sender, EventArgs e)
        {
            if (AppGlobals.Config.BrailleEditor.ShowUndoWindow != chkAlwaysShowThis.Checked)
            {
                AppGlobals.Config.BrailleEditor.ShowUndoWindow = chkAlwaysShowThis.Checked;
            }            
        }
    }
}
