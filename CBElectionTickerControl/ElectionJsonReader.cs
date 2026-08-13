using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace CBElectionTickerControl
{
    internal sealed class CityTickerData
    {
        public string CityName { get; set; }
        public List<CandidateTickerData> Candidates { get; set; }
    }

    internal sealed class CandidateTickerData
    {
        public int CandidateId { get; set; }
        public string Name { get; set; }
        public string Percent { get; set; }
        public string Vote { get; set; }
        public string ImagePath { get; set; }
    }

    internal static class ElectionJsonReader
    {
        // Kartların sırası hiçbir veri güncellemesinde değişmeyecek.
        private static readonly int[] CandidateOrder =
        {
            51, // Erdoğan
            75, // Kılıçdaroğlu
            73, // İnce
            76  // Oğan
        };

        public static CityTickerData ReadCity(
            string filePath,
            int cityId)
        {
            string json = ReadFileShared(filePath);

            JavaScriptSerializer serializer =
                new JavaScriptSerializer();

            ElectionRoot root =
                serializer.Deserialize<ElectionRoot>(json);

            if (root == null || root.Sehirler == null)
                throw new InvalidOperationException(
                    "JSON içinde Sehirler listesi bulunamadı.");

            ElectionCity sourceCity =
                root.Sehirler.FirstOrDefault(x => x.IlId == cityId);

            if (sourceCity == null)
                throw new InvalidOperationException(
                    "JSON içinde şehir bulunamadı. IlId: " + cityId);

            if (sourceCity.AdaySonuclari == null)
                throw new InvalidOperationException(
                    "Şehrin AdaySonuclari listesi bulunamadı.");

            Dictionary<int, ElectionCandidate> sourceCandidates =
                sourceCity.AdaySonuclari
                    .GroupBy(x => x.AdayId)
                    .ToDictionary(x => x.Key, x => x.First());

            List<CandidateTickerData> candidates =
                new List<CandidateTickerData>();

            foreach (int candidateId in CandidateOrder)
            {
                ElectionCandidate sourceCandidate;

                if (!sourceCandidates.TryGetValue(
                    candidateId,
                    out sourceCandidate))
                {
                    throw new InvalidOperationException(
                        "Aday bulunamadı. AdayId: " + candidateId);
                }

                candidates.Add(new CandidateTickerData
                {
                    CandidateId = candidateId,
                    Name = RepairTurkish(sourceCandidate.Ad),
                    Percent = sourceCandidate.OyOrani,
                    Vote = sourceCandidate.OySayisi,
                    ImagePath = NormalizeImagePath(
                        sourceCandidate.VizrtImagePath)
                });
            }

            return new CityTickerData
            {
                CityName = RepairTurkish(sourceCity.SehirAdi)
                    .ToUpperInvariant(),

                Candidates = candidates
            };
        }

        private static string ReadFileShared(string filePath)
        {
            using (FileStream stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete))
            using (StreamReader reader = new StreamReader(
                stream,
                Encoding.UTF8,
                true))
            {
                return reader.ReadToEnd();
            }
        }

        private static string NormalizeImagePath(string value)
        {
            value = value ?? string.Empty;

            if (value.StartsWith(
                "IMAGE*HT_SECIM",
                StringComparison.OrdinalIgnoreCase))
            {
                value = value.Replace(
                    "IMAGE*HT_SECIM",
                    "IMAGE*/HT_SECIM");
            }

            return value.Trim();
        }

        private static string RepairTurkish(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            string result = value.Trim();

            // Gerçek JSON dosyasında oluşmuş olabilecek
            // UTF-8/Windows-1252 bozulmasını düzeltir.
            if (result.Contains("Ã") ||
                result.Contains("Ä") ||
                result.Contains("Å"))
            {
                byte[] bytes =
                    Encoding.GetEncoding(1252).GetBytes(result);

                result = Encoding.UTF8.GetString(bytes);
            }

            return result;
        }

        private sealed class ElectionRoot
        {
            public List<ElectionCity> Sehirler { get; set; }
        }

        private sealed class ElectionCity
        {
            public int IlId { get; set; }
            public string PlakaKodu { get; set; }
            public string SehirAdi { get; set; }
            public List<ElectionCandidate> AdaySonuclari { get; set; }
        }

        private sealed class ElectionCandidate
        {
            public int AdayId { get; set; }
            public string Ad { get; set; }
            public string OyOrani { get; set; }
            public string OySayisi { get; set; }
            public string VizrtImagePath { get; set; }
        }
    }
}