using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BrailleToolkit;
using EasyBrailleEdit.DualEdit;
using Huanlin.Windows.Forms;
using SourceGrid;
using SourceGrid.Selection;

namespace EasyBrailleEdit
{
    public partial class DualEditTitleForm : Form, IBrailleGridForm
    {
        private BrailleDocument m_OrgBrDoc;	// 標題列所屬的 BrailleDocument 物件
        private BrailleDocument m_TmpBrDoc; // 把所有標題列都丟到這個暫時的 railleDocument 物件

        private DualEditTitleForm()
        {
            InitializeComponent();
        }
        
        public DualEditTitleForm(BrailleDocument brDoc)
            : this()
        {
            m_OrgBrDoc = brDoc;

            m_TmpBrDoc = new BrailleDocument(brDoc.Processor, brDoc.CellsPerLine);

            Titles = new List<BraillePageTitle>();

            // 複製所有標題列，並將標題列塞進暫存文件。
            BraillePageTitle newTitle = null;
            bool emptyTitleFound = false;
            foreach (BraillePageTitle t in brDoc.PageTitles)
            {
                if (t.TitleLine.CellCount > 0) // 避免塞進空的頁標題
                {
                    newTitle = t.Clone() as BraillePageTitle;
                    Titles.Add(newTitle);

                    m_TmpBrDoc.Lines.Add(newTitle.TitleLine);       // 塞進暫存文件。
                }
                else
                {
                    emptyTitleFound = true;
                }
            }

            if (emptyTitleFound)
            {
                MsgBoxHelper.ShowWarning("發現空的頁標題！程式已自動移除此空標題，請記得儲存文件。");
            }

            Controller = new BrailleGridController(this, brGrid, m_TmpBrDoc, forPageTitle: true);
        }

        private int RemoveEmptyTitles()
        {
            int deletedCount = 0;
            for (int i = Titles.Count - 1; i >= 0; i--)
            {
                if (Titles[i].TitleLine.CellCount < 1)
                {
                    Titles.RemoveAt(i);
                    deletedCount++;
                }
            }
            return deletedCount;
        }

        #region 屬性

        public List<BraillePageTitle> Titles { get; }

        private BrailleGridController Controller { get; }

        string IBrailleGridForm.StatusText
        {
            get => String.Empty;
            set
            {
                statMessage.Text = value;
            }
        }
        int IBrailleGridForm.StatusProgress
        {
            get => 0;
            set { }
        }

        string IBrailleGridForm.CurrentWordStatusText
        {
            get => String.Empty;
            set
            {
                statusLabelCurrentWord.Text = value;
            }
        }

        string IBrailleGridForm.CurrentLineStatusText
        {
            get => String.Empty;
            set
            {
                statusLabelCurrentLine.Text = value;
            }
        }

        string IBrailleGridForm.PageNumberText
        {
            get => String.Empty;
            set { }
        }

        #endregion

        private void DualEditTitleForm_Load(object sender, EventArgs e)
        {
            (this as IBrailleGridForm).StatusText = String.Empty;
            (this as IBrailleGridForm).CurrentLineStatusText = String.Empty;
            (this as IBrailleGridForm).CurrentWordStatusText = String.Empty;

            Controller.InitializeGrid();

            // 隱藏禁止使用的功能。
            string[] disabledCommands =
            {
                DualEditCommand.Names.InsertLine,
                DualEditCommand.Names.BreakLine,
                DualEditCommand.Names.FormatParagraph,
                DualEditCommand.Names.AddLine,
                DualEditCommand.Names.CopyToClipboard,    // 尚未加入，暫且隱藏。
                DualEditCommand.Names.PasteFromClipboard, // 尚未加入，暫且隱藏。
                DualEditCommand.Names.InsertTable
            };

            foreach (string cmd in disabledCommands)
            {
                Controller.MenuController.HideMenuItem(cmd);
            }

            brGrid.Selection.FocusRowEntered += GridSelection_FocusRowEntered;
            brGrid.Selection.CellGotFocus += GridSelection_CellGotFocus;

            Controller.FillGrid();
        }

        private void GridSelection_FocusRowEntered(object sender, RowEventArgs e)
        {
            var lineIdx = Controller.PositionMapper.GridRowToBrailleLineIndex(e.Row);
            var brLine = m_TmpBrDoc.Lines[lineIdx];

            

            (this as IBrailleGridForm).CurrentLineStatusText = brLine.ToOriginalTextString();
        }

        private void GridSelection_CellGotFocus(SelectionBase sender, ChangeActivePositionEventArgs e)
        {
            Controller.GridSelection_CellGotFocus(e);
        }

        /// <summary>
        /// Grid popup menu 點擊事件處裡常式。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GridMenu_Click(object sender, SourceGrid.CellContextEventArgs e)
        {
/*
            GridPopupMenuController menuCtrl = (GridPopupMenuController)sender;
            SourceGrid.CellContext cell = e.CellContext;
            SourceGrid.Grid grid = (SourceGrid.Grid)cell.Grid;
            int row = cell.Position.Row;
            int col = cell.Position.Column;

            switch (menuCtrl.Command)
            {
                case DualEditCommand.Names.EditWord:
                    EditController.EditWord(row, col);
                    break;
                case DualEditCommand.Names.InsertWord:
                    EditController.InsertWord(row, col);
                    break;
                case DualEditCommand.Names.InsertText:
//                    EditController.InsertText(row, col);
                    break;
                case DualEditCommand.Names.InsertBlank:  // 插入空方                        
                    EditController.InsertBlankCell(row, col, 1);
                    break;
                case DualEditCommand.Names.AppendWord:  // 在列尾插入空方
                    EditController.AppendWord(row, col);
                    break;
                case DualEditCommand.Names.DeleteWord:
                    EditController.DeleteCell(row, col);
                    break;
                case DualEditCommand.Names.BackDeleteWord:
                    EditController.BackspaceCell(row, col);
                    break;
            }
*/
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnAbortEdit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void DualEditTitleForm_KeyDown(object sender, KeyEventArgs e)
        {
            int row = brGrid.Selection.ActivePosition.Row;
            int col = brGrid.Selection.ActivePosition.Column;

            if (row < 0 || col < 0)
            {
                return;
            }

            if (e.Modifiers == Keys.None)
            {
                switch (e.KeyCode)
                {
                    case Keys.F4:
                        Controller.EditWord(brGrid, row, col);
                        e.Handled = true;
                        break;
                    case Keys.Back:     // 倒退刪除
                        Controller.BackspaceCell(brGrid, row, col);
                        e.Handled = true;
                        break;
                    case Keys.Left:
                        Controller.GridSelectLeftWord(row, col);
                        e.Handled = true;
                        break;
                }
            }
            else if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.I:        // Ctrl+I: 新增點字。
                        Controller.InsertWord(brGrid, row, col);
                        e.Handled = true;
                        break;
                    case Keys.Insert:    // Ctrl+Ins: 新增一串文字。
                        Controller.InsertText(brGrid, row, col);
                        e.Handled = true;
                        break;
                    case Keys.Delete:   // Ctrl+Delete: 刪除一格點字。
                        Controller.DeleteWord(brGrid, row, col);
                        RemoveEmptyTitles();                        
                        e.Handled = true;
                        break;
                    case Keys.E:        // Ctrl+E: 刪除一列。
                        Controller.DeleteLine(brGrid, row, col, true);
                        RemoveEmptyTitles();
                        e.Handled = true;
                        break;
                    case Keys.C:
                        Controller.CopyToClipboard(brGrid);
                        break;
                    case Keys.V:
                        Controller.PasteFromClipboard(brGrid, row, col);
                        break;
                }
            }
        }
    }
}
