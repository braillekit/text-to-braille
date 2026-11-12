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
	/// 列索引（ContentStartLineIndex）、列物件（ContentStartLineRef）與執行期識別碼（ContentStartLineIdentity）。
	/// ContentStartLineIndex、ContentStartLineRef 與 ContentStartLineIdentity 必須交互確認與修正，
	/// 以確保能夠取到正確的標題列。
    /// </summary>
    [Serializable]
    [DataContract]
    public sealed class BraillePageTitle : ICloneable, IComparable
    {
        [DataMember(Name = "TitleLine")]
        private BrailleLine? m_TitleLine = null!;

        [DataMember(Name = "BeginLineIndex")]
        private int m_ContentStartLineIndex;

        /// <summary>
        /// 取得標題後第一行文件內容的列物件參考（作為定位標題的錨點）。
        /// </summary>
        public BrailleLine ContentStartLineRef { get; private set; } = null!;

        /// <summary>
        /// 取得標題後第一行文件內容的執行期識別碼（作為定位標題的錨點）。
        /// </summary>
        public long ContentStartLineIdentity { get; private set; }

        /// <summary>
        /// 建構函式。
        /// </summary>
        public BraillePageTitle()
        {
            m_TitleLine = null;
            m_ContentStartLineIndex = -1;
        }

        /// <summary>
        /// 建構函式。
        /// </summary>
        /// <param name="words">標題文字。</param>
        /// <param name="beginLineIdx">起始列索引。</param>
        /// <param name="beginLine">起始列物件。</param>
        public BraillePageTitle(IReadOnlyList<BrailleWord> words, int beginLineIdx, BrailleLine beginLine)
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

        private void SetTitleLine(IReadOnlyList<BrailleWord> words, int beginLineIdx, BrailleLine beginLine)
        {
            var builder = new BrailleLineBuilder();
            builder.AddWords(words);
            builder.Tag = beginLineIdx;
            TitleLine = builder.ToBrailleLine();

            ContentStartLineIndex = beginLineIdx;
            ContentStartLineRef = beginLine;
            ContentStartLineIdentity = beginLine.Identity;
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
            ContentStartLineIndex = beginLineIdx;
            ContentStartLineRef = beginLine;
            ContentStartLineIdentity = beginLine.Identity;
        }

        /// <summary>
        /// 根據標題後第一行內容的執行期識別碼（ContentStartLineIdentity，作為「錨點」），
        /// 在文件中尋找其目前的索引位置，並更新 ContentStartLineIndex 屬性。
        /// </summary>
        /// <remarks>
        /// <para><strong>使用情境：</strong></para>
        /// <para>
        /// 在文件編輯過程中（插入或刪除行），ContentStartLineRef 所指向的 BrailleLine 物件在 Lines 集合中的索引位置可能會改變。
        /// 此方法透過「錨點」（ContentStartLineIdentity）重新定位該行在集合中的實際位置，確保 ContentStartLineIndex 與實際索引保持同步。
        /// </para>
        /// 
        /// <para><strong>運作原理：</strong></para>
        /// <list type="number">
        /// <item><description>檢查「錨點」(ContentStartLineIdentity) 是否存在。</description></item>
        /// <item><description>在文件的 Lines 集合中搜尋該錨點的當前索引。</description></item>
        /// <item><description>如果找到，更新 ContentStartLineIndex；如果找不到（可能已被刪除），返回 false。</description></item>
        /// </list>
        /// </remarks>
        /// <param name="brDoc">要從中尋找 BrailleLine 的 BrailleDocument 物件。</param>
        /// <returns>
        /// 如果成功在文件中找到起始列並更新索引，則返回 <c>true</c>；
        /// 如果起始列錨點不存在或在文件中找不到該列（可能已被刪除），則返回 <c>false</c>。
        /// </returns>
        public bool UpdateLineIndex(BrailleDocument brDoc)
        {
            if (ContentStartLineIdentity <= 0)
                return false;

            int idx = brDoc.IndexOfLine(ContentStartLineIdentity);
            if (idx < 0)
            {
                return false;
            }
            ContentStartLineIndex = idx;
            ContentStartLineRef = brDoc.Lines[idx];
            return true;
        }

        /// <summary>
        /// 根據起始列索引更新起始的 BrailleLine 物件。
        /// </summary>
        /// <param name="brDoc"></param>
        /// <returns></returns>
        public bool UpdateLineObject(BrailleDocument brDoc)
        {
            if (m_ContentStartLineIndex < 0 || m_ContentStartLineIndex >= brDoc.LineCount)
                return false;

            ContentStartLineRef = brDoc.Lines[m_ContentStartLineIndex];
            ContentStartLineIdentity = ContentStartLineRef.Identity;
            return true;
        }

        /// <summary>
        /// 依據目前的 anchor 狀態解析標題後第一行內容在文件中的索引。
        /// 優先使用 ContentStartLineIdentity 對應到目前文件中的列；若找不到，再退回既有索引。
        /// </summary>
        /// <param name="brDoc">文件物件。</param>
        /// <param name="lineIndex">解析出的列索引。</param>
        /// <returns>若能解析到有效列索引則為 true，否則為 false。</returns>
        public bool TryResolveContentStartLineIndex(BrailleDocument brDoc, out int lineIndex)
        {
            if (brDoc == null)
            {
                throw new ArgumentNullException(nameof(brDoc));
            }

            if (ContentStartLineIdentity > 0)
            {
                int idx = brDoc.IndexOfLine(ContentStartLineIdentity);
                if (idx >= 0)
                {
                    lineIndex = idx;
                    return true;
                }
            }

            if (m_ContentStartLineIndex >= 0 && m_ContentStartLineIndex < brDoc.LineCount)
            {
                lineIndex = m_ContentStartLineIndex;
                return true;
            }

            lineIndex = -1;
            return false;
        }

        /// <summary>
        /// 取得或設定標題列物件。
        /// </summary>
        public BrailleLine? TitleLine
        {
            get { return m_TitleLine; }
            set { m_TitleLine = value; }
        }

        /// <summary>
        /// 取得內容起始列索引（標題後第一行內容在文件中的位置）。
        /// </summary>
        public int ContentStartLineIndex
        {
            get { return m_ContentStartLineIndex; }
            private set
            {
                m_ContentStartLineIndex = value;
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
            if (m_TitleLine == null)
            {
                throw new InvalidOperationException("無法複製 BraillePageTitle，因為 TitleLine 為 null。");
            }
            var newTitle = new BraillePageTitle();
            newTitle.TitleLine = (BrailleLine)m_TitleLine.Clone();
            newTitle.ContentStartLineIndex = m_ContentStartLineIndex;
            newTitle.ContentStartLineRef = ContentStartLineRef;    // ContentStartLine 純粹是指標，因此不用深層複製。
            newTitle.ContentStartLineIdentity = ContentStartLineIdentity;
            return newTitle;
        }

        #endregion

        /// <summary>
        /// 比較兩個 BraillePageTitle 物件的順序（依據 ContentStartLineIndex）。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object? obj)
        {
            var title2 = obj as BraillePageTitle;
            if (title2 == null)
            {
                return 0;
            }
            return ContentStartLineIndex - title2.ContentStartLineIndex;
        }
    }
}
