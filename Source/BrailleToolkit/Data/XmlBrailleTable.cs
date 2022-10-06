using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Reflection;
using Huanlin.Common.Helpers;
using BrailleToolkit.Helpers;

namespace BrailleToolkit.Data
{
	/// <summary>
	/// �q XML �ɮ�Ū���I�r��Ӫ�A�ô��ѷj�M�\��C
	/// </summary>
	public sealed class XmlBrailleTable : BrailleTableBase
	{
		private string m_FileName;
		private bool m_Loaded;
		protected DataTable m_Table;

		public XmlBrailleTable()
		{
			m_Table = new DataTable();
			m_Table.CaseSensitive = true;	// ������ true�A�_�h���ǥb�Φr���|�M���βŸ��V�c�C
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
		/// �q XML �ɮ׸��J�I�r��Ӫ�C
		/// </summary>
		/// <param name="filename"></param>
		public override void Load(string filename)
		{
			if (String.IsNullOrEmpty(filename))
			{
				throw new ArgumentException("�ɦW�����w!");
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
        /// �q���w�ե�귽���J�I�r��Ӫ�C
        /// </summary>
        /// <param name="asmb"></param>
        /// <param name="resourceName"></param>
        public override void LoadFromResource(Assembly asmb, string resourceName)
        {
            Stream stream = asmb.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new Exception("XmlBrailleTable.LoadFromResource �䤣��귽: " + resourceName);
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
        /// �q�w�]���귽�W�١]�Y�������O�W�٥[�W .xml ���ɦW�^���J�I�r��Ӫ�C
        /// </summary>
        public virtual void LoadFromResource()
        {
            Assembly asmb = Assembly.GetExecutingAssembly();
            string resName = this.GetType().FullName + ".xml"; // Note: �o�ؼg�k�i�H�קK�g���� namsepace�A�ӥB�Ω� obfuscator �ɤ]�ॿ�`�B�@�C
            LoadFromResource(asmb, resName);
        }

        private void LoadFromStreamReader(StreamReader sr)
        {
            using (DataSet ds = new DataSet())
            {
                ds.Locale = CultureInfo.CurrentUICulture;
                ds.ReadXml(sr);
                m_Table = ds.Tables[0].Copy();
                m_Table.CaseSensitive = true;	// ������ true�A�_�h���ǥb�Φr���|�M���βŸ��V�c�C
                m_Table.PrimaryKey = new DataColumn[] { m_Table.Columns["text"] };

                ConvertDotsToCodeForTable();

                m_Loaded = true;
            }
        }

		/// <summary>
		/// �q XML �r����J�I�r��Ӫ�C
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

            ConvertDotsToCodeForTable();

			m_Loaded = true;
		}

        protected virtual void ConvertDotsToCodeForTable()
        {
            if (m_Table.Columns.IndexOf("code") < 0)
            {
                m_Table.Columns.Add(new DataColumn("code", typeof(string)));
            }

            for (int i = 0; i < m_Table.Rows.Count; i++)
            {
                var row = m_Table.Rows[i];
                var code = row["code"].ToString();
                if (string.IsNullOrWhiteSpace(code))
                {
                    string dots = row["dots"].ToString();
                    if (string.IsNullOrWhiteSpace(dots))
                    {
                        var text = row["text"].ToString();
                        throw new Exception($"�b�I�r��ƪ��A'{text}' �� code �M dots ���O�ŭȡI");
                    }

                    row["code"] = BrailleCellHelper.PositionNumbersToHexString(dots.Split(' '));
                }
            }
            m_Table.AcceptChanges();
        }

		/// <summary>
		/// �ˬd�I�r��Ӫ�O�_�w�g���J�A�Y�_�A�h��X exception�C
		/// </summary>
		/// <returns></returns>
		protected void CheckLoaded()
		{
			if (!m_Loaded)
			{
				throw new Exception("�I�r��Ӫ�|�����J���!");
			}
		}

		/// <summary>
		/// �ˬd�O�_���X�k���I�r�X�C
		/// </summary>
		/// <param name="code">�I�r�X���Q���i��r��C�Ҧp�G"A0"�C</param>
		protected void CheckCode(string code)
		{
            if (!string.IsNullOrWhiteSpace(code) && code.Length % 2 != 0)
			{
				throw new Exception("�I�r��Ӫ���Ƥ����T! code=" + code);
			}            
		}

		/// <summary>
		/// ���ޤl�C�q��r�Ÿ����o�������I�r�X�]16 �i��^�r��C
		/// </summary>
		/// <param name="text">��r�Ÿ��A�Ҧp�G�t�B�G�C</param>
		/// <returns>�I�r�X�r��A�Y�䤣��������Ÿ��A�|��X�ҥ~�C</returns>
        /// <remarks>�p�G�A�Ʊ�䤣��������I�r�X�ɤ��n��X�ҥ~�A�ӬO�Ǧ^�Ŧr��A�Шϥ� Find ��k�C</remarks>
		public override string this[string text]
		{
			get 
			{
				string brCode = Find(text);
                if (String.IsNullOrEmpty(brCode))
                {
                    throw new Exception("�䤣��������I�r�X: " + text);
                }
                return brCode;
			}
		}

        protected virtual DataRow FindDataRow(string text, string type = null)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return m_Table.Rows.Find(text); // could be null if not found.
            }

            string filter = $"text='{text}' and type='{type}'";
            DataRow[] rows = m_Table.Select(filter);
            if (rows.Length < 1)
                return null;
            return rows[0];
        }

        /// <summary>
        /// �j�M�Y�Ӥ�r�Ÿ��A�öǦ^�������I�r�X�C
        /// </summary>
        /// <param name="text">���j�M���Ÿ��C</param>
        /// <param name="type">���w���j�M���Ÿ������C�Y���P�������s�b�ۦP����r�Ÿ��A�K�����w���ѼơA�H�T�O��쥿�T���Ÿ��C</param>
        /// <returns>�Y�����A�h�Ǧ^�������I�r�X�A�_�h�Ǧ^ null�C</returns>
        /// <remarks>�p�G�A�Ʊ�䤣��������I�r�X�ɥ�X�ҥ~�A�Шϥί��ޤl�C</remarks>
        public override string Find(string text, string type=null)
		{
			CheckLoaded();

            DataRow row = FindDataRow(text, type);
			if (row == null)
			{
                return null;
			}
			string code = row["code"].ToString();

            CheckCode(code);
			return code;
		}

        public override string[] GetDots(string text, string type = null)
        {
            CheckLoaded();

            DataRow row = FindDataRow(text, type);
            if (row == null)
                return null;
                    
            return row["dots"].ToString().Split(' ');
        }

        public override bool Exists(string text)
        {
            CheckLoaded();

            DataRow row = m_Table.Rows.Find(text);
            return row != null;
        }
	}
}
