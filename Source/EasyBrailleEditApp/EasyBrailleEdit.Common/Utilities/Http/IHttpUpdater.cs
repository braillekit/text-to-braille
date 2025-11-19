namespace EasyBrailleEdit.Common.Utilities.Http
{
    /*
        此介面定義了可透過 HTTP 協定來執行應用程式自動更新檔案的行為。

        屬性：

          - ClientPath : 代表用戶端應用程式的所在路徑。例如 "C:\EasyBrailleEdit"。

          - ServerUri : 遠端伺服器上面，存放新版本檔案的 HTTP 路徑。例如: "http://hostname.com/files/ebe/"。

          - ChangeLogFileName : 版本變更說明文件的檔案名稱。例如 "ChangeLog.txt"。

        方法：

          - RetrieveUpdateListAsync() : 取得可更新的檔案清單。此函式不僅會從伺服器端取得更新清單及剖析其內容，還會檢查本地端的檔案是否需要更新或刪除。

          - HasUpdates() : 傳回 True/False，代表伺服器端是否有新版本。

          - UpdateAsync() : 下載並更新。

        與進度有關的事件：

          - FileUpdating
          - FileUpdated
          - DownloadProgressChanged

        Wrtiien by Huan-Lin Tsai. 2008-07-02.
    */

    public class HttpUpdaterFileEventArgs : EventArgs
    {
        public readonly string FileName;    // 準備要下載的檔名
        public readonly int Number;     // 這是第幾個檔案
        public readonly int Total;      // 總共有幾個檔案

        public HttpUpdaterFileEventArgs(string filename, int number, int total)
        {
            FileName = filename;
            Number = number;
            Total = total;
        }
    }

    public interface IHttpUpdater : IDisposable
    {
        /// <summary>
        /// 清理上次執行更新時所留下的暫存檔。
        /// </summary>
        /// <returns></returns>
        void CleanUp();

        /// <summary>
        /// 取得更新清單。此函式不僅會從伺服器端取得更新清單及剖析其內容，
        /// 還會檢查本地端的檔案是否需要更新或刪除。處理的結果是儲存在
        /// m_UpdateItems 屬性中。
        /// </summary>
        Task GetUpdateListAsync(string updateFileName);

        bool HasUpdates();

        /// <summary>
        /// 執行線上更新。
        /// </summary>
        /// <returns>已更新的檔案數量（包含刪除的檔案）。/// </returns>
        Task<int> UpdateAsync();

        #region 屬性

        string ClientPath { get; set; }

        string ServerUri { get; set; }

        string ChangeLogFileName { get; set; }

        #endregion 屬性

        #region 事件

        event EventHandler<HttpUpdaterFileEventArgs> FileUpdating;

        event EventHandler<HttpUpdaterFileEventArgs> FileUpdated;

        event EventHandler<DownloadProgress> DownloadProgressChanged;

        #endregion 事件
    }

    public enum UpdateAction
    {
        None,
        Overwrite,
        Delete
    }

    /// <summary>
    /// 更新項目。
    /// </summary>
    public class UpdateItem
    {
        public string FileName { get; set; } = null;
        public UpdateAction Operation { get; set; }

        public UpdateItem()
        {
            Operation = UpdateAction.None;
        }

        public UpdateItem(string filename, UpdateAction updAction)
        {
            FileName = filename;
            Operation = updAction;
        }

        public override bool Equals(object obj)
        {
            UpdateItem item = obj as UpdateItem;

            if (item == null)
                return false;

            return item.FileName.Equals(this.FileName, StringComparison.CurrentCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum RollbackAction
    {
        Rename
    }

    /// <summary>
    /// 自動更新的回覆項目。其內容記錄的不是當初執行的更新動作，
    /// 而是在復原時需要執行的補償動作。
    /// </summary>
    public class RollbackItem
    {
        public RollbackAction Operation { get; set; } // 原本執行的操作

        public string SourceFileName { get; set; }

        public string TargetFileName { get; set; }

        public RollbackItem(string original, string renamed, RollbackAction operation)
        {
            SourceFileName = original;
            TargetFileName = renamed;
            Operation = operation;
        }
    }
}