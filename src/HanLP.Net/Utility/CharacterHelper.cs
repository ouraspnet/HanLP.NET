using System;
using System.Text;
using HanLP.Net.Fix;

namespace HanLP.Net.Utility
{
    /// <summary>
    /// 字符集识别辅助工具类
    /// </summary>
    public class CharacterHelper
    {
        public static bool IsSpaceLetter(char input) {
            return input == 8 || input == 9
                    || input == 10 || input == 13
                    || input == 32 || input == 160;
        }

        public static bool IsEnglishLetter(char input) {
            return (input >= 'a' && input <= 'z')
                    || (input >= 'A' && input <= 'Z');
        }

        public static bool IsArabicNumber(char input) {
            return input >= '0' && input <= '9';
        }

        public static bool IsCJKCharacter(char input) {
            Character.UnicodeBlock ub = Character.UnicodeBlock.Of(input);
            if (ub == Character.UnicodeBlock.CJK_UNIFIED_IDEOGRAPHS
                    || ub == Character.UnicodeBlock.CJK_COMPATIBILITY_IDEOGRAPHS
                    || ub == Character.UnicodeBlock.CJK_UNIFIED_IDEOGRAPHS_EXTENSION_A
                    //全角数字字符和日韩字符
                    || ub == Character.UnicodeBlock.HALFWIDTH_AND_FULLWIDTH_FORMS
                    //韩文字符集
                    || ub == Character.UnicodeBlock.HANGUL_SYLLABLES
                    || ub == Character.UnicodeBlock.HANGUL_JAMO
                    || ub == Character.UnicodeBlock.HANGUL_COMPATIBILITY_JAMO
                    //日文字符集
                    || ub == Character.UnicodeBlock.HIRAGANA //平假名
                    || ub == Character.UnicodeBlock.KATAKANA //片假名
                    || ub == Character.UnicodeBlock.KATAKANA_PHONETIC_EXTENSIONS
                    ) {
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// 进行字符规格化（全角转半角，大写转小写处理）
        /// </summary>
        public static char Regularize(char input) {
            if (input == 12288) {
                input = (char)32;

            }
            else if (input > 65280 && input < 65375) {
                input = (char)(input - 65248);

            }
            else if (input >= 'A' && input <= 'Z') {
                input += (char)32;
            }

            return input;
        }
    }
}
