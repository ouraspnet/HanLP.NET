using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NLog;

namespace HanLP.Net.Utility
{
    public static class Predefine
    {
        /// <summary>
        /// hanlp.properties的路径，一般情况下位于classpath目录中。
        /// 但在某些极端情况下（不标准的Java虚拟机，用户缺乏相关知识等），允许将其设为绝对路径
        /// </summary>
        public static string HANLP_PROPERTIES_PATH;
        public static double MIN_PROBABILITY = 1e-10;
        public static int CT_SENTENCE_BEGIN = 1;        //Sentence begin
        public static int CT_SENTENCE_END = 4;          //Sentence ending
        public static int CT_SINGLE = 5;                //SINGLE byte
        public static int CT_DELIMITER = CT_SINGLE + 1; //delimiter
        public static int CT_CHINESE = CT_SINGLE + 2;   //Chinese Char
        public static int CT_LETTER = CT_SINGLE + 3;    //HanYu Pinyin
        public static int CT_NUM = CT_SINGLE + 4;       //HanYu Pinyin
        public static int CT_INDEX = CT_SINGLE + 5;     //HanYu Pinyin
        public static int CT_OTHER = CT_SINGLE + 12;    //Other
                                                        /// <summary>
                                                        /// 浮点数正则
                                                        /// </summary>
        public static Regex PATTERN_FLOAT_NUMBER = new Regex("^(-?\\d+)(\\.\\d+)?$", RegexOptions.Compiled);

        public static string POSTFIX_SINGLE =
            "坝邦堡城池村单岛道堤店洞渡队峰府冈港阁宫沟国海号河湖环集江礁角街井郡坑口矿里岭楼路门盟庙弄牌派坡铺旗桥区渠泉山省市水寺塔台滩坛堂厅亭屯湾屋溪峡县线乡巷洋窑营屿园苑院闸寨站镇州庄族陂庵町";

        public static string[] POSTFIX_MUTIPLE = {"半岛","草原","城市","大堤","大公国","大桥","地区",
        "帝国","渡槽","港口","高速公路","高原","公路","公园","共和国","谷地","广场",
        "国道","海峡","胡同","机场","集镇","教区","街道","口岸","码头","煤矿",
        "牧场","农场","盆地","平原","丘陵","群岛","沙漠","沙洲","山脉","山丘",
        "水库","隧道","特区","铁路","新村","雪峰","盐场","盐湖","渔场","直辖市",
        "自治区","自治县","自治州"};

        //Translation type
        public static int TT_ENGLISH = 0;
        public static int TT_RUSSIAN = 1;
        public static int TT_JAPANESE = 2;

        //Seperator type
        public static string SEPERATOR_C_SENTENCE = "。！？：；…";
        public static string SEPERATOR_C_SUB_SENTENCE = "、，（）“”‘’";
        public static string SEPERATOR_E_SENTENCE = "!?:;";
        public static string SEPERATOR_E_SUB_SENTENCE = ",()*'";
        //注释：原来程序为",()\042'"，"\042"为10进制42好ASC字符，为*
        public static string SEPERATOR_LINK = "\n\r 　";

        //Seperator between two words
        public static string WORD_SEGMENTER = "@";

        public static int CC_NUM = 6768;

        //The number of Chinese Char,including 5 empty position between 3756-3761
        public static int WORD_MAXLENGTH = 100;
        public static int WT_DELIMITER = 0;
        public static int WT_CHINESE = 1;
        public static int WT_OTHER = 2;

        public static int MAX_WORDS = 650;
        public static int MAX_SEGMENT_NUM = 10;

        public static int MAX_FREQUENCY = 25146057; // 现在总词频25146057

        public static Logger logger = LogManager.GetLogger("HanLP");

        /// <summary>
        /// Smoothing 平滑因子
        /// </summary>
        public static double dTemp = (double)1 / MAX_FREQUENCY + 0.00001;

        /// <summary>
        /// 平滑参数
        /// </summary>
        public static double dSmoothingPara = 0.1;

        /// <summary>
        /// 地址 ns
        /// </summary>
        public static string TAG_PLACE = "未##地";

        /// <summary>
        /// 句子的开始 begin
        /// </summary>
        public static string TAG_BIGIN = "始##始";

        /// <summary>
        /// 其它
        /// </summary>
        public static string TAG_OTHER = "未##它";

        /// <summary>
        /// 团体名词 nt
        /// </summary>
        public static string TAG_GROUP = "未##团";

        /// <summary>
        /// 数词 m
        /// </summary>
        public static string TAG_NUMBER = "未##数";

        /// <summary>
        /// 数量词 mq （现在觉得应该和数词同等处理，比如一个人和一人都是合理的）
        /// </summary>
        public static string TAG_QUANTIFIER = "未##量";

        /// <summary>
        /// 专有名词 nx
        /// </summary>
        public static string TAG_PROPER = "未##专";

        /// <summary>
        /// 时间 t
        /// </summary>
        public static string TAG_TIME = "未##时";

        /// <summary>
        /// 字符串 x
        /// </summary>
        public static string TAG_CLUSTER = "未##串";

        /// <summary>
        /// 结束 end
        /// </summary>
        public static string TAG_END = "末##末";

        /// <summary>
        /// 人名 nr
        /// </summary>
        public static string TAG_PEOPLE = "未##人";

        /// <summary>
        ///  trie树文件后缀名
        /// </summary>
        public static string TRIE_EXT = ".trie.dat";

        /// <summary>
        /// 值文件后缀名
        /// </summary>
        public static string VALUE_EXT = ".value.dat";

        /// <summary>
        ///  逆转后缀名
        /// </summary>
        public static string REVERSE_EXT = ".reverse";

        /// <summary>
        /// 二进制文件后缀
        /// </summary>
        public static string BIN_EXT = ".bin";

    }
}
