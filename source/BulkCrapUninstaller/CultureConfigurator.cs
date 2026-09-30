using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace BulkCrapUninstaller
{
    public static class CultureConfigurator
    {
        private static IEnumerable<CultureInfo> _supportedLanguages;
        private static CultureInfo _enUsCulture;

        private static CultureInfo EnUsCulture => _enUsCulture ??= CultureInfo.GetCultureInfo("en-US");

        public static IEnumerable<CultureInfo> SupportedLanguages => _supportedLanguages ??= GetSupportedLanguages();

        private static IEnumerable<CultureInfo> GetSupportedLanguages()
        {
            // Check what translations are available in program dir
            var translationDirectories = Program.AssemblyLocation.GetDirectories()
                .Where(x =>
                {
                    if (x.Name.Length < 2)
                        return false;
                    try
                    {
                        return x.GetFiles("BCUninstaller.resources.dll", SearchOption.TopDirectoryOnly).Any();
                    }
                    catch (SystemException e)
                    {
                        Console.WriteLine(e);
                        return false;
                    }
                })
                .Select(x => x.Name.Substring(0, 2).ToLower())
                .ToList();

            var supportedCultures = new[]
            {
                // en - English
                //("en-US"),
                "en-AU",
                "en-BZ",
                "en-CA",
                "en-IE",
                "en-JM",
                "en-NZ",
                "en-PH",
                "en-ZA",
                "en-TT",
                "en-GB",
                "en-ZW",

                // ar - Arabic
                "ar-DZ",
                "ar-BH",
                "ar-EG",
                "ar-IQ",
                "ar-JO",
                "ar-KW",
                "ar-LB",
                "ar-LY",
                "ar-MA",
                "ar-OM",
                "ar-QA",
                "ar-SA",
                "ar-SY",
                "ar-TN",
                "ar-AE",
                "ar-YE",

                // Czech
                "cs-CZ",

                // de - German
                "de-AT",
                "de-DE",
                "de-LI",
                "de-LU",
                "de-CH",

                // el - Greek
                "el-CY",
                "el-GR",

                // es - Spanish
                "es-AR",
                "es-BO",
                "es-CL",
                "es-CO",
                "es-CR",
                "es-DO",
                "es-EC",
                "es-SV",
                "es-GT",
                "es-HN",
                "es-MX",
                "es-NI",
                "es-PA",
                "es-PY",
                "es-PE",
                "es-PR",
                "es-ES",
                "es-UY",
                "es-VE",

                // fr - French
                "fr-BE",
                "fr-CA",
                "fr-FR",
                "fr-LU",
                "fr-MC",
                "fr-CH",

                // Hungarian
                "hu-HU",

                // id - Indonesian
                "id-ID",

                // it - Italian
                "it-IT",
                "it-CH",

                // ja - Japanese
                "ja-JP",

                // ko - Korean
                //"ko-KP", // North
                "ko-KR", // South

                // nl - Dutch
                "nl-NL",
                "nl-BE",

                // Polish
                "pl-PL",

                // pt - Portuguese
                "pt-PT",
                "pt-BR",

                // Russian
                "ru-RU",

                // Slovenian
                "sl-SI",

                // Swedish
                "sv-AX",
                "sv-FI",
                "sv-SE",

                // Turkish
                "tr-CY",
                "tr-TR",

                // Ukrainian
                "uk-UA",

                // Vietnamese
                "vi-VN",

                // Simplified Chinese
                "zh-Hans",

                // Traditional Chinese
                "zh-Hant"
            }.Attempt(CultureInfo.GetCultureInfo).ToList();

            supportedCultures.Add(EnUsCulture);

            //Debug.Assert(translationDirectories.All(x => supportedCultures.Select(c => c.Name.Substring(0, 2)).Contains(x, StringComparison.OrdinalIgnoreCase)),
            //    "Translation is not added to supported cultures - " + translationDirectories.FirstOrDefault(x => !supportedCultures.Select(c => c.Name.Substring(0, 2)).Contains(x, StringComparison.OrdinalIgnoreCase)));

            return supportedCultures.Where(x =>
            {
                var code = x.Name.Substring(0, 2).ToLower();
                return code.Equals("en", StringComparison.Ordinal) || translationDirectories.Contains(code, StringComparison.Ordinal);
            }).OrderBy(x => x.DisplayName).ToList().AsEnumerable();
        }

        public static void SetupCulture()
        {
            var currentCulture = CultureInfo.CurrentCulture;

            var targetLocale = Settings.Default.Language;
            if (targetLocale.IsNotEmpty())
            {
                try
                {
                    currentCulture = SupportedLanguages.First(x => x.Name.Equals(targetLocale));
                }
                catch
                {
                    Settings.Default.Language = string.Empty;
                }
            }

            if (!currentCulture.Name.ContainsAny(SupportedLanguages.Select(x => x.Parent.Name),
                StringComparison.OrdinalIgnoreCase))
                currentCulture = EnUsCulture;

            ProcessTools.SetDefaultCulture(currentCulture);
            var thread = Thread.CurrentThread;
            thread.CurrentCulture = currentCulture;
            thread.CurrentUICulture = currentCulture;
        }
    }
}
