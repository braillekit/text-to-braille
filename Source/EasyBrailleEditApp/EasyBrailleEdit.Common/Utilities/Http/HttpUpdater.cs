using System.Diagnostics;
using System.Net.Http.Headers;

namespace EasyBrailleEdit.Common.Utilities.Http
{
    /// <summary>
    /// 提供基於 HTTP 協定的應用程式自動更新功能，支援檔案下載失敗時的回復（rollback）機制。
    /// 若其中有一個檔案下載失敗，就會整批 rollback（把先前下載的檔案刪除）。
    /// </summary>
    /// <remarks>
    /// <para>改寫自 Eduardo Oliveira 的 AutoUpdate.vb，並增加下載進度的事件。</para>
    /// 
    /// <para><strong>公開屬性：</strong></para>
    /// <list type="bullet">
    /// <item><description><see cref="ClientPath"/> - 代表用戶端應用程式的所在路徑。例如 "C:\EasyBrailleEdit"。</description></item>
    /// <item><description><see cref="ServerUri"/> - 遠端伺服器上面，存放新版本檔案的 HTTP 路徑。例如: "http://hostname.com/files/ebe/"。</description></item>
    /// <item><description><see cref="ChangeLogFileName"/> - 版本變更說明文件的檔案名稱。例如 "ChangeLog.txt"。</description></item>
    /// </list>
    /// 
    /// <para><strong>方法：</strong></para>
    /// <list type="bullet">
    /// <item><description><see cref="GetUpdateListAsync"/> - 取得可更新的檔案清單。此函式不僅會從伺服器端取得更新清單及剖析其內容，還會檢查本地端的檔案是否需要更新或刪除。</description></item>
    /// <item><description><see cref="HasUpdates"/> - 傳回 True/False，代表伺服器端是否有新版本。</description></item>
    /// <item><description><see cref="UpdateAsync"/> - 下載並更新。</description></item>
    /// </list>
    /// 
    /// <para><strong>與進度有關的事件：</strong></para>
    /// <list type="bullet">
    /// <item><description><see cref="FileUpdating"/> - 檔案開始更新時觸發。</description></item>
    /// <item><description><see cref="FileUpdated"/> - 檔案更新完成時觸發。</description></item>
    /// <item><description><see cref="DownloadProgressChanged"/> - 下載進度變更時觸發。</description></item>
    /// </list>
    /// 
    /// <para>Written by Huan-Lin Tsai.</para>
    /// </remarks>
    public class HttpUpdater : IHttpUpdater
    {
        private bool m_Disposed = false;

        private const string ToDeleteExtension = ".ToDelete";
        private const string TempFileExtension = ".UpdTmp";

        private const string ErrorClientPathEmpty = "AutoUpdate.ClientPath 不可為空字串!";
        private const string ErrorDownloadingFile = "無法下載檔案: \r\n";
        private const string ErrorMessageUpdate = "自動更新過程發生錯誤.";
        private const string ErrorMessageDelete = "刪除檔案時發生錯誤.";
        private const string ErrorServerUriEmpty = "尚未指定 ServerUri 屬性!";
        private const string ReadyToRollback = "更新檔案過程發生錯誤，準備嘗試回復檔案。";

        private static HttpClient _httpClient;

        private string m_ClientPath;
        private string m_ServerUri = null!;
        private string m_ChangeLogFileName;		// 應用程式的變更記錄檔名，若有指定，在進行更新時會自動下載此檔案.
        private List<UpdateItem> m_UpdateItems;
        private readonly object _lockObject = new object();

        static HttpUpdater()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpUpdater"/> class.
        /// </summary>
        public HttpUpdater()
        {
            m_ClientPath = "";
            m_ChangeLogFileName = "";

            m_UpdateItems = new List<UpdateItem>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        /// <summary>
        /// Releases the unmanaged resources used by the HttpUpdater and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!m_Disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    FileUpdating = null;
                    FileUpdated = null;
                    DownloadProgressChanged = null;

                    m_UpdateItems.Clear();
                    m_UpdateItems = null;

                    m_Disposed = true;
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;

                // Note disposing has been done.
                m_Disposed = true;
            }
        }

        #region 清理

        /// <summary>
        /// 清理上次執行更新時所留下的暫存檔。
        /// </summary>
        /// <returns></returns>
        public void CleanUp()
        {
            CheckClientPath();

            string file;
            DirectoryInfo dir = new DirectoryInfo(m_ClientPath);

            FileInfo[] infos = dir.GetFiles("*" + ToDeleteExtension, SearchOption.AllDirectories);

            foreach (FileInfo info in infos)
            {
                file = info.FullName;
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            // 刪除暫存檔
            infos = dir.GetFiles("*" + TempFileExtension, SearchOption.AllDirectories);
            foreach (FileInfo info in infos)
            {
                file = info.FullName;
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
        }

        #endregion

        private void CheckServerUrl()
        {
            if (String.IsNullOrEmpty(m_ServerUri))
            {
                throw new Exception(ErrorServerUriEmpty);
            }
        }

        private void CheckClientPath()
        {
            if (String.IsNullOrEmpty(m_ClientPath))
            {
                throw new Exception(ErrorClientPathEmpty);
            }
        }


        private IEnumerable<string> StripComments(IEnumerable<string> lines)
        {
            // Parse the file list to strip comments
            return lines.Select(line =>
            {
                int i = line.IndexOf('\'');
                if (i >= 0) // 去掉註解（以單引號開始）
                {
                    return line.Substring(0, i).Trim();
                }
                return line.Trim();
            }).Where(line => !string.IsNullOrEmpty(line));
        }

        private bool FileExistsAndNotEmpty(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            if (fi.Exists && fi.Length > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 取得更新清單。此函式不僅會從伺服器端取得更新清單及剖析其內容，
        /// 還會檢查本地端的檔案是否需要更新或刪除。處理的結果是儲存在
        /// m_UpdateItems 屬性中。
        /// </summary>
        public async Task GetUpdateListAsync(string updateFileName)
        {
            CheckClientPath();
            CheckServerUrl();

            CleanUp();

            // 取得 Update.txt 的內容並逐行讀取
            var lines = new List<string>();
            using (var stream = await _httpClient.GetStreamAsync(m_ServerUri + updateFileName))
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lines.Add(line);
                }
            }

            var fileList = StripComments(lines);

            string[] info;
            string infoFilePath;
            String infoParam;
            List<string> fileNameList = new List<string>();

            Version verRemote;  // 遠端檔案的版本
            Version verLocal;   // 本地端檔案的版本
            FileVersionInfo fileVerInfo;

            m_UpdateItems.Clear();  // 清除更新項目清單

            foreach (string file in fileList)
            {
                info = file.Split(';');
                infoFilePath = info[0].Trim();          // 從檔案清單項目中取出一個檔名（包含完整路徑）

                if (String.IsNullOrEmpty(infoFilePath))
                {
                    continue;
                }

                if (m_ChangeLogFileName.Equals(infoFilePath, StringComparison.CurrentCultureIgnoreCase))
                {
                    // 忽略變更記錄檔。
                    continue;
                }
                if (info.Length > 1)
                {
                    infoParam = info[1].Trim().ToLower();   // 從檔案清單項目中取出欲對該檔案執行的動作參數
                }
                else
                {
                    infoParam = "?";    // 沒有指定分號，給預設值：用戶端沒有該檔案時才更新
                }

                // 去掉開頭的 '.' 和兩個反斜線 '\'。
                while (infoFilePath.Length > 0 && (infoFilePath[0] == '.' || infoFilePath[0] == '\\'))
                {
                    infoFilePath = infoFilePath.Substring(1);
                }

                if (string.IsNullOrEmpty(infoFilePath))
                {
                    continue;
                }

                UpdateItem item = new UpdateItem();
                item.FileName = infoFilePath;

                // 忽略已經存在清單中（重複）的檔案
                if (m_UpdateItems.Contains(item))
                {
                    continue;
                }

                // 根據指定的更新動作和參數調整 NeedDelete 和 NeedUpdate 旗號

                string clientFileName = ClientPath + item.FileName;

                bool fileExists = File.Exists(clientFileName);  // FileExistsAndNotEmpty(clientFileName);

                item.Operation = UpdateAction.None;

                if ((infoParam == "delete"))
                {
                    // The second parameter is "delete"
                    if (fileExists)
                    {
                        item.Operation = UpdateAction.Delete;   // 刪除
                    }
                }
                else if ((infoParam == "?"))
                {
                    // The second parameter is "?" : 檢查檔案是否存在，若存在則不更新，不存在才更新
                    if (!fileExists)
                    {
                        item.Operation = UpdateAction.Overwrite;
                    }
                }
                else if (infoParam != string.Empty && (infoParam[0] == '=' && fileExists))
                {
                    // 第二個參數若以 '=' 字元開始，表示只有 local 跟遠端的檔案版本不同就要更新
                    fileVerInfo = FileVersionInfo.GetVersionInfo(clientFileName);
                    verRemote = new Version(infoParam.Substring(1, infoParam.Length - 1));
                    verLocal = new Version(fileVerInfo.FileVersion.Split(' ')[0]);

                    if (verRemote != verLocal)
                    {
                        // 只要兩個版本不同就要更新
                        item.Operation = UpdateAction.Overwrite;
                    }
                }
                else
                {
                    // infoParam 指定的是版本編號

                    if (fileExists)
                    {
                        // If 2nd parameter is empty, do nothing as file already exists
                        if (infoParam != string.Empty)
                        {
                            // Check the version of local and remote files
                            fileVerInfo = FileVersionInfo.GetVersionInfo(clientFileName);

                            if (fileVerInfo.FileVersion == null)
                            {
                                // 用戶端有這個檔案，可是沒有版本編號
                                item.Operation = UpdateAction.Overwrite;
                            }
                            else
                            {
                                // 比對兩邊的版本編號
                                verRemote = new Version(infoParam);
                                verLocal = new Version(fileVerInfo.FileVersion.Split(' ')[0]);

                                if (verRemote > verLocal)
                                {
                                    item.Operation = UpdateAction.Overwrite;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 用戶端沒有這個檔案，所以需要更新。
                        item.Operation = UpdateAction.Overwrite;
                    }
                }

                // 若有需要更新才加入更新項目清單.
                if (item.Operation != UpdateAction.None)
                {
                    m_UpdateItems.Add(item);
                    // Logger.Debug("找到需要更新的檔案： {@Item}", item);
                }
            }
            //Logger.Debug($"完成獲取更新檔案清單，共 {m_UpdateItems.Count} 個更新項目。");
        }

        /// <summary>
        /// Determines whether there are updates available.
        /// </summary>
        /// <returns>True if updates are available; otherwise, false.</returns>
        public bool HasUpdates()
        {
            return (m_UpdateItems.Count > 0);
        }

        /// <summary>
        /// 執行線上更新。
        /// </summary>
        /// <returns>已更新的檔案數量（包含刪除的檔案）。/// </returns>
        public async Task<int> UpdateAsync()
        {
            if (!HasUpdates())
            {
                return 0;
            }

            CleanUp();

            // 加入預設的變更記錄檔名.
            if (!String.IsNullOrEmpty(m_ChangeLogFileName))
            {
                UpdateItem updItem = new UpdateItem(m_ChangeLogFileName, UpdateAction.Overwrite);
                if (!m_UpdateItems.Contains(updItem))
                {
                    m_UpdateItems.Add(updItem);
                }
            }

            int updCount = 0;

            // execute the following line even for check runs
            List<RollbackItem> rollBackList = new List<RollbackItem>();

            var progress = new Progress<DownloadProgress>(p =>
            {
                DownloadProgressChanged?.Invoke(this, p);
            });

            try
            {
                foreach (UpdateItem item in m_UpdateItems)
                {
                    var serverFileUrl = ServerUri + item.FileName;
                    var clientFileName = ClientPath + item.FileName;
                    var tempFileName = clientFileName + DateTime.Now.Ticks.ToString() + TempFileExtension;
                    var toDeleteFileName = tempFileName + ToDeleteExtension;


                    // 確保子目錄存在
                    var actualClientPath = Path.GetDirectoryName(clientFileName);
                    if (!Directory.Exists(actualClientPath))
                    {
                        Directory.CreateDirectory(actualClientPath);
                    }

                    // 開始執行更新作業
                    var updEvtArgs = new HttpUpdaterFileEventArgs(item.FileName, updCount + 1, m_UpdateItems.Count);
                    OnFileUpdating(updEvtArgs);

                    RollbackItem rollBackItem;
                    switch (item.Operation)
                    {
                        case UpdateAction.Overwrite:
                            // Logger.Debug($"正在下載檔案，來源: {serverFileUrl}，目的： {tempFileName}");

                            // 1.下載檔案並存成暫時檔名
                            await HttpDownloader.DownloadFileAsync(serverFileUrl, tempFileName, progress);

                            if (!FileExistsAndNotEmpty(tempFileName))	// 檢查檔案下載是否成功
                            {
                                throw new Exception(ErrorDownloadingFile + item.FileName);
                                // note: 錯誤訊息中不要顯示完整檔案路徑，以免使用者看到完整的下載路徑。
                            }

                            // 2.將欲覆蓋的原有檔案更名為待刪除檔案
                            if (File.Exists(clientFileName))	// 再檢查一次檔案是否存在，以確保不會發生錯誤
                            {
                                // 此時不真的刪除檔案，而是用 rename 的方式將目前的檔案更名（因為檔案可能
                                // 正在使用中），待下次執行更新作業時，便會由 Cleanup 真正將它們刪除。
                                File.Move(clientFileName, toDeleteFileName);
                            }

                            // 3.將下載的檔案更名為欲覆蓋的檔案
                            File.Move(tempFileName, clientFileName);

                            // 4.建立 rollback 項目
                            rollBackItem = new RollbackItem(toDeleteFileName, clientFileName, RollbackAction.Rename);
                            rollBackList.Add(rollBackItem);
                            break;
                        case UpdateAction.Delete:
                            if (File.Exists(clientFileName))	// 檢查檔案是否存在，以確保不會發生錯誤
                            {
                                // 此時不真的刪除檔案，而是用 rename 的方式將目前的檔案更名（因為檔案可能
                                // 正在使用中），待下次執行更新作業時，便會由 Cleanup 真正將它們刪除。
                                File.Move(clientFileName, toDeleteFileName);

                                // 建立 rollback 資訊
                                rollBackItem = new RollbackItem(toDeleteFileName, clientFileName, RollbackAction.Rename);
                                rollBackList.Add(rollBackItem);
                            }
                            break;
                        default:
                            break;
                    }

                    // 注意這裡用了一個技巧：先利用 File.Move 把 client 端要更新的檔案 rename 為待下次
                    // 刪除的暫存檔名，然後才把下載下來的新版檔案 rename 成目標檔案。這樣的話，即使目
                    // 標檔案是正在執行中的主程式（自己），也一樣可以成功更新。這是因為執行中的檔案是
                    // 允許改名的。

                    OnFileUpdated(updEvtArgs);

                    updCount++;
                }

                return updCount;
            }
            catch (Exception)
            {
                // Rollback 
                RollbackItem? rollback;

                for (int i = rollBackList.Count - 1; i >= 0; i--)
                {
                    rollback = rollBackList[i];
                    if (rollback.Operation == RollbackAction.Rename)
                    {
                        if (File.Exists(rollback.TargetFileName))
                        {
                            File.Delete(rollback.TargetFileName);
                        }
                        if (File.Exists(rollback.SourceFileName))
                        {
                            File.Move(rollback.SourceFileName, rollback.TargetFileName);
                        }
                    }
                }
                // Logger.Error(ex, "更新檔案時發生錯誤!");
                throw;
            }
            finally
            {
                rollBackList.Clear();
            }
        }

        /// <summary>
        /// Raises the FileUpdating event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnFileUpdating(HttpUpdaterFileEventArgs e) => FileUpdating?.Invoke(this, e);

        /// <summary>
        /// Raises the FileUpdated event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnFileUpdated(HttpUpdaterFileEventArgs e) => FileUpdated?.Invoke(this, e);


        #region 屬性

        /// <summary>
        /// Gets or sets the client application path.
        /// </summary>
        public string ClientPath
        {
            get { return m_ClientPath; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new InvalidOperationException(ErrorClientPathEmpty);
                }
                m_ClientPath = value;
                if (!value.EndsWith("\\"))	// 自動附加反斯線
                {
                    m_ClientPath = value + "\\";
                }
            }
        }

        /// <summary>
        /// Gets or sets the server URI where update files are located.
        /// </summary>
        public string ServerUri
        {
            get { return m_ServerUri; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new InvalidOperationException("AutoUpdate.ServerUri 不可為空字串!");
                }
                m_ServerUri = value;
                if (!value.EndsWith("/"))	// 自動附加斯線
                {
                    m_ServerUri = value + "/";
                }
            }
        }

        /// <summary>
        /// Gets or sets the change log file name.
        /// </summary>
        public string ChangeLogFileName
        {
            get { return m_ChangeLogFileName; }
            set { m_ChangeLogFileName = value; }
        }

        #endregion

        #region 事件

        /// <summary>
        /// Occurs when a file is about to be updated.
        /// </summary>
        public event EventHandler<HttpUpdaterFileEventArgs>? FileUpdating;

        /// <summary>
        /// Occurs when a file has been updated.
        /// </summary>
        public event EventHandler<HttpUpdaterFileEventArgs>? FileUpdated;

        /// <summary>
        /// Occurs when download progress changes.
        /// </summary>
        public event EventHandler<DownloadProgress>? DownloadProgressChanged;

        #endregion

    }

}
