using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;

namespace CardSurvival_LongNap
{

    public class LocalizationInfo
    {
        Dictionary<string, string> LocalizationLookup { get; } = new Dictionary<string, string>();


        /// <summary>
        /// Loads the data from the CSV file.  Expects "LocalizationKey","Text" format.
        /// Retains the data from the previous loads.
        /// </summary>
        /// <param name="csvFileName"></param>
        private void LoadCsv(string csvFileName)
        {
            Dictionary<string, List<string>> csvData = CSVParser.LoadFromPath(csvFileName);

            //Overwrite if there are dupes
            foreach (var csvRow in csvData) {
                //Transform any \n strings as new lines and load the text.
                LocalizationLookup[csvRow.Key] = csvRow.Value[0].Replace("\\n", "\n");
            }
        }

        /// <summary>
        /// Loads the file matching the game's Language.
        /// </summary>
        /// <param name="languageFilesDirectory">
        /// The directory that is expected to contain the localization files.</param>
        public void Load(string languageFilesDirectory)
        {
            string localizationFilePath = GetLanguageFilePath(languageFilesDirectory);

            if(File.Exists(localizationFilePath))
            {
                LoadCsv(localizationFilePath);
            }
            else
            {
                Plugin.Log.LogWarning($"Localization file not found.  Using defaults.  Expected file'{localizationFilePath}'");
            }
        }

        /// <summary>
        /// Returns the path to the file name for the game's current language.
        /// </summary>
        /// <param name="languageFilesDirectory"></param>
        /// <returns></returns>
        private string GetLanguageFilePath(string languageFilesDirectory)
        {

            string fileName;

            string languageName = LocalizationManager.Instance.Languages[LocalizationManager.CurrentLanguage].LanguageName;

            switch (languageName)
            {
                case "简体中文":
                    fileName = "zh.csv";
                    break;
                case "English":
                    fileName = "en.csv";
                    break;
                default:
                    //If not supported, try using the language name from the game as a fallback.

                    //Get the filename and .csv just in case the language has a directory separator. 
                    fileName = Path.Combine(Path.GetFileName(languageName), ".csv");
                    Plugin.Log.LogWarning($"Unknown language name.  Using ${fileName}");
                    break;
            }

            string fullPath = Path.Combine(languageFilesDirectory, fileName);
            return fullPath;
        }



        /// <summary>
        /// Updates the DefaultText fo the LocalizedString if there is a translation that matches the LocalizationKey.
        /// </summary>
        /// <param name="localizedString"></param>
        public void SetLocalization(ref LocalizedString localizedString)
        {
            if (string.IsNullOrWhiteSpace(localizedString.LocalizationKey))
            {
                //Do not update
                return;
            }

            string key = localizedString.LocalizationKey.Trim();

            if (LocalizationLookup.TryGetValue(key, out string text))
            {
                localizedString.DefaultText = text;
            }
            else
            {
                Plugin.Log.LogWarning($"Localization Key not found '{key}'");
            }

            //Otherwise, no change.

        }
    }
}
