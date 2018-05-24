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
    public partial class ViewTextForm: Form
    {
        public ViewTextForm()
        {
            InitializeComponent();
        }


        public string Content
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }

        private void ViewTextForm_Load(object sender, EventArgs e)
        {
            textBox1.SelectionStart = 0;
        }


        private void ViewTextForm_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void ViewTextForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;
            }
        }

    }
}
