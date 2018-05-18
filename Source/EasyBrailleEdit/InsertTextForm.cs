using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyBrailleEdit
{
    public partial class InsertTextForm: Form
    {
        public InsertTextForm()
        {
            InitializeComponent();
        }

        private void InsertTextForm_Load(object sender, EventArgs e)
        {
            lblErrorCount.Text = "";
            lblErrorCount.ForeColor = label1.ForeColor;
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            DoConvert();
        }


        private void DoConvert()
        {

        }
    }
}
