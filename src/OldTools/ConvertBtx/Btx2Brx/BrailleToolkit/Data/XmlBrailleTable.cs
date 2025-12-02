using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Reflection;
using Huanlin.Common.Helpers;

namespace BrailleToolkit.Data
{
	/// <summary>
	/// 從 XML 檔案讀取點字對照表，並提供搜尋功能。
	/// </summary>
	internal class XmlBrailleTable : BrailleTableBase
	{
		private string m_FileName;
		private bool m_Loaded;
		protected DataTable m_Table;

		public XmlBrailleTable()
		{
			m_Table = new DataTable();
			m_Table.CaseSensitive = true;	// 必須為 true，否則有些半形字元會和全形符號混淆。
            m_Table.Locale = CultureInfo.CurrentUICulture;
			m_FileName = "";
		}

		public XmlBrailleTable(string filename)
			: this()
		{
			Load(filename);
		}

		public override void Load()
		{
			Load(m_FileName);
		}

		/// <summary>
		/// 從 XML 檔案載入點字對照表。
		/// </summary>
		/// <param name="filename"></param>
		public override void Load(string filename)
		{
			if (String.IsNullOrEmpty(filename))
			{
				throw new ArgumentException("檔名未指定!");
			}

			if (m_Loaded && (String.Compare(m_FileName, filename, true, CultureInfo.CurrentUICulture) == 0))
			{
				return;
			}

            using (StreamReader sr = new StreamReader(filename))
            {
                LoadFromStreamReader(sr);
                m_FileName = filename;
            }
		}

        /// <summary>
        /// 從指定組件資源載入點字對照表。
        /// </summary>
        /// <param name="asmb"></param>
        /// <param name="resourceName"></param>
        public override void LoadFromResource(Assembly asmb, string resourceName)
        {
            Stream stream = asmb.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new Exception("XmlBrailleTable.LoadFromResource 找不到資源: " + resourceName);
            }
            using (stream)
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    LoadFromStreamReader(sr);
                }
            }
        }

        /// <summary>
        /// 從預設的資源名稱（即物件的類別名稱加上 .xml 副檔名）載入點字對照表。
        /// </summary>
        public virtual void LoadFromResource()
        {
            Assembly asmb = Assembly.GetExecutingAssembly();
            string resName = this.GetType().FullName + ".xml"; // Note: 這種寫法可以避免寫死的 namsepace，而且用於 obfuscator 時也能正常運作。
            this.LoadFromResource(asmb, resName);
        }

        private void LoadFromStreamReader(StreamReader sr)
        {
            using (DataSet ds = new DataSet())
            {
                ds.Locale = CultureInfo.CurrentUICulture;
                ds.ReadXml(sr);
                m_Table = ds.Tables[0].Copy();
                m_Table.CaseSensitive = true;	// 必須為 true，否則有些半形字元會和全形符號混淆。
                m_Table.PrimaryKey = new DataColumn[] { m_Table.Columns["text"] };

                m_Loaded = true;
            }

            //Debug
            //for (int i = 0; i < m_Table.Columns.Count; i++)
            //{
            //    System.Diagnostics.Debug.WriteLine(m_Table.Columns[i].ColumnName);
            //}
        }

		/// <summary>
		/// 從 XML 字串載入點字對照表。
		/// </summary>
		/// <param name="xml"></param>
		public void LoadFromXmlString(string xml)
		{
			StringReader sr = new StringReader(xml);
			DataSet ds = new DataSet();
            ds.Locale = CultureInfo.CurrentUICulture;
			ds.ReadXml(sr);
			m_Table = ds.Tables[0].Copy();
			m_Table.PrimaryKey = new DataColumn[] { m_Table.Columns["text"] };
			sr.Close();

			m_Loaded = true;
		}


		/// <summary>
		/// 檢查點字對照表是否已經載入，若否，則丟出 exception。
		/// </summary>
		/// <returns></returns>
		protected void CheckLoaded()
		{
			if (!m_Loaded)
			{
				throw new Exception("點字對照表尚未載入資料!");
			}
		}

		/// <summary>
		/// 檢查是否為合法的點字碼。
		/// </summary>
		/// <param name="code">點字碼的十六進位字串。例如："A0"。</param>
		protected void CheckCode(string code)
		{
			if (String.IsNullOrEmpty(code) || code.Length % 2 != 0)
			{
				throw new Exception("點字對照表的資料不正確: " + code);
			}
		}

		/// <summary>
		/// 索引子。從文字符號取得對應的點字碼（16 進位）字串。
		/// </summary>
		/// <param name="text">文字符號，例如：ㄅ、：。</param>
		/// <returns>點字碼字串，若找不到對應的符號，會丟出例外。</returns>
        /// <remarks>如果你希望找不到對應的點字碼時不要丟出例外，而是傳回空字串，請使用 Find 方法。</remarks>
		public override string this[string text]
		{
			get 
			{
				string brCode = Find(text);
                if (String.IsNullOrEmpty(brCode))
                {
                    throw new Exception("找不到對應的點字碼: " + text);
                }
                return brCode;
			}
		}

		/// <summary>
		/// 搜尋某個文字符號，並傳回對應的點字碼。
		/// </summary>
		/// <param name="text">欲搜尋的符號。</param>
		/// <returns>若有找到，則傳回對應的點字碼，否則傳回空字串。</returns>
        /// <remarks>如果你希望找不到對應的點字碼時丟出例外，請使用索引子。</remarks>
		public override string Find(string text)
		{
			CheckLoaded();

			DataRow row = m_Table.Rows.Find(text);
			if (row == null)
			{
				return "";
			}
			string code = row["code"].ToString();
			CheckCode(code);
			return code;
		}

		protected string FindFromDataRows(DataRow[] rows, string text)
		{
			foreach (DataRow row in rows)
			{
				if (row["text"].ToString() == text)
				{
					string code = row["code"].ToString();
					CheckCode(code);
					return code;
				}
			}
			return "";
		}

	}
}
