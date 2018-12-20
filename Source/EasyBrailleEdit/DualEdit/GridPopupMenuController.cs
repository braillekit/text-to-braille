using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;


namespace EasyBrailleEdit.DualEdit
{
    /// <summary>
    /// 當使用者在 Grid 儲存格上點右鍵時顯示的 popup menu 類別。
    /// </summary>
    public class GridPopupMenuController : SourceGrid.Cells.Controllers.ControllerBase
    {
        private ContextMenu m_Menu = new ContextMenu();
        private SourceGrid.CellContext m_CellContext;

        private event SourceGrid.CellContextEventHandler m_PopupMenuClick = null;

        public GridPopupMenuController()
        {
            string[] menuItemDefs =
            {
                $"從目前位置折行(Enter);{DualEditCommand.Names.BreakLine}",
                $"修改這個字(&E)...;{DualEditCommand.Names.EditWord};" + ((int)Shortcut.F4).ToString(),
                "-;",
                $"插入一串文字(&I)...;{DualEditCommand.Names.InsertText};" + ((int)Shortcut.CtrlIns).ToString(),
                $"插入一個字(&W)...;{DualEditCommand.Names.InsertWord};" + ((int)Shortcut.CtrlI).ToString(),
                $"插入於行尾(&A)...;{DualEditCommand.Names.AppendWord}",
                $"插入空方(&B);{DualEditCommand.Names.InsertBlank}",
                $"在上方插入一行(&L);{DualEditCommand.Names.InsertLine};" + ((int)Shortcut.CtrlShiftI).ToString(),
                $"在下方插入一行(&M);{DualEditCommand.Names.AddLine};" + ((int)Shortcut.CtrlShiftA).ToString(),
                $"插入表格(&T);{DualEditCommand.Names.InsertTable};" + ((int)Shortcut.CtrlShiftT).ToString(),
                "-;",
                $"刪除(&D);{DualEditCommand.Names.DeleteWord};" + ((int)Shortcut.CtrlDel).ToString(),
                $"倒退刪除(&K);{DualEditCommand.Names.BackDeleteWord}",
                $"刪除整行(&R);{DualEditCommand.Names.DeleteLine};" + ((int)Shortcut.CtrlE).ToString(),
                $"段落重整(&F);{DualEditCommand.Names.FormatParagraph};" + ((int)Shortcut.CtrlShiftF).ToString(),
                "-;",
                $"全選;{DualEditCommand.Names.SelectAll};" + ((int)Shortcut.CtrlA).ToString(),
                $"剪下;{DualEditCommand.Names.CutToClipboard};" + ((int)Shortcut.CtrlX).ToString(),
                $"複製;{DualEditCommand.Names.CopyToClipboard};" + ((int)Shortcut.CtrlC).ToString(),
                $"貼上;{DualEditCommand.Names.PasteFromClipboard};" + ((int)Shortcut.CtrlV).ToString(),
                $"貼上至行尾;{DualEditCommand.Names.PasteToEndOfLine};" + ((int)Shortcut.CtrlShiftV).ToString()
            };

            MenuItem mi;
            char[] sep = { ';' };
            EventHandler clickHandler = new EventHandler(GridPopupMenuItem_Click);

            foreach (string s in menuItemDefs)
            {
                string[] def = s.Split(sep);
                mi = new MenuItem(def[0]);
                mi.Tag = def[1];
                if (!mi.Text.Equals("-"))
                {
                    mi.Click += clickHandler;
                }
                if (def.Length > 2)
                {
                    mi.Shortcut = (Shortcut)Convert.ToInt32(def[2]);
                }
                m_Menu.MenuItems.Add(mi);
            }
        }

        /// <summary>
        /// 根據 tag 字串值尋找對應的選單項目，並令其隱藏或顯示。
        /// </summary>
        /// <param name="tag"></param>
        public void HideMenuItem(string tag)
        {
            foreach (MenuItem item in m_Menu.MenuItems)
            {
                if (tag.Equals((string)item.Tag, StringComparison.CurrentCultureIgnoreCase))
                {
                    item.Visible = false;
                }
            }
        }

        public override void OnMouseUp(SourceGrid.CellContext sender, MouseEventArgs e)
        {
            base.OnMouseUp(sender, e);

            if (e.Button == MouseButtons.Right)
            {
                m_CellContext = sender;
                m_Menu.Show(sender.Grid, new Point(e.X, e.Y));
            }
        }

        private void GridPopupMenuItem_Click(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            if (mi != null)
            {
                SourceGrid.CellContextEventArgs args = new SourceGrid.CellContextEventArgs(m_CellContext);
                Command = mi.Tag.ToString();
                OnPopupMenuClick(args);
            }
        }

        protected void OnPopupMenuClick(SourceGrid.CellContextEventArgs args)
        {
            m_PopupMenuClick?.Invoke(this, args);
        }

        public event SourceGrid.CellContextEventHandler PopupMenuClick
        {
            add
            {
                m_PopupMenuClick += value;
            }
            remove
            {
                m_PopupMenuClick -= value;
            }
        }

        public string Command { get; private set; }
    }

}
