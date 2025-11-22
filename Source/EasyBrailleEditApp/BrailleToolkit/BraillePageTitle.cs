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
    public sealed class BraillePageTitle : ICloneable, IComparable
    {
        [DataMember(Name = "TitleLine")]
        private BrailleLine m_TitleLine;

        [DataMember(Name = "BeginLineIndex")]
        private int m_BeginLineIndex;

        /// <summary>
        /// 取得標題列下方那一列的列物件參考。
        /// </summary>
        public BrailleLine BeginLineRef { get; private set; }

        /// <summary>
        /// 建構函式。
        /// </summary>
        public BraillePageTitle()
        {
            m_TitleLine = null;
            m_BeginLineIndex = -1;
        }

        /// <summary>
        /// 建構函式。
        /// </summary>
        /// <param name="words">標題文字。</param>
        /// <param name="beginLineIdx">起始列索引。</param>
        /// <param name="beginLine">起始列物件。</param>
        public BraillePageTitle(List<BrailleWord> words, int beginLineIdx, BrailleLine beginLine)
        {
            SetTitleLine(words, beginLineIdx, beginLine);
        }

        /// <summary>
        /// 建構函式。
        /// </summary>
        /// <param name="titleLine">標題列物件。</param>
        /// <param name="beginLineIdx">起始列索引。</param>
        /// <param name="beginLine">起始列物件。</param>
        public BraillePageTitle(BrailleLine titleLine, int beginLineIdx, BrailleLine beginLine) : this()
        {
            SetTitleLine(titleLine, beginLineIdx, beginLine);
        }

        private void SetTitleLine(List<BrailleWord> words, int beginLineIdx, BrailleLine beginLine)
        {
            TitleLine = new BrailleLine();
            TitleLine.Words.AddRange(words);
            TitleLine.Tag = beginLineIdx;

            BeginLineIndex = beginLineIdx;
            BeginLineRef = beginLine;
        }

        /// <summary>
        /// 設定標題列。
        /// </summary>
        /// <param name="titleLine">標題列物件。</param>
        /// <param name="beginLineIdx">起始列索引。</param>
        /// <param name="beginLine">起始列物件。</param>
        public void SetTitleLine(BrailleLine titleLine, int beginLineIdx, BrailleLine beginLine)
        {
            TitleLine = titleLine;
            TitleLine.Tag = beginLineIdx;
            BeginLineIndex = beginLineIdx;
            BeginLineRef = beginLine;
        }

        /// <summary>
        /// 根據頁標題下方第一行的物件參考（BeginLineRef，作為「錨點」），在文件中尋找其目前的索引位置，並更新 BeginLineIndex 屬性。
        /// </summary>
        /// <remarks>
        /// <para><strong>使用情境：</strong></para>
        /// <para>
        /// 在文件編輯過程中（插入或刪除行），BeginLineRef 所指向的 BrailleLine 物件在 Lines 集合中的索引位置可能會改變。
        /// 此方法透過「錨點」（BeginLineRef）重新定位該行在集合中的實際位置，確保 BeginLineIndex 與實際索引保持同步。
        /// </para>
        /// 
        /// <para><strong>運作原理：</strong></para>
        /// <list type="number">
        /// <item><description>檢查「錨點」(BeginLineRef) 是否存在。</description></item>
        /// <item><description>在文件的 Lines 集合中搜尋該錨點的當前索引。</description></item>
        /// <item><description>如果找到，更新 BeginLineIndex；如果找不到（可能已被刪除），返回 false。</description></item>
        /// </list>
        /// 
        /// <para><strong>注意：</strong>此方法不會修改 BeginLineRef 本身，因為 IndexOf 找到的物件就是 BeginLineRef 本身。</para>
        /// </remarks>
        /// <param name="brDoc">要從中尋找 BrailleLine 的 BrailleDocument 物件。</param>
        /// <returns>
        /// 如果成功在文件中找到起始列並更新索引，則返回 <c>true</c>；
        /// 如果起始列參考為 null 或在文件中找不到該列（可能已被刪除），則返回 <c>false</c>。
        /// </returns>
        public bool UpdateLineIndex(BrailleDocument brDoc)
        {
            if (BeginLineRef == null)
                return false;

            int idx = brDoc.Lines.IndexOf(BeginLineRef);
            if (idx < 0)
            {
                return false;
            }
            // 注意：不需要 BeginLineRef = brDoc.Lines[idx]，因為 IndexOf 找到的就是 BeginLineRef 本身
            BeginLineIndex = idx;
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

            BeginLineRef = brDoc.Lines[m_BeginLineIndex];
            return true;
        }

        /// <summary>
        /// 取得或設定標題列物件。
        /// </summary>
        public BrailleLine TitleLine
        {
            get { return m_TitleLine; }
            set { m_TitleLine = value; }
        }

        /// <summary>
        /// 取得起始列索引。
        /// </summary>
        public int BeginLineIndex
        {
            get { return m_BeginLineIndex; }
            private set
            {
                m_BeginLineIndex = value;
                if (TitleLine != null)
                {
                    TitleLine.Tag = value;
                }
            }
        }

        /// <summary>
        /// 轉回原始文字字串。
        /// </summary>
        /// <returns></returns>
        public string ToOriginalTextString()
        {
            if (TitleLine == null || TitleLine.IsEmpty())
            {
                return String.Empty;
            }

            string text = TitleLine.ToOriginalTextString();
            if (!text.StartsWith(XmlTagHelper.GetBeginTagName(ContextTagNames.Title)))
            {
                text = XmlTagHelper.EncloseWithTag(text, ContextTagNames.Title);
            }
            return text;
        }

        #region ICloneable Members

        /// <summary>
        /// 深層複製。
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var newTitle = new BraillePageTitle();
            newTitle.TitleLine = (BrailleLine)m_TitleLine.Clone();
            newTitle.BeginLineIndex = m_BeginLineIndex;
            newTitle.BeginLineRef = BeginLineRef;    // BeginLine 純粹是指標，因此不用深層複製。
            return newTitle;
        }

        #endregion

        /// <summary>
        /// 比較兩個 BraillePageTitle 物件的順序（依據 BeginLineIndex）。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            var title2 = obj as BraillePageTitle;
            if (title2 == null)
            {
                return 0;
            }
            return BeginLineIndex - title2.BeginLineIndex;
        }
    }
}
