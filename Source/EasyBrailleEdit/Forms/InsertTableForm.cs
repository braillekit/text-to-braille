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
    public partial class InsertTableForm : Form
    {
        public InsertTableForm()
        {
            InitializeComponent();
        }

        public int RowCount { get => (int)numRows.Value; }

        public int ColumnCount { get => (int)numColumns.Value; }

        public int CellsPerColumn { get => (int)numCellsPerColumn.Value; }

        private void InsertTableForm_Load(object sender, EventArgs e)
        {
            numRows.Value = 2;
            numRows.Minimum = 1;
            numRows.Maximum = 12;

            numColumns.Value = 2;
            numColumns.Minimum = 1;
            numColumns.Maximum = 18;

            numCellsPerColumn.Value = 1;
            numCellsPerColumn.Minimum = 1;
            numCellsPerColumn.Maximum = 38;
        }
    }
}
