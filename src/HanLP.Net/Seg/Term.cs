using System;
using System.Collections.Generic;
using System.Text;
using HanLP.Net.Corpus.Tag;
using HanLP.Net.Utility;

namespace HanLP.Net.Seg
{
    /// <summary>
    /// 一个单词，用户可以直接访问此单词的全部属性
    /// </summary>
    public class Term
    {
        /// <summary>
        /// 词语
        /// </summary>
        private string word;

        /// <summary>
        /// 词性
        /// </summary>
        private Nature nature;

        /// <summary>
        /// 在文本中的起始位置（需开启分词器的offset选项）
        /// </summary>
        public int offset;

        /// <summary>
        /// 构造一个单词
        /// </summary>
        public Term(string word, Nature nature) {
            this.Word = word;
            this.nature = nature;
        }


        public override string ToString() {
            if (HanLP.Config.ShowTermNature)
                return Word + "/" + nature;
            return Word;
        }

        public int Length { get { return Word.Length; } }

        /// <summary>
        /// 词语
        /// </summary>
        public string Word { get => word; set => word = value; }

        /// <summary>
        /// 词性
        /// </summary>
        public Nature Nature { get => nature; set => nature = value; }

        /// <summary>
        /// 获取本词语在HanLP词库中的频次
        /// </summary>
        /// <returns>频次，0代表这是个OOV</returns>
        public int GetFrequency() {
            // return LexiconUtility.GetFrequency(Word);
            throw new NotImplementedException();
        }
    }

}
