using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using HanLP.Net.Collection.Trie;
using HanLP.Net.Corpus.IO;
using HanLP.Net.Corpus.Tag;
using HanLP.Net.Utility;

namespace HanLP.Net.Dictionary
{
    public static class CoreDictionary
    {
        public const int TOTAL_FREQUENCY = 221894;

        public static string path = HanLP.Config.CoreDictionaryPath;

        internal static Attribute Get(string word) {
            throw new NotImplementedException();
        }

        public static DoubleArrayTrie<Attribute> Trie { get; private set; }

        static CoreDictionary() {
            Trie = new DoubleArrayTrie<Attribute>();
            var watch = Stopwatch.StartNew();
            if (!Load(path)) {
                Predefine.logger.Error("核心词典" + path + "加载失败");
            }
            else {
                watch.Stop();
                Predefine.logger.Info(path + "加载成功，" + Trie.Length + "个词条，耗时" + watch.ElapsedMilliseconds + "ms");
            }
        }

        private static bool Load(string path) {
            Predefine.logger.Info("核心词典开始加载:" + path);
            if (LoadDat(path)) return true;
            return ReadAndSaveCoreDicFile(path);
        }

        private static bool LoadDat(string path) {
            try {
                ByteArray byteArray = ByteArray.CreateByteArray(path + Predefine.BIN_EXT);
                if (byteArray == null) return false;
                int size = byteArray.NextInt();
                var attributes = new Attribute[size];
                var natureIndexArray = Enum.GetValues(typeof(Nature)) as Nature[];
                for (int i = 0; i < size; ++i) {
                    // 第一个是全部频次，第二个是词性个数
                    int currentTotalFrequency = byteArray.NextInt();
                    int length = byteArray.NextInt();
                    attributes[i] = new Attribute(length) {
                        totalFrequency = currentTotalFrequency
                    };
                    for (int j = 0; j < length; ++j) {
                        attributes[i].nature[j] = natureIndexArray[byteArray.NextInt()];
                        attributes[i].frequency[j] = byteArray.NextInt();
                    }
                }
                if (!Trie.Load(byteArray, attributes) || byteArray.HasMore()) return false;
            }
            catch (Exception e) {
                Predefine.logger.Warn("读取失败，问题发生在" + e);
                return false;
            }
            return true;
        }

        private static bool ReadAndSaveCoreDicFile(string path) {
            if (!File.Exists(path)) {
                Predefine.logger.Warn("核心词典不存在");
                return false;
            }
            var dic = new SortedDictionary<string, Attribute>();

            Stopwatch watch = Stopwatch.StartNew();
            int maxFrequency = 0;
            foreach (var line in File.ReadAllLines(path, Encoding.UTF8)) {
                string[] param = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                int natureCount = (param.Length - 1) / 2;
                var att = new Attribute(natureCount);
                for (int i = 0; i < natureCount; ++i) {
                    att.nature[i] = (Nature)Enum.Parse(typeof(Nature), param[1 + 2 * i]);
                    att.frequency[i] = int.Parse(param[2 + 2 * i]);
                    att.totalFrequency += att.frequency[i];
                }
                dic.Add(param[0], att);
                maxFrequency += att.totalFrequency;
            }
            watch.Stop();

            Predefine.logger.Info($"核心词典读入词条{ dic.Count} ,全部频次{maxFrequency}, 耗时{watch.ElapsedMilliseconds}ms.");

            Trie.Build(dic);

            Predefine.logger.Info($"核心词典加载成功，下面将写入缓存……");

            return SaveAttribute(path, dic);
        }

        private static bool SaveAttribute(string path, SortedDictionary<string, Attribute> dic) {
            Action<BinaryWriter> addAttributeStatement = bw => {
                bw.Write(dic.Count);
                foreach (var item in dic.Values) {
                    bw.Write(item.totalFrequency);
                    bw.Write(item.nature.Length);
                    for (int i = 0; i < item.nature.Length; i++) {
                        bw.Write((int)item.nature[i]);
                        bw.Write(item.frequency[i]);
                    }
                }
            };

            try {
                var savePath = Path.Combine(path, Predefine.BIN_EXT);
                Trie.Save(savePath, addAttributeStatement);
            }
            catch (Exception e) {
                Predefine.logger.Warn("保存失败" + e.Message);
                return false;
            }
            return true;
        }


        public static Attribute Create(String natureWithFrequency) {
            try {
                string[] param = natureWithFrequency.Split(' ');
                int natureCount = param.Length / 2;
                Attribute attribute = new Attribute(natureCount);
                for (int i = 0; i < natureCount; ++i) {
                    attribute.nature[i] = LexiconUtility.convertStringToNature(param[2 * i], null);
                    attribute.frequency[i] = int.Parse(param[1 + 2 * i]);
                    attribute.totalFrequency += attribute.frequency[i];
                }
                return attribute;
            }
            catch (Exception e) {
                Predefine.logger.Warn("使用字符串" + natureWithFrequency + "创建词条属性失败！" + e.Message);
                return null;
            }
        }

    }

    [Serializable]
    public class Attribute
    {
        /// <summary>
        /// 词性列表
        /// </summary>
        public Nature[] nature;

        /// <summary>
        /// 词性对应的词频
        /// </summary>
        public int[] frequency;

        public int totalFrequency;

        public Attribute(int length) {
            nature = new Nature[length];
            frequency = new int[length];
        }

        public Attribute(Nature[] nature, int[] frequency) {
            this.nature = nature;
            this.frequency = frequency;
        }

        public Attribute(Nature nature, int frequency) : this(1) {
            this.nature[0] = nature;
            this.frequency[0] = frequency;
        }

        public Attribute(Nature[] nature, int[] frequency, int totalFrequency) {
            this.nature = nature;
            this.frequency = frequency;
            this.totalFrequency = totalFrequency;
        }

        public Attribute(Nature nature) : this(nature, 1000) {

        }

        public static Attribute Create(string natureWithFrequency) {
            try {
                string[] param = natureWithFrequency.Split(' ');
                int natureCount = param.Length / 2;
                var attribute = new Attribute(natureCount);
                for (int i = 0; i < natureCount; i++) {
                    //attribute[i] = 
                    attribute.frequency[i] = int.Parse(param[1 + 2 * i]);
                    attribute.totalFrequency += attribute.frequency[i];
                }
                return attribute;
            }
            catch (Exception e) {
                var a = e.Message; //TODO;
                                   //logger.Warn("使用字符串" + natureWithFrequency + "创建词条属性失败！" + TextUtility.exceptionToString(e))
                return null;
            }
        }

        /// <summary>
        ///  从字节流中加载
        /// </summary>
        public static Attribute Ccreate(ByteArray byteArray, Nature[] natureIndexArray) {

            int currentTotalFrequency = byteArray.NextInt();
            int length = byteArray.NextInt();

            var attribute = new Attribute(length) {
                totalFrequency = currentTotalFrequency
            };

            for (int j = 0; j < length; ++j) {
                attribute.nature[j] = natureIndexArray[byteArray.NextInt()];
                attribute.frequency[j] = byteArray.NextInt();
            }
            return attribute;
        }

        /// <summary>
        /// 获取词性的词频
        /// </summary>
        /// <param name="nature">字符串词性</param>
        /// <returns>词频</returns>
        public int GetNatureFrequency(string nature) {
            try {
                var pos = (Nature)Enum.Parse(typeof(Nature), nature);
                return GetNatureFrequency(pos);
            }
            catch (ArgumentException) {
                return 0;
            }
        }

        /// <summary>
        /// 获取词性的词频
        /// </summary>
        /// <param name="nature">词性</param>
        /// <returns>词频</returns>
        public int GetNatureFrequency(Nature nature) {
            int i = 0;
            foreach (Nature pos in this.nature) {
                if (nature == pos) {
                    return frequency[i];
                }
                ++i;
            }
            return 0;
        }


        /// <summary>
        /// 是否有某个词性
        /// </summary>
        public bool HasNature(Nature nature) {
            return GetNatureFrequency(nature) > 0;
        }

        /// <summary>
        /// 是否有以某个前缀开头的词性
        /// </summary>
        /// <param name="prefix">词性前缀，比如u会查询是否有ude, uzhe等等</param>
        /// <returns></returns>
        public bool HasNatureStartsWith(string prefix) {
            return this.nature.Any(x => x.ToString().StartsWith(prefix));
        }

        public override string ToString() {
            var sb = new StringBuilder();
            for (int i = 0; i < nature.Length; ++i) {
                sb.Append(nature[i]).Append(' ').Append(frequency[i]).Append(' ');
            }
            return sb.ToString();
        }

        public void Save(Stream outStream) {
            using (var writer = new BinaryWriter(outStream)) {
                writer.Write(totalFrequency);
                writer.Write(nature.Length);
                for (int i = 0; i < nature.Length; ++i) {
                    writer.Write((int)nature[i]);
                    writer.Write(frequency[i]);
                }
            }
        }
    }

}
