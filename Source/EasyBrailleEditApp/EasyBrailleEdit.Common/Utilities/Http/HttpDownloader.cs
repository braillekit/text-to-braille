namespace EasyBrailleEdit.Common.Utilities.Http
{
    /// <summary>
    /// Provides utility methods for downloading files via HTTP.
    /// </summary>
    public static class HttpDownloader
    {
        // 讓 HttpClient 成為靜態的，以便在應用程式中重複使用。
        private static readonly HttpClient HttpClient = new HttpClient();

        /// <summary>
        /// Downloads a file from the specified URL to the destination path.
        /// </summary>
        /// <param name="url">The URL to download from.</param>
        /// <param name="destinationPath">The destination file path.</param>
        /// <param name="progress">Optional progress reporter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task DownloadFileAsync(
            string url,
            string destinationPath,
            IProgress<DownloadProgress>? progress = null,
            CancellationToken cancellationToken = default)
        {
            using (var response = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength;

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken))
                using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    var totalBytesRead = 0L;
                    var buffer = new byte[8192];
                    var isMoreToRead = true;

                    do
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        if (bytesRead == 0)
                        {
                            isMoreToRead = false;
                        }
                        else
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);

                            totalBytesRead += bytesRead;

                            progress?.Report(new DownloadProgress
                            {
                                TotalBytes = totalBytes,
                                BytesRead = totalBytesRead
                            });
                        }
                    }
                    while (isMoreToRead);
                }
            }
        }
    }

    /// <summary>
    /// Represents the progress of a download operation.
    /// </summary>
    public class DownloadProgress
    {
        /// <summary>
        /// Gets or sets the total bytes to download.
        /// </summary>
        public long? TotalBytes { get; set; }

        /// <summary>
        /// Gets or sets the bytes read so far.
        /// </summary>
        public long BytesRead { get; set; }

        /// <summary>
        /// Gets the progress percentage.
        /// </summary>
        public double ProgressPercentage => TotalBytes.HasValue ? (double)BytesRead / TotalBytes.Value * 100.0 : 0;
    }
}