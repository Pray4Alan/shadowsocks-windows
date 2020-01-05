using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using Microsoft.VisualBasic.FileIO;

using NLog;

using Shadowsocks.Std.Model;
using Shadowsocks.Std.Util;

namespace Shadowsocks.Std.Win.Util
{
    public class WinI18N : IGetI18N
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IGetResources resources;

        public WinI18N(IGetResources resources)
        {
            this.resources = resources;
        }

        private static Dictionary<string, string> Init(string res, string locale, ref Dictionary<string, string> strings)
        {
            using TextFieldParser csvParser = new TextFieldParser(new StringReader(res));

            csvParser.SetDelimiters(",");

            // search language index
            string[] localeNames = csvParser.ReadFields();

            int enIndex = 0;
            int targetIndex = -1;

            for (int i = 0; i < localeNames.Length; i++)
            {
                if (localeNames[i] == "en")
                    enIndex = i;
                if (localeNames[i] == locale)
                    targetIndex = i;
            }

            // Fallback to same language with different region
            if (targetIndex == -1)
            {
                string localeNoRegion = locale.Split('-')[0];
                for (int i = 0; i < localeNames.Length; i++)
                {
                    if (localeNames[i].Split('-')[0] == localeNoRegion)
                        targetIndex = i;
                }
                if (targetIndex != -1 && enIndex != targetIndex)
                {
                    _logger.Info($"Using {localeNames[targetIndex]} translation for {locale}");
                }
                else
                {
                    // Still not found, exit
                    _logger.Info($"Translation for {locale} not found");
                    return strings;
                }
            }

            // read translation lines
            while (!csvParser.EndOfData)
            {
                string[] translations = csvParser.ReadFields();
                string source = translations[enIndex];
                string translation = translations[targetIndex];

                // source string or translation empty
                if (source.IsNullOrWhiteSpace() || translation.IsNullOrWhiteSpace()) continue;
                // line start with comment
                if (translations[0].TrimStart(' ')[0] == '#') continue;

                strings[source] = translation;
            }

            return strings;
        }

        public Dictionary<string, string> GetI18N(ref Dictionary<string, string> strings)
        {
            string i18n;
            string locale = CultureInfo.CurrentCulture.Name;
            if (!File.Exists(I18N.I18N_FILE))
            {
                i18n = resources.GetI18NCSV();
                //File.WriteAllText(I18N_FILE, i18n, Encoding.UTF8);
            }
            else
            {
                _logger.Info("Using external translation");
                i18n = File.ReadAllText(I18N.I18N_FILE, Encoding.UTF8);
            }
            _logger.Info("Current language is: " + locale);

            return Init(i18n, locale, ref strings);
        }
    }
}
