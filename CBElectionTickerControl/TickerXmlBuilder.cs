using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace CBElectionTickerControl
{
    internal static class TickerXmlBuilder
    {
        private static readonly CultureInfo TurkishCulture =
            CultureInfo.GetCultureInfo("tr-TR");

        public static string Build(CityTickerData data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Candidates == null ||
                data.Candidates.Count != 4)
            {
                throw new InvalidOperationException(
                    "XML üretmek için tam olarak dört aday gereklidir.");
            }

            XElement element = new XElement(
                "element",
                new XAttribute("key", "1"),
                new XElement("design", "CityResult"),

                TextValue(
                    "CityName",
                    NormalizeName(data.CityName)),

                TextValue(
                    "c1_name",
                    NormalizeName(data.Candidates[0].Name)),
                TextValue(
                    "c1_pct",
                    NormalizePercent(data.Candidates[0].Percent)),
                TextValue(
                    "c1_vote",
                    NormalizeVote(data.Candidates[0].Vote)),

                TextValue(
                    "c2_name",
                    NormalizeName(data.Candidates[1].Name)),
                TextValue(
                    "c2_pct",
                    NormalizePercent(data.Candidates[1].Percent)),
                TextValue(
                    "c2_vote",
                    NormalizeVote(data.Candidates[1].Vote)),

                TextValue(
                    "c3_name",
                    NormalizeName(data.Candidates[2].Name)),
                TextValue(
                    "c3_pct",
                    NormalizePercent(data.Candidates[2].Percent)),
                TextValue(
                    "c3_vote",
                    NormalizeVote(data.Candidates[2].Vote)),

                TextValue(
                    "c4_name",
                    NormalizeName(data.Candidates[3].Name)),
                TextValue(
                    "c4_pct",
                    NormalizePercent(data.Candidates[3].Percent)),
                TextValue(
                    "c4_vote",
                    NormalizeVote(data.Candidates[3].Vote))
            );

            XElement group = new XElement(
                "group",
                new XAttribute(
                    "name",
                    "local_preview_ankara"),
                element
            );

            return group.ToString();
        }

        // Mevcut önizleme ve manuel test düğmelerini bozmaz.
        public static string BuildPreview()
        {
            return Build(new CityTickerData
            {
                CityName = "ANKARA",

                Candidates = new List<CandidateTickerData>
                {
                    new CandidateTickerData
                    {
                        CandidateId = 51,
                        Name = "ERDOĞAN",
                        Percent = "45.2",
                        Vote = "12345678"
                    },
                    new CandidateTickerData
                    {
                        CandidateId = 75,
                        Name = "KILIÇDAROĞLU",
                        Percent = "44.8",
                        Vote = "11234567"
                    },
                    new CandidateTickerData
                    {
                        CandidateId = 73,
                        Name = "İNCE",
                        Percent = "5.4",
                        Vote = "1345678"
                    },
                    new CandidateTickerData
                    {
                        CandidateId = 76,
                        Name = "OĞAN",
                        Percent = "4.6",
                        Vote = "1123456"
                    }
                }
            });
        }

        private static XElement TextValue(
            string fieldIdentifier,
            string value)
        {
            return new XElement(
                "value",
                new XAttribute("label", fieldIdentifier),
                new XAttribute("attribute", "text"),
                value ?? string.Empty
            );
        }

        private static string NormalizeName(string value)
        {
            return (value ?? string.Empty)
                .Trim()
                .ToUpper(TurkishCulture);
        }

        private static string NormalizePercent(string value)
        {
            string number = (value ?? string.Empty)
                .Replace("%", string.Empty)
                .Trim()
                .Replace(".", ",");

            return "% " + number;
        }

        private static string NormalizeVote(string value)
        {
            string digits = new string(
                (value ?? string.Empty)
                    .Where(char.IsDigit)
                    .ToArray());

            long vote;

            if (!long.TryParse(digits, out vote))
                return "0 OY";

            return vote.ToString("N0", TurkishCulture) + " OY";
        }
    }
}