using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using BrailleToolkit.Helpers;
using BrailleToolkit.Tags;

namespace BrailleToolkit
{
    /// <summary>
    /// 點字文件的頁標題。
	/// 此類別包含一個指向標題列的 BrailleLine 物件參考（TitleLine），以及指向標題列下方那一列的
	/// 列索引（BeginLineIndex）和列物件（BeginLine）。
	/// BeginLineIndex 和 BeginLine 必須交互確認與修正，以確保能夠取到正確的標題列。
    /// </summary>
    [Serializable]
    [DataContract]
    public class BraillePageTitle : ICloneable
    {
        [DataMember(Name = "TitleLine")]
        private BrailleLine m_TitleLine;

        [DataMember(Name = "BeginLineIndex")]
        private int m_BeginLineIndex;

        [NonSerialized]
        private BrailleLine m_BeginLine;

        private BraillePageTitle()
        {
            m_TitleLine = null;
            m_BeginLineIndex = -1;
            m_BeginLine = null;
        }

        public BraillePageTitle(BrailleDocument brDoc, int index) : this()
        {
            SetTitleLine(brDoc, index);
        }

        public void SetTitleLine(BrailleDocument brDoc, int index)
        {
            m_TitleLine = brDoc.Lines[index];
            m_TitleLine.RemoveContextTags();    // 移除所有情境標籤（這裡主要是把標題標籤拿掉）。

			m_BeginLineIndex = index + 1;   // 從下一列開始就是使用此標題。

			if (m_BeginLineIndex >= brDoc.LineCount)	// 標題列就是文件的最後一列?
			{
				//System.Diagnostics.Trace.WriteLine("BraillePageTitle.SetTitleLine: 標題列後面沒有文字內容!");
				m_BeginLineIndex = -1;
				m_BeginLine = null;
				return;
			}

            m_BeginLine = brDoc.Lines[m_BeginLineIndex];
        }

        /// <summary>
        /// 更新頁標題的起始列索引。
        /// </summary>
        /// <param name="brDoc"></param>
        /// <returns></returns>
        public bool UpdateLineIndex(BrailleDocument brDoc)
        {
            if (m_BeginLine == null)
                return false;

            int idx = brDoc.Lines.IndexOf(m_BeginLine);
            if (idx < 0)
            {
                return false;
            }
            m_BeginLine = brDoc.Lines[idx];
            m_BeginLineIndex = idx;
            return true;
        }

        /// <summary>
        /// 根據起始列索引更新起始的 BrailleLine 物件。
        /// </summary>
        /// <param name="brDoc"></param>
        /// <returns></returns>
        public bool UpdateLineObject(BrailleDocument brDoc)
        {
            if (m_BeginLineIndex < 0 || m_BeginLineIndex >= brDoc.LineCount)
                return false;

            m_BeginLine = brDoc.Lines[m_BeginLineIndex];
            return true;
        }
        
        public BrailleLine TitleLine
        {
            get { return m_TitleLine; }
            set { m_TitleLine = value; }
        }

        public int BeginLineIndex
        {
            get { return m_BeginLineIndex; }
        }

        public string ToOriginalTextString()
        {
            if (TitleLine == null || TitleLine.IsEmpty())
            {
                return String.Empty;
            }

            string text = TitleLine.ToOriginalTextString(null);
            return XmlTagHelper.EncloseWithTag(text, ContextTagNames.Title);
        }

		#region ICloneable Members

		/// <summary>
		/// 深層複製。
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			BraillePageTitle t = new BraillePageTitle();
			t.m_TitleLine = (BrailleLine) m_TitleLine.Clone();
			t.m_BeginLine = m_BeginLine;	// BeginLine 純粹是指標，因此不用深層複製。
			t.m_BeginLineIndex = m_BeginLineIndex;
			return t;
		}

		#endregion
	}
}
