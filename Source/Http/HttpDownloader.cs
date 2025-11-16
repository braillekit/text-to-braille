using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Huanlin.Common.Http
{
    /// <summary>
    /// A simple HTTP client wrapper for downloading file with progress events.
    /// Ref: https://github.com/txstudio/HttpClientDownloadWithProgressBar/blob/master/ConsoleApp/Program.cs
    /// </summary>
    public class HttpDownloader
    {
        private readonly HttpClient _client;

        public HttpDownloader(EventHandler<HttpProgressEventArgs> progressHandler, bool noCache = false)
        {
            var handler = new ProgressMessageHandler()
            {
                InnerHandler = new HttpClientHandler()
            };
            handler.HttpReceiveProgress += progressHandler;

            _client = new HttpClient(handler);

            _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = noCache
            };
        }

        public Task<byte[]> GetByteArrayAsync(string url)
        {
            return _client.GetByteArrayAsync(url);
        }

        public async Task DownloadAsync(Uri uri, string fileName)
        {
            var bytes = await _client.GetByteArrayAsync(uri);
            await File.WriteAllBytesAsync(fileName, bytes);
        }

        public Task<string> DownloadStringAsync(Uri uri)
        {
            return _client.GetStringAsync(uri);
        }
    }
}
