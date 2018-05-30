using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit;
using EasyBrailleEdit.Common;
using EasyBrailleEdit.Forms;
using Huanlin.Windows.Forms;

namespace EasyBrailleEdit.DualEdit
{
    internal partial class BrailleGridController
    {

        public void ViewBraille()
        {
            var exporter = new BrailleDataExporter(
                _doc,
                AppGlobals.Config.Braille.LinesPerPage,
                _doc.StartPageNumber,
                true);

            int endPageNum;

            string brailleText = exporter.GetAllBrailleText(out endPageNum);


            var form = new ViewTextForm();
            form.Text = $"檢視點字內容（頁碼：{_doc.StartPageNumber}～{endPageNum}）";
            form.Content = brailleText;
            form.ShowDialog();
        }

        public void ViewText()
        {
            var form = new ViewTextForm();
            form.Text = "檢視明眼字";
            form.Content = _doc.GetAllText();
            form.ShowDialog();
        }

        /// <summary>
        /// 檢視模式：只顯示點字。
        /// </summary>
        private void ViewBrailleOnly()
        {
            MsgBoxHelper.ShowWarning("注意! 此為測試功能，若發現任何問題，請切回預設模式:\n" +
                "從主選單點選「檢視 > 模式 > 顯示全部」。");

            int row;

            _grid.SuspendLayout();
            CursorHelper.ShowWaitCursor();
            try
            {
                for (row = 1; row < _grid.RowsCount; row += 3)
                {
                    _grid.Rows.HideRow(row + 1);
                    _grid.Rows.HideRow(row + 2);
                }
            }
            finally
            {
                _grid.ResumeLayout();
                ResizeCells();
                CursorHelper.RestoreCursor();
            }
        }

        /// <summary>
        /// 檢視模式：顯示明眼字及注音。
        /// </summary>
        private void ViewTextAndZhuyin()
        {
            int row;

            _grid.SuspendLayout();
            CursorHelper.ShowWaitCursor();
            try
            {
                for (row = 1; row < _grid.RowsCount; row += 3)
                {
                    _grid.Rows.HideRow(row);
                }
            }
            finally
            {
                _grid.ResumeLayout();
                ResizeCells();
                CursorHelper.RestoreCursor();
            }
        }

        /// <summary>
        /// 檢視模式：顯示全部。
        /// </summary>
        private void ViewAll()
        {
            int row;

            _grid.SuspendLayout();
            CursorHelper.ShowWaitCursor();
            try
            {
                for (row = 1; row < _grid.RowsCount; row += 3)
                {
                    _grid.Rows.ShowRow(row);
                    _grid.Rows.ShowRow(row + 1);
                    _grid.Rows.ShowRow(row + 2);
                    _grid.Rows.AutoSizeRow(row);
                    _grid.Rows.AutoSizeRow(row + 1);
                    _grid.Rows.AutoSizeRow(row + 2);
                }
            }
            finally
            {
                _grid.ResumeLayout();
                ResizeCells();
                CursorHelper.RestoreCursor();
            }
        }
    }
}
