using System.Windows.Forms;

namespace EasyBrailleEdit
{
    public partial class DualEditGotoForm : Form
    {
        private int m_MaxLineNumber = 9999;
        private int m_MaxPageNumber = 999;

        public DualEditGotoForm()
        {
            InitializeComponent();
        }

        public bool IsGotoLine
        {
            get { return rdoLine.Checked; }
            set { rdoLine.Checked = value; }
        }

        public int MaxLineNumber
        {
            get { return m_MaxLineNumber; }
            set { m_MaxLineNumber = value; }
        }

        public int MaxPageNumber
        {
            get { return m_MaxPageNumber; }
            set { m_MaxPageNumber = value; }
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