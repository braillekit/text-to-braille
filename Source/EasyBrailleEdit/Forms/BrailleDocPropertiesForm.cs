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
    public partial class BrailleDocPropertiesForm: Form
    {
        public BrailleDocPropertiesForm()
        {
            InitializeComponent();
        }

        public int CellsPerLine
        {
            get { return Convert.ToInt32(lblCellsPerLine.Text); }
            set
            {
                lblCellsPerLine.Text = value.ToString();
            }
        }

        public int StartPageNumber
        {
            get { return (int) numStartPageNumber.Value; }
            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                numStartPageNumber.Value = value;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
