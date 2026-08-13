using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CBElectionTickerControl
{
    public sealed class LocalVizEngineClient : IDisposable
    {
        // Değiştirilemez: yalnızca bu bilgisayardaki Viz Engine.
        private const string LocalHost = "127.0.0.1";
        private const int LocalPort = 6100;

        // Yalnızca bizim sayfa direktörümüz çalıştırılabilir.
        private const string Page34Command =
            "MAIN_SCENE*STAGE*DIRECTOR*SOL_4_ADAY_SAYFA START NORMAL";

        private const string Page12Command =
            "MAIN_SCENE*STAGE*DIRECTOR*SOL_4_ADAY_SAYFA START REVERSE";

        private readonly SemaphoreSlim _sendLock =
            new SemaphoreSlim(1, 1);

        private TcpClient _client;
        private NetworkStream _stream;

        public Task ShowPage34Async()
        {
            return SendAllowedCommandAsync(Page34Command);
        }

        public Task ShowPage12Async()
        {
            return SendAllowedCommandAsync(Page12Command);
        }

        private async Task SendAllowedCommandAsync(string command)
        {
            if (command != Page34Command && command != Page12Command)
                throw new InvalidOperationException(
                    "İzin verilmeyen Viz Engine komutu.");

            await _sendLock.WaitAsync();

            try
            {
                Exception lastError = null;

                // Bağlantı kopmuşsa bir kez yeniden bağlanıp tekrar dener.
                for (int attempt = 0; attempt < 2; attempt++)
                {
                    try
                    {
                        await EnsureConnectedAsync();

                        string fullCommand = "-1 " + command + "\0";
                        byte[] data = Encoding.UTF8.GetBytes(fullCommand);

                        await _stream.WriteAsync(data, 0, data.Length);
                        await _stream.FlushAsync();
                        return;
                    }
                    catch (Exception ex)
                    {
                        lastError = ex;
                        CloseConnection();
                    }
                }

                throw new IOException(
                    "Yerel Viz Engine komutu gönderilemedi.",
                    lastError);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        private async Task EnsureConnectedAsync()
        {
            if (_client != null &&
                _client.Connected &&
                _stream != null)
            {
                return;
            }

            CloseConnection();

            _client = new TcpClient();
            await _client.ConnectAsync(LocalHost, LocalPort);
            _stream = _client.GetStream();
        }

        private void CloseConnection()
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }

            if (_client != null)
            {
                _client.Close();
                _client = null;
            }
        }

        public void Dispose()
        {
            CloseConnection();
            _sendLock.Dispose();
        }
    }
}