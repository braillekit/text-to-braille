using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyBrailleEdit.Forms
{
    public partial class UndoBufferForm: Form
    {
        public UndoBufferForm()
        {
            InitializeComponent();
        }

        private void UndoBufferForm_Load(object sender, EventArgs e)
        {
            Opacity = 0.8;
            Width = 160;
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

        private void UndoBufferForm_Shown(object sender, EventArgs e)
        {
        }
    }
}
