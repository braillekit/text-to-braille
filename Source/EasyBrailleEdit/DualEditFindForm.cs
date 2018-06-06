using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BrailleToolkit;
using Huanlin.Windows.Forms;

namespace EasyBrailleEdit
{
	public partial class DualEditFindForm : Form
	{
		private BrailleDocument m_BrDoc;
		private bool m_IsFirstTime;		// 是否是第一次尋找（以分辨是否為找下一筆）.
		private int m_StartLineIndex;
		private int m_StartWordIndex;
		private bool m_CaseSensitive;
		private int m_FoundLineIndex;
		private int m_FoundWordIndex;
		private event TargetFoundEvent m_TargetFoundEvent;
		private event DecideStartPositionEvent m_DecideStartPosEvent;

		public DualEditFindForm()
		{
			InitializeComponent();

			m_CaseSensitive = false;
		}

		public class TargetFoundEventArgs : EventArgs
		{
			public TargetFoundEventArgs(int lineIdx, int wordIdx)
			{
				LineIndex = lineIdx;
				WordIndex = wordIdx;
			}

			public readonly int LineIndex;
			public readonly int WordIndex;
		}

		public delegate void TargetFoundEvent(object sender, TargetFoundEventArgs args);

		public event TargetFoundEvent TargetFound
		{
			add { m_TargetFoundEvent += value; }
			remove { m_TargetFoundEvent -= value; }
		}

		private void OnTargetFound(TargetFoundEventArgs args)
		{
			if (m_TargetFoundEvent != null)
			{
				m_TargetFoundEvent(this, args);
			}
		}

		public class DecideStartPositionEventArgs : EventArgs
		{
			public int LineIndex;
			public int WordIndex;
		}

		public delegate void DecideStartPositionEvent(object sender, DecideStartPositionEventArgs args);

		public event DecideStartPositionEvent DecidingStartPosition
		{
			add { m_DecideStartPosEvent += value; }
			remove { m_DecideStartPosEvent -= value; }
		}

		private void OnDecidingStartPosition(DecideStartPositionEventArgs args)
		{
			if (m_DecideStartPosEvent != null)
			{
				m_DecideStartPosEvent(this, args);
			}
		}


		public BrailleDocument Document
		{
			get { return m_BrDoc; }
			set 
			{
				if (m_BrDoc != value)
				{
					m_BrDoc = value;
					m_StartLineIndex = 0;
					m_StartWordIndex = 0;
					m_FoundLineIndex = -1;
					m_FoundWordIndex = -1;
				}
			}
		}

		public int StartLineIndex
		{
			get { return m_StartLineIndex; }
			set { m_StartLineIndex = value; }
		}

		public int StartWordIndex
		{
			get { return m_StartWordIndex; }
			set { m_StartWordIndex = value; }
		}

		public int FoundLineIndex
		{
			get { return m_FoundLineIndex; }
			set { m_FoundLineIndex = value; }
		}

		public int FoundWordIndex
		{
			get { return m_FoundWordIndex; }
			set { m_FoundWordIndex = value; }
		}

		public bool IsFirstTime
		{
			get { return m_IsFirstTime; }
			set
			{
				m_IsFirstTime = value;
				if (m_IsFirstTime)
				{
					btnFind.Text = "尋找(&F)";
				}
				else
				{
					btnFind.Text = "找下一筆(&N)";
				}
			}
		}

		/// <summary>
		/// 找下一個符合的字串。
		/// </summary>
		/// <returns></returns>
		public bool FindNext()
		{
			string target = txtTarget.Text;

			DecideStartPositionEventArgs dspArgs = new DecideStartPositionEventArgs();
			OnDecidingStartPosition(dspArgs);

			int lineIdx = dspArgs.LineIndex;	// m_StartLineIndex;
			int wordIdx = dspArgs.WordIndex;	// m_StartWordIndex;

			int i;
			BrailleLine brLine;

			if (lineIdx >= m_BrDoc.LineCount)
				return false;

			brLine = m_BrDoc[lineIdx];

			if (!this.IsFirstTime)
			{
				wordIdx++;	// 若是找下一筆，則從目前位置的下一個字開始找起.
				if (wordIdx >= brLine.WordCount)
				{
					lineIdx++;
					if (lineIdx >= m_BrDoc.LineCount)
					{
						return false;
					}
					wordIdx = 0;
                    brLine = m_BrDoc[lineIdx];
                }
			}

			while (true)
			{
				if (m_CaseSensitive)
				{
					i = brLine.IndexOf(target, wordIdx, StringComparison.Ordinal);
				}
				else
				{
					i = brLine.IndexOf(target, wordIdx, StringComparison.OrdinalIgnoreCase);
				}

				if (i >= 0 && i >= wordIdx)	// 有找到?
				{
					m_FoundLineIndex = lineIdx;
					m_FoundWordIndex = i;

					// 觸發事件。
					TargetFoundEventArgs args = new TargetFoundEventArgs(m_FoundLineIndex, m_FoundWordIndex);
					OnTargetFound(args);

					// 調整起始搜尋的位置（移至下一個字），以便下次搜尋下一筆。
					m_StartLineIndex = m_FoundLineIndex;
					m_StartWordIndex = m_FoundWordIndex;
					IsFirstTime = false;
					return true;	
				}

				// 沒找到，往下一個字移動.

				wordIdx++;
				if (wordIdx + target.Length >= brLine.WordCount)
				{
					lineIdx++;
					if (lineIdx >= m_BrDoc.LineCount)
					{
						break;
					}
					brLine = m_BrDoc[lineIdx];
					wordIdx = 0;
				}
			}
			return false;
		}

		private void btnFind_Click(object sender, EventArgs e)
		{
			m_CaseSensitive = chkCaseSensitive.Checked;
			if (!FindNext())
			{
				MsgBoxHelper.ShowInfo("已搜尋至文件結尾。");
			}
		}

		private void DualEditFindForm_Load(object sender, EventArgs e)
		{
			IsFirstTime = true;
		}

		private void DualEditFindForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				this.Hide();
				Owner.Activate();
				Owner.BringToFront();
			}
		}

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}