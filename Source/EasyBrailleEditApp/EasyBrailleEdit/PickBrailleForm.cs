using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BrailleToolkit;
using BrailleToolkit.Converters;
using SourceGrid;

namespace EasyBrailleEdit
{
    public partial class PickBrailleForm : Form
    {
        public class CellClickEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            private PickBrailleForm m_Form;

            public CellClickEvent(PickBrailleForm form)
            {
                m_Form = form;
            }

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnClick(sender, e);

                var grid = (SourceGrid.Grid)sender.Grid;
                int row = sender.Position.Row;
                int col = sender.Position.Column;

                BrailleCellSelected(grid, row, col);
            }

            public override void OnFocusEntered(CellContext sender, EventArgs e)
            {
                base.OnFocusEntered(sender, e);

                var grid = (SourceGrid.Grid)sender.Grid;
                int row = sender.Position.Row;
                int col = sender.Position.Column;

                BrailleCellSelected(grid, row, col);
            }

            private void BrailleCellSelected(SourceGrid.Grid grid, int row, int col)
            {
                m_Form.lblBraille.Text = grid[row, col].Value.ToString();
                m_Form.lblBrailleCode.Text = grid[row, col].Tag as string;
            }
        }

        private CellClickEvent m_ClickController;

        public PickBrailleForm()
        {
            InitializeComponent();
        }

        private void PickBrailleForm_Load(object sender, EventArgs e)
        {
            m_ClickController = new CellClickEvent(this);

            brGrid.Redim(4, 16);

            int row;
            int col;
            int brCodeNum = 0;
            string brCodeStr;
            char brChar;

            for (row = 0; row < brGrid.RowsCount; row++)
            {
                for (col = 0; col < brGrid.ColumnsCount; col++)
                {
                    brCodeStr = brCodeNum.ToString("X2");
                    brChar = BrailleFontConverter.ToChar(brCodeStr);
                    brGrid[row, col] = new SourceGrid.Cells.Cell(brChar);
                    brGrid[row, col].Tag = brCodeStr;
                    brGrid[row, col].AddController(m_ClickController);

                    brCodeNum++;
                }
            }

            brGrid.AutoSizeCells();

            lblBraille.Text = String.Empty;
            lblBrailleCode.Text = String.Empty;
        }

        public string BrailleText
        {
            get { return lblBraille.Text; }
        }

        private void brGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}