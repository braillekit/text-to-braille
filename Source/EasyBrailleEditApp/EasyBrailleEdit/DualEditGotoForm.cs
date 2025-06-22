using System.ComponentModel;
using System.Windows.Forms;

namespace EasyBrailleEdit
{
    public partial class DualEditGotoForm : Form
    {
        public DualEditGotoForm()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsGotoLine
        {
            get { return rdoLine.Checked; }
            set { rdoLine.Checked = value; }
        }

        public int Position
        {
            get { return (int)numPosition.Value; }
        }

        private void DualEditGotoForm_Shown(object sender, System.EventArgs e)
        {
            numPosition.Select(0, 255);
        }
    }
}