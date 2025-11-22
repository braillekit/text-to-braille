using System.Diagnostics;
using System.Text;

namespace EasyBrailleEdit.Common.Utilities.Windows;

/// <summary>
/// 此類別可用來執行檔案。
/// </summary>
public sealed class FileRunner : IDisposable
{
	private ProcessStartInfo m_StartInfo;
	private Process m_Process;
	private bool m_NeedWait;		// 是否等待應用程式執行結束
	private TimeSpan m_WaitTime;			// 要等多久，TimeSpan.Zero 表示採用內定值（30分鐘）

	private string m_ErrMsg;
	private StringBuilder m_StdError;
	private StringBuilder m_StdOutput;
    private readonly object _lockObject = new object();

    private event DataReceivedEventHandler m_StdOutputReceivedEvent;

    /// <summary>
    /// 初始化 FileRunner 類別的新執行個體。
    /// </summary>
	public FileRunner()
	{
		m_StdError = new StringBuilder();
		m_StdOutput = new StringBuilder();

		m_StartInfo = new ProcessStartInfo();

		m_NeedWait = true;
		m_WaitTime = TimeSpan.Zero;
		m_StartInfo.CreateNoWindow = false;
		m_StartInfo.UseShellExecute = false;
		m_StartInfo.WorkingDirectory = "";
		m_StartInfo.WindowStyle = ProcessWindowStyle.Normal;
		m_StartInfo.RedirectStandardError = false;
		m_StartInfo.RedirectStandardOutput = false;
	}

    /// <summary>
    /// 釋放由 FileRunner 使用的所有資源。
    /// </summary>
	public void Dispose()
	{
		if (m_Process != null) 
		{
			KillProcess();
		}
	}

    /// <summary>
    /// 以非同步方式執行指定的檔案。
    /// </summary>
    /// <param name="filename">要執行的檔案名稱。</param>
    /// <param name="argument">傳遞給應用程式的命令列引數。</param>
    /// <returns>表示非同步作業的 Task，其結果為處理序的結束代碼。</returns>
    public async Task<int> RunAsync(string filename, string argument)
    {
        if (Running)
        {
            throw new InvalidOperationException("先前執行的程式尚未結束!");
        }

        m_ErrMsg = "";
        m_StdError.Length = 0;
        m_StdOutput.Length = 0;

        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentException("未指定欲執行的檔案名稱!", nameof(filename));
        }

        if (!string.IsNullOrEmpty(WorkingDirectory) && !Directory.Exists(WorkingDirectory))
        {
            throw new DirectoryNotFoundException("指定的工作路徑不存在: " + WorkingDirectory);
        }

        m_StartInfo.FileName = filename;
        m_StartInfo.Arguments = argument;

        m_Process = new Process { StartInfo = m_StartInfo, EnableRaisingEvents = true };

        var tcs = new TaskCompletionSource<int>();

        m_Process.Exited += (sender, args) => tcs.TrySetResult(m_Process.ExitCode);

        if (RedirectStandardOutput)
        {
            m_Process.OutputDataReceived += Process_OutputDataReceived;
            m_Process.ErrorDataReceived += Process_ErrorDataReceived;
        }

        try
        {
            if (!m_Process.Start())
            {
                m_Process.Dispose();
                m_Process = null;
                throw new InvalidOperationException("沒有啟動新的處理序（可能重複使用既有的處理序）。");
            }

            if (RedirectStandardOutput)
            {
                m_Process.BeginOutputReadLine();
                m_Process.BeginErrorReadLine();
            }

            if (!m_NeedWait)
            {
                tcs.TrySetResult(0); // 不需要等待，立即完成 Task
                return await tcs.Task;
            }

            var waitDuration = m_WaitTime <= TimeSpan.Zero
                                ? TimeSpan.FromMinutes(30)  // 預設等待 30 分鐘
                                : m_WaitTime;

            using (var timeoutCts = new System.Threading.CancellationTokenSource(waitDuration))
            {
                using (timeoutCts.Token.Register(() => tcs.TrySetException(new TimeoutException($"執行的應用程式超過指定的等待時間 ({waitDuration.TotalSeconds} 秒) 而被強制終止!"))))
                {
                    return await tcs.Task;
                }
            }
        }
        catch (Exception ex)
        {
            if (ex is TimeoutException)
            {
                KillProcess(); // 只有在逾時的時候才強制終止
            }
            else
            {
                // 對於其他錯誤，確保資源被釋放
                if (m_Process != null)
                {
                    m_Process.Dispose();
                    m_Process = null;
                }
            }
            throw;
        }
        finally
        {
            if (m_Process != null && RedirectStandardOutput)
            {
                m_Process.OutputDataReceived -= Process_OutputDataReceived;
                m_Process.ErrorDataReceived -= Process_ErrorDataReceived;
            }
        }
    }

    /// <summary>
    /// 執行檔案。
    /// </summary>
    /// <param name="filename">檔案名稱。</param>
    /// <param name="argument">檔案執行時帶的參數，可傳空字串。</param>
    /// <returns>若執行成功則傳回 true，否則傳回 false 並設定 ErrorMsg 屬性。</returns>
    public bool Run(string filename, string argument)
    {
        try
        {
            // 同步等待非同步版本完成
            RunAsync(filename, argument).GetAwaiter().GetResult();
            return true;
        }
        catch (Exception ex)
        {
            m_ErrMsg = ex.Message;
            return false;
        }
    }

	void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
	{
		lock (_lockObject)
		{
			m_StdOutput.Append(e.Data);
			m_StdOutput.Append(System.Environment.NewLine);
		}
		OnStdOutputReceived(e);
	}

	void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
	{
		// Standard error 一樣也輸出到 standard output device.
		Process_OutputDataReceived(sender, e);
	}

    private void OnStdOutputReceived(DataReceivedEventArgs args)
    {
        if (m_StdOutputReceivedEvent != null)
        {
            // 觸發用戶端事件（Note: 用戶端若要在此事件中更新 UI，務必 marshal 回主執行緒!）
            m_StdOutputReceivedEvent(this, args);
        }
    }

    /// <summary>
    /// 強制終止執行的程式。
    /// </summary>
    public void KillProcess()
    {
        if (m_Process == null)
            return;

        try
        {
            if (!m_Process.HasExited)
            {
                m_Process.Kill();
                m_ErrMsg = "執行的應用程式超過指定等待時間而被強制終止!";
            }
        }
        catch (Exception ex)
        {
            m_ErrMsg = "嘗試終止處理序時發生錯誤: " + ex.Message;
        }
        finally
        {
            m_Process.Close();
            m_Process.Dispose();
            m_Process = null;
        }
    }
	#region 屬性------------------------------

    /// <summary>
    /// 取得一個值，指出處理序是否仍在執行。
    /// </summary>
	public bool Running
	{
		get 
		{
			if (m_Process == null)
				return false;
			return !m_Process.HasExited;
		}
	}

    /// <summary>
    /// 取得或設定要執行之處理序的工作目錄。
    /// </summary>
	public string WorkingDirectory
	{
		get { return m_StartInfo.WorkingDirectory; }
		set { m_StartInfo.WorkingDirectory = value; }
	}

    /// <summary>
    /// 取得或設定一個值，指出是否要使用作業系統 shell 來啟動處理序。
    /// </summary>
	public bool UseShellExecute
	{
		get { return m_StartInfo.UseShellExecute; }
		set 
		{
			if (value && RedirectStandardOutput)
			{
				throw new InvalidOperationException("UseShellExecute 和 RedirectStandardOutput 不可同時為 true!");
			}
			m_StartInfo.UseShellExecute = value; 
		}
	}

    /// <summary>
    /// 取得或設定是否顯示處理序的視窗。
    /// </summary>
	public bool ShowWindow
	{
		get { return !m_StartInfo.CreateNoWindow; }
		set { m_StartInfo.CreateNoWindow = !value; }
	}

    /// <summary>
    /// 取得或設定一個值，指出是否將應用程式的輸出寫入 StandardOutput。
    /// </summary>
	public bool RedirectStandardOutput
	{
		get { return m_StartInfo.RedirectStandardOutput; }
		set
		{
			if (value && UseShellExecute)
			{
				throw new InvalidOperationException("UseShellExecute 和 RedirectStandardOutput 不可同時為 true!");
			}

			m_StartInfo.RedirectStandardOutput = value;
			m_StartInfo.RedirectStandardError = value;
		}
	}

    /// <summary>
    /// 取得或設定是否等待應用程式執行結束。
    /// </summary>
	public bool NeedWait
	{
		get { return m_NeedWait; }
		set { m_NeedWait = value; }
	}

	/// <summary>
	/// 要等多久。若設為 TimeSpan.Zero，表示採用內定值（30分鐘）。超過此時間 process 將被強制終止。
	/// </summary>
	public TimeSpan WaitTime
	{
		get { return m_WaitTime; }
		set
		{
			if (value < TimeSpan.Zero || value.TotalHours > 24)
			{
				throw new ArgumentOutOfRangeException(nameof(WaitTime), "WaitTime 不可為負值或超過 24 小時!");
			}
			m_WaitTime = value;
		}
	}

    /// <summary>
    /// 取得上次執行發生錯誤時的錯誤訊息。
    /// </summary>
	public string ErrorMsg
	{
		get { return m_ErrMsg; }
	}

    /// <summary>
    /// 取得從執行的應用程式所擷取到的標準輸出。
    /// </summary>
	public string StdOutputMsg
	{
		get
		{
			return m_StdOutput.ToString();
		}
	}

    /// <summary>
    /// 取得或設定啟動處理序時使用的視窗狀態。
    /// </summary>
	public ProcessWindowStyle WindowStyle
	{
		get { return m_StartInfo.WindowStyle; }
		set { m_StartInfo.WindowStyle = value; }
	}

    /// <summary>
    /// 取得或設定用於啟動處理序的動詞，例如 "runas"。
    /// </summary>
	public string Verb
	{
		get { return m_StartInfo.Verb; }
		set { m_StartInfo.Verb = value; }
	}

    /// <summary>
    /// 取得或設定一個值，指出處理序無法啟動時，是否要顯示錯誤對話方塊。
    /// </summary>
	public bool ErrorDialog
	{
		get { return m_StartInfo.ErrorDialog; }
		set { m_StartInfo.ErrorDialog = value; }
	}

    /// <summary>
    /// 取得處理序已結束執行的結束代碼。
    /// </summary>
	public int ExitCode 
	{
		get 
		{
			if (m_Process == null)
				throw new Exception("沒有執行程式或程式已完全釋放，故無法取得 ExitCode。");
			return m_Process.ExitCode;
		}
	}

    /// <summary>
    /// 取得處理序結束執行的時間。
    /// </summary>
	public DateTime ExitTime
	{
		get
		{
			if (m_Process == null)
				throw new Exception("沒有執行程式或程式已完全釋放，故無法取得 ExitTime。");
			return m_Process.ExitTime;
		}
	}

	#endregion

	#region 事件--------------------------------

    /// <summary>
    /// 當應用程式將資料寫入其重新導向的 StandardOutput 資料流時發生。
    /// </summary>
	public event DataReceivedEventHandler StdOutputReceived
	{
		add
		{
			m_StdOutputReceivedEvent += value;
		}
		remove
		{
			m_StdOutputReceivedEvent -= value;
		}
	}

	#endregion
}