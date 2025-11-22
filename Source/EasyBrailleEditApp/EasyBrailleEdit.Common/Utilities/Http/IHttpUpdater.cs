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

    /// <summary>
    /// Event arguments for HTTP updater file events.
    /// </summary>
    public class HttpUpdaterFileEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the file number in the update sequence.
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// Gets the total number of files to be updated.
        /// </summary>
        public int Total { get; private set; }

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

    /// <summary>
    /// Specifies the action to perform during an update.
    /// </summary>
    public enum UpdateAction
    {
        /// <summary>No action.</summary>
        None,
        /// <summary>Overwrite the file.</summary>
        Overwrite,
        /// <summary>Delete the file.</summary>
        Delete
    }

    /// <summary>
    /// 更新項目。
    /// </summary>
    /// <summary>
    /// Represents an item to be updated.
    /// </summary>
    public class UpdateItem
    {
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; } = null;

        /// <summary>
        /// Gets or sets the update operation.
        /// </summary>
        public UpdateAction Operation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateItem"/> class.
        /// </summary>
        public UpdateItem()
        {
            Operation = UpdateAction.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateItem"/> class with specified parameters.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <param name="updAction">The update action.</param>
        public UpdateItem(string filename, UpdateAction updAction)
        {
            FileName = filename;
            Operation = updAction;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            UpdateItem item = obj as UpdateItem;

            if (item == null)
                return false;

            return item.FileName.Equals(this.FileName, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Specifies the rollback action.
    /// </summary>
    public enum RollbackAction
    {
        /// <summary>Rename the file.</summary>
        Rename
    }

    /// <summary>
    /// 自動更新的回覆項目。其內容記錄的不是當初執行的更新動作，
    /// 而是在復原時需要執行的補償動作。
    /// </summary>
    /// <summary>
    /// Represents a rollback item for automatic updates.
    /// </summary>
    public class RollbackItem
    {
        /// <summary>
        /// Gets or sets the rollback operation.
        /// </summary>
        public RollbackAction Operation { get; set; }

        /// <summary>
        /// Gets or sets the source file name.
        /// </summary>
        public string SourceFileName { get; set; }

        /// <summary>
        /// Gets or sets the target file name.
        /// </summary>
        public string TargetFileName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RollbackItem"/> class.
        /// </summary>
        /// <param name="original">The original file name.</param>
        /// <param name="renamed">The renamed file name.</param>
        /// <param name="operation">The rollback operation.</param>
        public RollbackItem(string original, string renamed, RollbackAction operation)
        {
            SourceFileName = original;
            TargetFileName = renamed;
            Operation = operation;
        }
    }
}