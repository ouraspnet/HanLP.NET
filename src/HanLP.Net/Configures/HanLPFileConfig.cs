using System;
using System.Collections.Generic;
using System.Text;

namespace HanLP.Net.Configures
{
    public class HanLPFileConfig
    {
        /// <summary>
        /// 核心词典路径
        /// </summary>
        public string CoreDictionaryPath { get; set; } = "data/dictionary/CoreNatureDictionary.txt";

        /// <summary>
        /// 核心词典词性转移矩阵路径
        /// </summary>
        public string CoreDictionaryTransformMatrixDictionaryPath { get; set; } = "data/dictionary/CoreNatureDictionary.tr.txt";

        /// <summary>
        /// 用户自定义词典路径
        /// </summary>
        public string[] CustomDictionaryPath { get; set; } = new string[] { "data/dictionary/custom/CustomDictionary.txt" };

        /// <summary>
        /// 2元语法词典路径
        /// </summary>
        public string BiGramDictionaryPath { get; set; } = "data/dictionary/CoreNatureDictionary.ngram.txt";

        /// <summary>
        /// 停用词词典路径
        /// </summary>
        public string CoreStopWordDictionaryPath { get; set; } = "data/dictionary/stopwords.txt";

        /// <summary>
        /// 同义词词典路径
        /// </summary>
        public string CoreSynonymDictionaryDictionaryPath { get; set; } = "data/dictionary/synonym/CoreSynonym.txt";

        /// <summary>
        /// 人名词典路径
        /// </summary>
        public string PersonDictionaryPath { get; set; } = "data/dictionary/person/nr.txt";

        /// <summary>
        /// 人名词典转移矩阵路径
        /// </summary>
        public string PersonDictionaryTrPath { get; set; } = "data/dictionary/person/nr.tr.txt";

        /// <summary>
        /// 地名词典路径
        /// </summary>
        public string PlaceDictionaryPath { get; set; } = "data/dictionary/place/ns.txt";

        /// <summary>
        /// 地名词典转移矩阵路径
        /// </summary>
        public string PlaceDictionaryTrPath { get; set; } = "data/dictionary/place/ns.tr.txt";

        /// <summary>
        /// 地名词典路径
        /// </summary>
        public string OrganizationDictionaryPath { get; set; } = "data/dictionary/organization/nt.txt";

        /// <summary>
        /// 地名词典转移矩阵路径
        /// </summary>
        public string OrganizationDictionaryTrPath { get; set; } = "data/dictionary/organization/nt.tr.txt";

        /// <summary>
        /// 简繁转换词典根目录
        /// </summary>
        public string tcDictionaryRoot { get; set; } = "data/dictionary/tc/";

        /// <summary>
        /// 声母韵母语调词典
        /// </summary>
        public string SYTDictionaryPath { get; set; } = "data/dictionary/pinyin/SYTDictionary.txt";

        /// <summary>
        /// 拼音词典路径
        /// </summary>
        public string PinyinDictionaryPath { get; set; } = "data/dictionary/pinyin/pinyin.txt";

        /// <summary>
        /// 音译人名词典
        /// </summary>
        public string TranslatedPersonDictionaryPath { get; set; } = "data/dictionary/person/nrf.txt";

        /// <summary>
        /// 日本人名词典路径
        /// </summary>
        public string JapanesePersonDictionaryPath { get; set; } = "data/dictionary/person/nrj.txt";

        /// <summary>
        /// 字符类型对应表
        /// </summary>
        public string CharTypePath { get; set; } = "data/dictionary/other/CharType.dat.yes";

        /// <summary>
        /// 字符正规化表（全角转半角，繁体转简体）
        /// </summary>
        public string CharTablePath { get; set; } = "data/dictionary/other/CharTable.txt";

        /// <summary>
        /// 词-词性-依存关系模型
        /// </summary>
        public string WordNatureModelPath { get; set; } = "data/model/dependency/WordNature.txt";

        /// <summary>
        /// 最大熵-依存关系模型
        /// </summary>
        public string MaxEntModelPath { get; set; } = "data/model/dependency/MaxEntModel.txt";

        /// <summary>
        /// 神经网络依存模型路径
        /// </summary>
        public string NNParserModelPath { get; set; } = "data/model/dependency/NNParserModel.txt";

        /// <summary>
        /// CRF分词模型
        /// </summary>
        public string CRFSegmentModelPath { get; set; } = "data/model/segment/CRFSegmentModel.txt";

        /// <summary>
        /// HMM分词模型
        /// </summary>
        public string HMMSegmentModelPath { get; set; } = "data/model/segment/HMMSegmentModel.bin";

        /// <summary>
        /// CRF依存模型
        /// </summary>
        public string CRFDependencyModelPath { get; set; } = "data/model/dependency/CRFDependencyModelMini.txt";

        /// <summary>
        /// 分词结果是否展示词性
        /// </summary>
        public bool ShowTermNature { get; set; } = true;

        /// <summary>
        /// 是否执行字符正规化（繁体->简体，全角->半角，大写->小写），切换配置后必须删CustomDictionary.txt.bin缓存
        /// </summary>
        public bool Normalization { get; set; } = false;
    }
}
