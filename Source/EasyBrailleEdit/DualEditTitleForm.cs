using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BrailleToolkit;
using EasyBrailleEdit.DualEdit;

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
            foreach (BraillePageTitle t in brDoc.PageTitles)
            {
                newTitle = t.Clone() as BraillePageTitle;
                Titles.Add(newTitle);

                m_TmpBrDoc.Lines.Add(newTitle.TitleLine);		// 塞進暫存文件。
            }

            EditController = new BrailleGridController(this, brGrid, m_TmpBrDoc);

            //EditController.DataChanged += DualEditControler_DataChanged;

            IsDirty = false;
        }

        void DualEditControler_DataChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        #region 屬性

        public List<BraillePageTitle> Titles { get; }

        public int CellsPerLine { get; set; }

        public bool IsDirty { get; set; }

        private BrailleGridController EditController { get; }
        public string StatusText
        {
            get => String.Empty;
            set { }
        }
        public int StatusProgress
        {
            get => 0;
            set { }
        }

        public string CurrentWordStatusText
        {
            get => String.Empty;
            set { }
        }

        public string CurrentLineStatusText
        {
            get => String.Empty;
            set { }
        }

        public string PageNumberText
        {
            get => String.Empty;
            set { }
        }

        #endregion

        private void DualEditTitleForm_Load(object sender, EventArgs e)
        {
            EditController.InitializeGrid();

            // 隱藏禁止使用的功能。
            string[] disabledCommands =
            {
                DualEditCommand.Names.InsertLine,
                DualEditCommand.Names.DeleteLine,
                DualEditCommand.Names.BreakLine,
                DualEditCommand.Names.FormatParagraph,
                DualEditCommand.Names.AddLine,
                DualEditCommand.Names.CopyToClipboard,   // 尚未加入，暫且隱藏。
                DualEditCommand.Names.PasteFromClipboard // 尚未加入，暫且隱藏。
            };

            foreach (string cmd in disabledCommands)
            {
                EditController.MenuController.HideMenuItem(cmd);
            }
           
            EditController.FillGrid();
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

    }
}