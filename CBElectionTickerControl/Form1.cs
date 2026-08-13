using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CBElectionTickerControl
{
    public partial class Form1 : Form
    {
        private readonly VizTickerClient _tickerClient =
            new VizTickerClient();
        private readonly LocalVizEngineClient _vizEngineClient =
            new LocalVizEngineClient();

        public Form1()
        {
            InitializeComponent();
            _pageTimer.Interval = 5000;
            _pageTimer.Tick += PageTimer_Tick;
            _jsonDebounceTimer.Interval = 400;
            _jsonDebounceTimer.Tick += JsonDebounceTimer_Tick;
            ConfigureJsonWatcher();
            FormClosed += (sender, e) =>
            {
                _jsonWatcher.EnableRaisingEvents = false;
                _jsonWatcher.Changed -= JsonWatcher_Changed;
                _jsonWatcher.Created -= JsonWatcher_Changed;
                _jsonWatcher.Renamed -= JsonWatcher_Renamed;
                _jsonWatcher.Dispose();

                _jsonDebounceTimer.Stop();
                _jsonDebounceTimer.Tick -= JsonDebounceTimer_Tick;
                _jsonDebounceTimer.Dispose();
                _pageTimer.Stop();
                _pageTimer.Tick -= PageTimer_Tick;
                _pageTimer.Dispose();
                _vizEngineClient.Dispose();
            };
        }

        private const string LiveJsonDirectory =
    @"C:\Users\metutun\Documents\viztickerservice secim";

        private const string LiveJsonFileName =
    "canli_test.json";

        private readonly System.IO.FileSystemWatcher _jsonWatcher =
    new System.IO.FileSystemWatcher();

        private readonly System.Windows.Forms.Timer _jsonDebounceTimer =
            new System.Windows.Forms.Timer();

        private bool _jsonUpdateRunning;
        private bool _jsonUpdatePending;

        private void btnConnectLocal_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                _tickerClient.ConnectLocal();
                _jsonWatcher.EnableRaisingEvents = true;
                ScheduleJsonUpdate();

                lblStatus.Text = "YEREL TEST TICKER'A BAĞLANDI";
                lblStatus.ForeColor = Color.Green;
                btnConnectLocal.Enabled = false;
                btnSendLocalTest.Enabled = true;
                btnReadTickerState.Enabled = true;
                btnRecreateLocalElement.Enabled = true;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "BAĞLANTI HATASI";
                lblStatus.ForeColor = Color.Red;

                MessageBox.Show(
                    ex.Message,
                    "Yerel bağlantı hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnPreviewXml_Click(
            object sender,
            EventArgs e)
        {
            const string liveJsonPath =
                @"C:\Users\metutun\Documents\viztickerservice secim\canli_test.json";

            try
            {
                CityTickerData data =
                    ElectionJsonReader.ReadCity(
                        liveJsonPath,
                        6); // Ankara IlId

                string xml = TickerXmlBuilder.Build(data);

                txtXmlPreview.Text = xml;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "JSON okuma hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnSendLocalTest_Click(object sender, EventArgs e)
        {
            if (!_tickerClient.IsConnected)
            {
                MessageBox.Show(
                    "Önce yerel test ticker bağlantısını kurun.",
                    "Bağlantı yok",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            const string liveJsonPath =
    @"C:\Users\metutun\Documents\viztickerservice secim\canli_test.json";

            CityTickerData data =
                ElectionJsonReader.ReadCity(
                    liveJsonPath,
                    6);

            string xml = TickerXmlBuilder.Build(data);
            txtXmlPreview.Text = xml;

            DialogResult result = MessageBox.Show(
                "Test verisi yalnızca şu ticker'a gönderilecek:\n\n" +
                VizTickerClient.LocalTestTickerName +
                "\n\nDevam edilsin mi?",
                "Yerel test gönderimi",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                string sendResult = _tickerClient.SendValidatedLocalTestXml(xml);
                string storedXml = _tickerClient.ReadLocalTestElementXml();

                txtXmlPreview.Text =
                    "GÖNDERİLEN XML:\r\n" + xml +
                    "\r\n\r\nTICKER SERVICE GERİ OKUMA:\r\n" + storedXml;

                MessageBox.Show(
                    "Yerel test verisi gönderildi.",
                    "Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Gönderim hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async void btnShowPage34_Click(object sender, EventArgs e)
        {
            btnShowPage34.Enabled = false;

            try
            {
                await _vizEngineClient.ShowPage34Async();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Yerel Viz Engine hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnShowPage34.Enabled = true;
            }
        }

        private async void btnShowPage12_Click(object sender, EventArgs e)
        {
            btnShowPage12.Enabled = false;

            try
            {
                await _vizEngineClient.ShowPage12Async();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Yerel Viz Engine hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnShowPage12.Enabled = true;
            }
        }

        private readonly System.Windows.Forms.Timer _pageTimer =
    new System.Windows.Forms.Timer();

        private bool _showPage34Next = true;
        private bool _pageCommandRunning;

        private async void PageTimer_Tick(object sender, EventArgs e)
        {
            if (_pageCommandRunning)
                return;

            _pageCommandRunning = true;

            try
            {
                if (_showPage34Next)
                    await _vizEngineClient.ShowPage34Async();
                else
                    await _vizEngineClient.ShowPage12Async();

                _showPage34Next = !_showPage34Next;
            }
            catch (Exception ex)
            {
                _pageTimer.Stop();

                MessageBox.Show(
                    ex.Message + "\n\nOtomatik sayfa değişimi durduruldu.",
                    "Yerel Viz Engine hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                _pageCommandRunning = false;
            }
        }

        private void chkAutoPageSwitch_CheckedChanged(
            object sender,
            EventArgs e)
        {
            if (chkAutoPageSwitch.Checked)
            {
                _showPage34Next = true;
                _pageTimer.Start();
            }
            else
            {
                _pageTimer.Stop();
            }
        }

        private void ConfigureJsonWatcher()
        {
            _jsonWatcher.Path = LiveJsonDirectory;
            _jsonWatcher.Filter = LiveJsonFileName;

            _jsonWatcher.NotifyFilter =
                System.IO.NotifyFilters.LastWrite |
                System.IO.NotifyFilters.Size |
                System.IO.NotifyFilters.FileName;

            _jsonWatcher.SynchronizingObject = this;
            _jsonWatcher.Changed += JsonWatcher_Changed;
            _jsonWatcher.Created += JsonWatcher_Changed;
            _jsonWatcher.Renamed += JsonWatcher_Renamed;
        }
        private void JsonWatcher_Changed(object sender,System.IO.FileSystemEventArgs e)
        {
            ScheduleJsonUpdate();
        }

        private void JsonWatcher_Renamed(object sender,System.IO.RenamedEventArgs e)
        {
            ScheduleJsonUpdate();
        }

        private void ScheduleJsonUpdate()
        {
            _jsonDebounceTimer.Stop();
            _jsonDebounceTimer.Start();
        }

        private async Task<string> BuildLatestXmlWithRetryAsync()
        {
            string filePath = System.IO.Path.Combine(
                LiveJsonDirectory,
                LiveJsonFileName);

            Exception lastError = null;

            for (int attempt = 1; attempt <= 5; attempt++)
            {
                try
                {
                    CityTickerData data =
                        ElectionJsonReader.ReadCity(filePath, 6);

                    return TickerXmlBuilder.Build(data);
                }
                catch (Exception ex)
                {
                    lastError = ex;

                    if (attempt < 5)
                        await Task.Delay(attempt * 100);
                }
            }

            throw new InvalidOperationException(
                "JSON dosyası beş denemede sağlam okunamadı. " +
                "Son doğru yayın verisi korunuyor.",
                lastError);
        }
        private async void JsonDebounceTimer_Tick( object sender,EventArgs e)
        {
            _jsonDebounceTimer.Stop();

            if (_jsonUpdateRunning)
            {
                _jsonUpdatePending = true;
                return;
            }

            _jsonUpdateRunning = true;

            try
            {
                string xml = await BuildLatestXmlWithRetryAsync();

                string result =
    _tickerClient.SendValidatedLocalTestXml(xml);

                string storedXml =
                    _tickerClient.ReadLocalTestElementXml();

                txtXmlPreview.Text =
                    "OTOMATİK GÜNCELLEME: " + result +
                    "\r\n\r\nGÖNDERİLEN XML:\r\n" + xml +
                    "\r\n\r\nTICKER SERVICE GERİ OKUMA:\r\n" + storedXml;
            }
            catch (Exception ex)
            {
                // Hatalı/yarım veri gönderilmez; son doğru yayın korunur.
                txtXmlPreview.Text =
                    "OTOMATİK GÜNCELLEME BAŞARISIZ\r\n" +
                    "Son doğru yayın verisi korunuyor.\r\n\r\n" +
                    ex.Message;
            }
            finally
            {
                _jsonUpdateRunning = false;

                if (_jsonUpdatePending)
                {
                    _jsonUpdatePending = false;
                    _jsonDebounceTimer.Start();
                }
            }



        }

        private void btnRecreateLocalElement_Click(
            object sender,
            EventArgs e)
        {
            if (!_tickerClient.IsConnected)
                return;

            DialogResult answer = MessageBox.Show(
                "Yalnızca local_preview_ankara / key=1 elementi " +
                "silinip TTL olmadan yeniden eklenecek.\n\nDevam edilsin mi?",
                "Yerel elementi yenile",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (answer != DialogResult.Yes)
                return;

            try
            {
                string filePath = System.IO.Path.Combine(
                    LiveJsonDirectory,
                    LiveJsonFileName);

                CityTickerData data =
                    ElectionJsonReader.ReadCity(filePath, 6);

                string xml = TickerXmlBuilder.Build(data);

                string result =
                    _tickerClient.RecreateLocalTestElement(xml);

                string storedXml =
                    _tickerClient.ReadLocalTestElementXml();

                txtXmlPreview.Text =
                    result +
                    "\r\n\r\nTICKER SERVICE GERİ OKUMA:\r\n" +
                    storedXml;

                btnRecreateLocalElement.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Yerel element yenileme hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnReadTickerState_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                txtXmlPreview.Text =
                    _tickerClient.ReadLocalTestGroupState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ticker durum okuma hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}