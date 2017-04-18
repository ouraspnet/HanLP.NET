using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using HanLP.Net.Configures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HanLP.Net
{
    public class HanLPConfiguration
    {
        public HanLPConfiguration(IConfiguration config, ILoggerFactory logFactory) {

            Configuration = config;
            LoggerFactory = logFactory;
            SetFileConfig();
        }

        public IConfiguration Configuration { get; set; }

        public ILoggerFactory LoggerFactory { get;  set; }

        public string DataRootPath { get; private set; }

        public HanLPFileConfig FileConfig { get; set; } = new HanLPFileConfig();


        protected virtual void SetFileConfig() {

            var section = Configuration.GetSection("HanLP.NET");
            string root = null;
            if (!string.IsNullOrEmpty(section["root"])) {
                root = Path.GetFullPath(section["root"]);
            } 
            else {
                root = Directory.GetCurrentDirectory();
            }
            DataRootPath = root;

            if (section != null) {

                FileConfig.CoreDictionaryPath = Path.Combine(root, string.IsNullOrEmpty(section["CoreDictionaryPath"]) ? FileConfig.CoreDictionaryPath : section["CoreDictionaryPath"]);
                FileConfig.CoreDictionaryTransformMatrixDictionaryPath = Path.Combine(root, string.IsNullOrEmpty(section["CoreDictionaryTransformMatrixDictionaryPath"]) ? FileConfig.CoreDictionaryTransformMatrixDictionaryPath : section["CoreDictionaryTransformMatrixDictionaryPath"]);
                FileConfig.BiGramDictionaryPath = Path.Combine(root, string.IsNullOrEmpty(section["BiGramDictionaryPath"]) ? FileConfig.BiGramDictionaryPath : section["BiGramDictionaryPath"]);
                FileConfig.CoreStopWordDictionaryPath = Path.Combine(root, string.IsNullOrEmpty(section["CoreStopWordDictionaryPath"]) ? FileConfig.CoreStopWordDictionaryPath : section["CoreStopWordDictionaryPath"]);
                FileConfig.CoreSynonymDictionaryDictionaryPath = Path.Combine(root, string.IsNullOrEmpty(section["CoreSynonymDictionaryDictionaryPath"]) ? FileConfig.CoreSynonymDictionaryDictionaryPath : section["CoreSynonymDictionaryDictionaryPath"]);
                FileConfig.PersonDictionaryPath = Path.Combine(root, string.IsNullOrEmpty(section["PersonDictionaryPath"]) ? FileConfig.PersonDictionaryPath : section["PersonDictionaryPath"]);
                FileConfig.PersonDictionaryTrPath = Path.Combine(root, string.IsNullOrEmpty(section["PersonDictionaryTrPath"]) ? FileConfig.PersonDictionaryTrPath : section["PersonDictionaryTrPath"]);

                var value = section["PersonDictionaryTrPath"];
                if (string.IsNullOrEmpty(value)) {
                    value = "data/dictionary/custom/CustomDictionary.txt";
                }
                string[] pathArray = value.Split(';');
                String prePath = root;
                for (int i = 0; i < pathArray.Length; ++i) {
                    if (pathArray[i].StartsWith(" ")) {
                        pathArray[i] = prePath + pathArray[i].Trim();
                    }
                    else {
                        pathArray[i] = root + pathArray[i];
                        int lastSplash = pathArray[i].LastIndexOf('/');
                        if (lastSplash != -1) {
                            prePath = pathArray[i].Substring(0, lastSplash + 1);
                        }
                    }
                }
                FileConfig.CustomDictionaryPath = pathArray;

                FileConfig.tcDictionaryRoot = Path.Combine(root, string.IsNullOrEmpty(section["tcDictionaryRoot"]) ? FileConfig.tcDictionaryRoot : section["tcDictionaryRoot"]);
                if (!FileConfig.tcDictionaryRoot.EndsWith("/")) FileConfig.tcDictionaryRoot += '/';
                FileConfig.SYTDictionaryPath = Path.Combine(root, string.IsNullOrEmpty(section["SYTDictionaryPath"]) ? FileConfig.SYTDictionaryPath : section["SYTDictionaryPath"]);
                FileConfig.PinyinDictionaryPath = Path.Combine(root, string.IsNullOrEmpty(section["PinyinDictionaryPath"]) ? FileConfig.PinyinDictionaryPath : section["PinyinDictionaryPath"]);
                FileConfig.TranslatedPersonDictionaryPath = Path.Combine(root, string.IsNullOrEmpty(section["TranslatedPersonDictionaryPath"]) ? FileConfig.TranslatedPersonDictionaryPath : section["TranslatedPersonDictionaryPath"]);
                FileConfig.JapanesePersonDictionaryPath = Path.Combine(root, string.IsNullOrEmpty(section["JapanesePersonDictionaryPath"]) ? FileConfig.JapanesePersonDictionaryPath : section["JapanesePersonDictionaryPath"]);
                FileConfig.PlaceDictionaryPath = Path.Combine(root, string.IsNullOrEmpty(section["PlaceDictionaryPath"]) ? FileConfig.PlaceDictionaryPath : section["PlaceDictionaryPath"]);
                FileConfig.PlaceDictionaryTrPath = Path.Combine(root, string.IsNullOrEmpty(section["PlaceDictionaryTrPath"]) ? FileConfig.PlaceDictionaryTrPath : section["PlaceDictionaryTrPath"]);
                FileConfig.OrganizationDictionaryPath = Path.Combine(root, string.IsNullOrEmpty(section["OrganizationDictionaryPath"]) ? FileConfig.OrganizationDictionaryPath : section["OrganizationDictionaryPath"]);
                FileConfig.OrganizationDictionaryTrPath = Path.Combine(root, string.IsNullOrEmpty(section["OrganizationDictionaryTrPath"]) ? FileConfig.OrganizationDictionaryTrPath : section["OrganizationDictionaryTrPath"]);
                FileConfig.CharTypePath = Path.Combine(root, string.IsNullOrEmpty(section["CharTypePath"]) ? FileConfig.CharTypePath : section["CharTypePath"]);
                FileConfig.CharTablePath = Path.Combine(root, string.IsNullOrEmpty(section["CharTablePath"]) ? FileConfig.CharTablePath : section["CharTablePath"]);
                FileConfig.WordNatureModelPath = Path.Combine(root, string.IsNullOrEmpty(section["WordNatureModelPath"]) ? FileConfig.WordNatureModelPath : section["WordNatureModelPath"]);
                FileConfig.MaxEntModelPath = Path.Combine(root, string.IsNullOrEmpty(section["MaxEntModelPath"]) ? FileConfig.MaxEntModelPath : section["MaxEntModelPath"]);
                FileConfig.NNParserModelPath = Path.Combine(root, string.IsNullOrEmpty(section["NNParserModelPath"]) ? FileConfig.NNParserModelPath : section["NNParserModelPath"]);
                FileConfig.CRFSegmentModelPath = Path.Combine(root, string.IsNullOrEmpty(section["CRFSegmentModelPath"]) ? FileConfig.CRFSegmentModelPath : section["CRFSegmentModelPath"]);
                FileConfig.CRFDependencyModelPath = Path.Combine(root, string.IsNullOrEmpty(section["CRFDependencyModelPath"]) ? FileConfig.CRFDependencyModelPath : section["CRFDependencyModelPath"]);
                FileConfig.HMMSegmentModelPath = Path.Combine(root, string.IsNullOrEmpty(section["HMMSegmentModelPath"]) ? FileConfig.HMMSegmentModelPath : section["HMMSegmentModelPath"]);
                if (!string.IsNullOrEmpty(section["ShowTermNature"])) {
                    FileConfig.ShowTermNature = section["ShowTermNature"] == "true";
                }
                if (!string.IsNullOrEmpty(section["Normalization"])) {
                    FileConfig.Normalization = section["Normalization"] == "true";
                }
            }
        }
    }
}
