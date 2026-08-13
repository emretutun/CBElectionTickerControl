using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using VIZTICKERLib;

namespace CBElectionTickerControl
{
    internal sealed class VizTickerClient
    {
        // Uygulamanın erişebileceği tek ticker.
        public const string LocalTestTickerName =
            "CBSECIMLOCALTEST2026";

        // Uygulamanın erişebileceği tek grup.
        public const string LocalGroupName =
            "local_preview_ankara";

        private static readonly HashSet<string> AllowedTextFields =
            new HashSet<string>(StringComparer.Ordinal)
            {
                "CityName",

                "c1_name", "c1_pct", "c1_vote",
                "c2_name", "c2_pct", "c2_vote",
                "c3_name", "c3_pct", "c3_vote",
                "c4_name", "c4_pct", "c4_vote"
            };

        private static readonly HashSet<string> AllowedImageFields =
            new HashSet<string>(StringComparer.Ordinal)
            {
                "c1_img",
                "c2_img",
                "c3_img",
                "c4_img"
            };
        public string ReadLocalTestGroupState()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException(
                    "Önce yerel test ticker bağlantısını kurun.");
            }

            lock (_sendLock)
            {
                return
                    "GRUP LİSTESİ:\r\n" +
                    _tickerControl.ListGroups() +
                    "\r\n\r\nAKTİF GRUP:\r\n" +
                    _tickerControl.GetCurrentGroupName() +
                    "\r\n\r\nYEREL GRUP XML:\r\n" +
                    _tickerControl.GetGroup(LocalGroupName);
            }
        }
        private readonly object _sendLock = new object();

        private IVizTickers _tickers;
        private IVizTickerControl _tickerControl;

        private string _lastElementXml;

        public bool IsConnected
        {
            get { return _tickerControl != null; }
        }

        public void ConnectLocal()
        {
            lock (_sendLock)
            {
                _tickers = new VizTickers();

                if (_tickers.GetTicker(LocalTestTickerName) == null)
                    _tickers.AddTicker(LocalTestTickerName);

                _tickerControl =
                    _tickers.GetTicker(LocalTestTickerName);

                if (_tickerControl == null)
                {
                    throw new InvalidOperationException(
                        "Yerel test ticker bağlantısı kurulamadı.");
                }
                _tickerControl.StartVizCommunication(LocalTestTickerName);

                _lastElementXml = null;

                _lastElementXml = null;
            }
        }

        public string SendValidatedLocalTestXml(string xml)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException(
                    "Önce yerel test ticker bağlantısını kurun.");
            }

            XElement group = ParseAndValidate(xml);

            XElement element =
                group.Elements("element").Single();

            string groupName =
                (string)group.Attribute("name");

            string elementKey =
                (string)element.Attribute("key");

            string groupXml =
                group.ToString(SaveOptions.DisableFormatting);

            string elementXml =
                element.ToString(SaveOptions.DisableFormatting);

            lock (_sendLock)
            {
                // Aynı veri yeniden geldiyse gereksiz UpdateElement gönderme.
                // Fakat servis yeniden başlamış olabileceği için elementin
                // hâlâ var olduğunu da kontrol ediyoruz.
                if (_lastElementXml == elementXml)
                {
                    string existingSameElement =
                        TryGetElement(groupName, elementKey);

                    if (!string.IsNullOrWhiteSpace(existingSameElement))
                        return "DEĞİŞİKLİK YOK";
                }

                string existingElement =
                    TryGetElement(groupName, elementKey);

                string operation;

                if (!string.IsNullOrWhiteSpace(existingElement))
                {
                    // Normal canlı veri güncellemesi.
                    // Grup silinmez, sahne boş kalmaz.
                    _tickerControl.UpdateElement(
                        groupName,
                        elementXml);

                    operation = "ELEMENT GÜNCELLENDİ";
                }
                else
                {
                    string existingGroup =
                        TryGetGroup(groupName);

                    if (string.IsNullOrWhiteSpace(existingGroup))
                    {
                        // Sadece ilk çalışmada grup ve element oluşturulur.
                        _tickerControl.AddGroupAfterGroup(
                            "",
                            groupXml);

                        operation = "GRUP VE ELEMENT OLUŞTURULDU";
                    }
                    else
                    {
                        // Grup var fakat element yoksa element eklenir.
                        _tickerControl.PutElement(
                            groupName,
                            elementXml);

                        operation = "ELEMENT GRUBA EKLENDİ";
                    }
                }

                string verification =
                    TryGetElement(groupName, elementKey);

                if (string.IsNullOrWhiteSpace(verification))
                {
                    throw new InvalidOperationException(
                        "Ticker Service işlemi aldı fakat element doğrulanamadı.");
                }

                _lastElementXml = elementXml;

                return operation;
            }
        }

        public string ReadLocalTestElementXml()
        {
            if (!IsConnected)
                throw new InvalidOperationException(
                    "Önce yerel test ticker bağlantısını kurun.");

            lock (_sendLock)
            {
                return TryGetElement(LocalGroupName, "1");
            }
        }

        private XElement ParseAndValidate(string xml)
        {
            XElement group;

            try
            {
                group = XElement.Parse(
                    xml,
                    LoadOptions.None);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Gönderilecek XML geçerli değil.",
                    ex);
            }

            if (group.Name.LocalName != "group")
            {
                throw new InvalidOperationException(
                    "XML kök elemanı group olmalıdır.");
            }

            string groupName =
                (string)group.Attribute("name");

            if (groupName != LocalGroupName)
            {
                throw new InvalidOperationException(
                    "İzin verilmeyen grup: " + groupName);
            }

            List<XElement> elements =
                group.Elements("element").ToList();

            if (elements.Count != 1)
            {
                throw new InvalidOperationException(
                    "XML içerisinde yalnızca bir element olmalıdır.");
            }

            XElement element = elements[0];

            string elementKey =
                (string)element.Attribute("key");

            if (elementKey != "1")
            {
                throw new InvalidOperationException(
                    "Yalnızca 1 anahtarlı yerel test elementi kullanılabilir.");
            }

            string design =
                (string)element.Element("design");

            if (design != "CityResult")
            {
                throw new InvalidOperationException(
                    "Yalnızca CityResult tasarımı kullanılabilir.");
            }

            HashSet<string> usedFields =
                new HashSet<string>(StringComparer.Ordinal);

            foreach (XElement value in element.Elements("value"))
            {
                string label =
                    (string)value.Attribute("label");

                string attribute =
                    (string)value.Attribute("attribute");

                if (!usedFields.Add(label))
                {
                    throw new InvalidOperationException(
                        "Tekrarlanan alan bulundu: " + label);
                }

                if (AllowedTextFields.Contains(label))
                {
                    if (attribute != "text")
                    {
                        throw new InvalidOperationException(
                            label + " alanının attribute değeri text olmalıdır.");
                    }

                    continue;
                }

                if (AllowedImageFields.Contains(label))
                {
                    if (attribute != "image")
                    {
                        throw new InvalidOperationException(
                            label + " alanının attribute değeri image olmalıdır.");
                    }

                    string imagePath = value.Value ?? "";

                    if (!string.IsNullOrWhiteSpace(imagePath) &&
                        !imagePath.StartsWith(
                            "IMAGE*/HT_SECIM/C_EKRAN/",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            "İzin verilmeyen görsel yolu: " + imagePath);
                    }

                    continue;
                }

                throw new InvalidOperationException(
                    "İzin verilmeyen alan: " + label);
            }

            return group;
        }

        private string TryGetGroup(string groupName)
        {
            try
            {
                return _tickerControl.GetGroup(groupName);
            }
            catch
            {
                return string.Empty;
            }
        }

        private string TryGetElement(
            string groupName,
            string elementKey)
        {
            try
            {
                return _tickerControl.GetElement(
                    groupName,
                    elementKey);
            }
            catch
            {
                return string.Empty;
            }
        }

        public string RecreateLocalTestElement(string xml)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException(
                    "Önce yerel test ticker bağlantısını kurun.");
            }

            XElement group = ParseAndValidate(xml);
            XElement element = group.Elements("element").Single();

            string groupName = (string)group.Attribute("name");
            string elementKey = (string)element.Attribute("key");
            string elementXml =
                element.ToString(SaveOptions.DisableFormatting);

            lock (_sendLock)
            {
                if (string.IsNullOrWhiteSpace(TryGetGroup(groupName)))
                {
                    throw new InvalidOperationException(
                        "Yerel test grubu bulunamadı.");
                }

                if (!string.IsNullOrWhiteSpace(
                    TryGetElement(groupName, elementKey)))
                {
                    _tickerControl.DeleteElement(
                        groupName,
                        elementKey);
                }

                _tickerControl.PutElement(
                    groupName,
                    elementXml);

                string verification =
                    TryGetElement(groupName, elementKey);

                if (string.IsNullOrWhiteSpace(verification))
                {
                    throw new InvalidOperationException(
                        "Element yeniden oluşturuldu fakat doğrulanamadı.");
                }

                _lastElementXml = elementXml;

                return "ELEMENT YENİDEN OLUŞTURULDU";
            }
        }
    }
}