using System;
using System.Text;
using System.Web;

namespace Huanlin.Common.Http
{
    public static class WebHelper
    {
/*
        /// <summary>
        /// 傳回指定的 HTTP 請求的根虛擬路徑。
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
		public static string GetRootUrl(HttpRequest req)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(req.Url.Scheme);
			sb.Append("://");
			sb.Append(req.Url.Authority);
			sb.Append(req.ApplicationPath);
			sb.Append("/");

			return sb.ToString();
		}

		/// <summary>
		/// 取得用戶端的 IP 位址。
		/// </summary>
		/// <param name="req">Request 物件。</param>
		/// <param name="detectProxy">是否偵測 proxy。</param>
		/// <returns></returns>
		public static string GetClientIPAddress(HttpRequest req, bool detectProxy)
		{
			string ip = null;

			if (detectProxy)
			{
				ip = req.ServerVariables["HTTP_X_FORWARDED_FOR"];
				if (String.IsNullOrEmpty(ip))
				{
					ip = req.ServerVariables["REMOTE_ADDR"];
				}
			}
			else
			{
				ip = req.ServerVariables["REMOTE_ADDR"];
			}

			return ip;
		}

        /// <summary>
        /// 透過 HTTP 從遠端機器下載檔案。
        /// </summary>
        /// <param name="url">遠端 URL。</param>
        /// <param name="fileName">不包含路徑的檔案名稱（用戶端下載檔案時看到的檔名）。</param>
        public static void RemoteDownloadFile(string url, string fileName)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            byte[] buffer = wc.DownloadData(url);

            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.Clear();
            string encodedFileName = HttpUtility.UrlEncode(fileName);
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + encodedFileName);
            HttpContext.Current.Response.AddHeader("Content-Length", buffer.Length.ToString());
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.BinaryWrite(buffer);
            HttpContext.Current.Response.End();
        }
*/
    }
}
