namespace HanLP.Net.Collection.Trie
{
    public class DoubleArrayTrieSearcher<T>
    {
        private DoubleArrayTrie<T> _arrayTrie;

        public DoubleArrayTrieSearcher(DoubleArrayTrie<T> arrayTrie) {
            _arrayTrie = arrayTrie;
        }

        /// <summary>
        /// key的起点
        /// </summary>
        public int begin;

        /// <summary>
        /// key的长度
        /// </summary>
        public int length;

        /// <summary>
        /// key的字典序坐标
        /// </summary>
        public int index;

        /// <summary>
        /// key对应的value
        /// </summary>
        public T value;

        /// <summary>
        /// 传入的字符数组
        /// </summary>
        private char[] charArray;

        /// <summary>
        /// 上一个node位置
        /// </summary>
        private int last;

        /// <summary>
        /// 上一个字符的下标
        /// </summary>
        private int i;

        /// <summary>
        /// charArray的长度，效率起见，开个变量
        /// </summary>
        private int arrayLength;

        /**
         * 构造一个双数组搜索工具
         *
         * @param offset    搜索的起始位置
         * @param charArray 搜索的目标字符数组
         */

        public DoubleArrayTrieSearcher(int offset, char[] charArray) {
            this.charArray = charArray;
            i = offset;
            last = _arrayTrie.Bases[0];
            arrayLength = charArray.Length;
            // A trick，如果文本长度为0的话，调用next()时，会带来越界的问题。
            // 所以我要在第一次调用next()的时候触发begin == arrayLength进而返回false。
            // 当然也可以改成begin >= arrayLength，不过我觉得操作符>=的效率低于==
            if (arrayLength == 0) begin = -1;
            else begin = offset;
        }

        /// <summary>
        /// 取出下一个命中输出
        /// </summary>
        /// <returns></returns>
        public bool Next() {
            int b = last;
            int n;
            int p;

            for (; ; ++i) {
                if (i == arrayLength)               // 指针到头了，将起点往前挪一个，重新开始，状态归零
                {
                    ++begin;
                    if (begin == arrayLength) break;
                    i = begin;
                    b = _arrayTrie.Bases[0];
                }
                p = b + (int)(charArray[i]) + 1;   // 状态转移 p = base[char[i-1]] + char[i] + 1
                if (b == _arrayTrie.Check[p])                  // base[char[i-1]] == check[base[char[i-1]] + char[i] + 1]
                    b = _arrayTrie.Bases[p];                    // 转移成功
                else {
                    i = begin;                      // 转移失败，也将起点往前挪一个，重新开始，状态归零
                    ++begin;
                    if (begin == arrayLength) break;
                    b = _arrayTrie.Bases[0];
                    continue;
                }
                p = b;
                n = _arrayTrie.Bases[p];
                if (b == _arrayTrie.Check[p] && n < 0)         // base[p] == check[p] && base[p] < 0 查到一个词
                {
                    length = i - begin + 1;
                    index = -n - 1;
                    value = _arrayTrie[index];
                    last = b;
                    ++i;
                    return true;
                }
            }

            return false;
        }
    }
}