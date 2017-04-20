using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HanLP.Net.Corpus.IO;
using HanLP.Net.Fix;

namespace HanLP.Net.Collection.Trie
{
    /// <summary>
    /// 双数组Trie树
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleArrayTrie<T> : ITrie<T>
    {
        private const int BUF_SIZE = 16384;
        private const int UNIT_SIZE = 8; // size of int + int

        private class Node
        {
            public int code;
            public int depth;
            public int left;
            public int right;

            public override string ToString() {
                return $"Node{{code={code}, depth={depth}, left={left}, right={right}}}";
            }
        }

        protected int[] check;
        protected int[] bases;

        private BitSet used;

        /// <summary>
        /// base 和 check 的大小
        /// </summary>
        protected int size;

        private int allocSize;
        private List<string> key;
        private int keySize;
        private int[] length;
        private int[] value;
        protected T[] v;
        private int progress;
        private int nextCheckPos;
        private int error_;

        private int Resize(int newSize) {
            int[] base2 = new int[newSize];
            int[] check2 = new int[newSize];
            if (allocSize > 0) {
                Array.Copy(bases, 0, base2, 0, allocSize);
                Array.Copy(check, 0, check2, 0, allocSize);
            }

            bases = base2;
            check = check2;

            return allocSize = newSize;
        }

        /// <summary>
        /// 获取直接相连的子节点
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="siblings">（子）兄弟节点</param>
        /// <returns>兄弟节点个数</returns>
        private int Fetch(Node parent, List<Node> siblings) {
            if (error_ < 0)
                return 0;

            int prev = 0;

            for (int i = parent.left; i < parent.right; i++) {
                if ((length != null ? length[i] : key[i].Length) < parent.depth)
                    continue;

                string tmp = key[i];

                int cur = 0;
                if ((length != null ? length[i] : tmp.Length) != parent.depth)
                    cur = (int)tmp[(parent.depth)] + 1;

                if (prev > cur) {
                    error_ = -3;
                    return 0;
                }

                if (cur != prev || siblings.Count == 0) {
                    Node tmp_node = new Node();
                    tmp_node.depth = parent.depth + 1;
                    tmp_node.code = cur;
                    tmp_node.left = i;
                    if (siblings.Count != 0)
                        siblings[(siblings.Count - 1)].right = i;

                    siblings.Add(tmp_node);
                }

                prev = cur;
            }

            if (siblings.Count != 0)
                siblings[(siblings.Count - 1)].right = parent.right;

            return siblings.Count;
        }

        private int Insert(List<Node> siblings) {
            if (error_ < 0) return 0;

            int begin = 0;
            int pos = Math.Max(siblings[0].code + 1, nextCheckPos) - 1;
            int nonzero_num = 0;
            int first = 0;

            if (allocSize <= pos)
                Resize(pos + 1);

            outer:
            // 此循环体的目标是找出满足base[begin + a1...an]  == 0的n个空闲空间,a1...an是siblings中的n个节点
            while (true) {
                pos++;

                if (allocSize <= pos) Resize(pos + 1);

                if (check[pos] != 0) {
                    nonzero_num++;
                    continue;
                }
                else if (first == 0) {
                    nextCheckPos = pos;
                    first = 1;
                }

                begin = pos - siblings[0].code; // 当前位置离第一个兄弟节点的距离
                if (allocSize <= (begin + siblings[(siblings.Count - 1)].code)) {
                    Resize(begin + siblings[(siblings.Count - 1)].code + Character.MAX_VALUE);
                }

                if (used.Get(begin)) continue;

                for (int i = 1; i < siblings.Count; i++)
                    if (check[begin + siblings[i].code] != 0)
                        goto outer;
                break;
            }

            // -- Simple heuristics --
            // if the percentage of non-empty contents in check between the
            // index
            // 'next_check_pos' and 'check' is greater than some constant value
            // (e.g. 0.9),
            // new 'next_check_pos' index is written by 'check'.
            if (1.0 * nonzero_num / (pos - nextCheckPos + 1) >= 0.95)
                nextCheckPos = pos; // 从位置 next_check_pos 开始到 pos 间，如果已占用的空间在95%以上，下次插入节点时，直接从 pos 位置处开始查找

            //used[begin] = true;
            used.Set(begin, true);

            size = (size > begin + siblings[(siblings.Count - 1)].code + 1) ? size
                    : begin + siblings[(siblings.Count - 1)].code + 1;

            for (int i = 0; i < siblings.Count; i++) {
                check[begin + siblings[i].code] = begin;
            }

            for (int i = 0; i < siblings.Count; i++) {
                var new_siblings = new List<Node>();

                if (Fetch(siblings[i], new_siblings) == 0)  // 一个词的终止且不为其他词的前缀
                {
                    bases[begin + siblings[i].code] = (value != null)
                        ? (-value[siblings[i].left] - 1)
                        : (-siblings[i].left - 1);

                    if (value != null && (-value[siblings[i].left] - 1) >= 0) {
                        error_ = -2;
                        return 0;
                    }
                    progress++;
                }
                else {
                    int h = Insert(new_siblings);   // dfs
                    bases[begin + siblings[i].code] = h;
                }
            }
            return begin;
        }

        public DoubleArrayTrie() {
            check = null;
            bases = null;
            used = new BitSet();
            size = 0;
            allocSize = 0;
            error_ = 0;
        }

        private void Clear() {
            check = null;
            bases = null;
            used = null;
            allocSize = 0;
            size = 0;
        }

        public int UnitSize { get { return UNIT_SIZE; } }

        public int Size { get { return size; } }

        public int TotalSize { get { return size * UNIT_SIZE; } }

        public int NonzeroSize { get { return check.Count(x => x != 0); } }

        public int Build(List<string> keys, List<T> value) {
            return Build(keys, value.ToArray());
        }

        public int Build(List<string> keys, T[] value) {
            Debug.Assert(keys.Count == value.Length, "键的个数与值的个数不一样！");
            Debug.Assert(keys.Count > 0, "键值个数为0！");
            v = value;
            return Build(keys, null, null, keys.Count);
        }

        /// <summary>
        /// 构建DAT
        /// </summary>
        /// <param name="keyValueList">意此entrySet一定要是字典序的！否则会失败</param>
        public int Build(List<KeyValuePair<string, T>> keyValueList) {
            var keyList = new List<string>(keyValueList.Count);
            var valueList = new List<T>(keyValueList.Count);
            foreach (var item in keyValueList) {
                keyList.Add(item.Key);
                valueList.Add(item.Value);
            }
            return Build(keyList, valueList);
        }

        /// <summary>
        /// 方便地构造一个双数组trie树
        /// </summary>
        /// <param name="keyValueDic">升序键值对map</param>
        /// <returns></returns>
        public int Build(SortedDictionary<string, T> keyValueDic) {
            Debug.Assert(keyValueDic != null);
            var keyValueList = keyValueDic.ToList();
            return Build(keyValueList);
        }

        /// <summary>
        /// 唯一的构建方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <param name="keySize"></param>
        /// <returns></returns>
        public int Build(List<string> key, int[] length, int[] value, int keySize) {
            if (key == null || keySize > key.Count) return 0;

            this.key = key;
            this.length = length;
            this.keySize = keySize;
            this.value = value;
            this.progress = 0;

            Resize(65536 * 32); // 32个双字节

            bases[0] = 1;
            nextCheckPos = 0;

            Node rootNode = new Node() {
                left = 0,
                right = this.keySize,
                depth = 0
            };
            List<Node> siblings = new List<Node>();

            Fetch(rootNode, siblings);

            Insert(siblings);

            used = null;
            this.key = null;
            this.length = null;

            return error_;
        }

        public void Open(string fileName) {
            var file = File.Create(fileName);
            size = (int)file.Length / UNIT_SIZE;
            check = new int[size];
            bases = new int[size];
            var sourceStream = new FileStream(fileName, FileMode.Open);
            using (var sr = new StreamReader(sourceStream))
            using (var reader = new BinaryReader(sr.BaseStream)) {
                for (int i = 0; i < size; i++) {
                    bases[i] = ReadInt(reader);
                    check[i] = ReadInt(reader);
                }
            }
        }

        private int ReadInt(BinaryReader reader) {
            int ch1 = reader.Read();
            int ch2 = reader.Read();
            int ch3 = reader.Read();
            int ch4 = reader.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
                throw new EndOfStreamException();
            return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + (ch4 << 0));
        }

        public bool Save(string fileName) {
            try {

                StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.Create));
                BinaryWriter writer = new BinaryWriter(sw.BaseStream);
                writer.Write(size);
                for (int i = 0; i < size; i++) {
                    writer.Write(bases[i]);
                    writer.Write(check[i]);
                }
                writer.Dispose();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将base和check保存下来
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public bool Save(Stream stream) {
            try {
                using (var writer = new StreamWriter(stream)) {
                    writer.Write(size);
                    for (int i = 0; i < size; i++) {
                        writer.Write(bases[i]);
                        writer.Write(check[i]);
                    }
                }
            }
            catch (Exception) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 从磁盘加载，需要额外提供值
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Load(string path, List<T> value) {
            if (!LoadBaseAndCheck(path)) return false;
            v = value.ToArray();
            return true;
        }

        public bool Load(ByteArray byteArray, T[] value) {
            if (byteArray == null) return false;
            size = byteArray.NextInt();
            bases = new int[size + 65535];   // 多留一些，防止越界
            check = new int[size + 65535];
            for (int i = 0; i < size; i++) {
                bases[i] = byteArray.NextInt();
                check[i] = byteArray.NextInt();
            }
            v = value;
            used = null;    // 无用的对象,释放掉
            return true;
        }

        public bool Load(string path) {
            return LoadBaseAndCheck(path);
        }

        private bool LoadBaseAndCheck(string path) {
            try {
                var sourceStream = new FileStream(path, FileMode.Open);
                using (var sr = new StreamReader(sourceStream))
                using (var reader = new BinaryReader(sr.BaseStream)) {
                    size = ReadInt(reader);
                    bases = new int[size + 65535];
                    check = new int[size + 65535];
                    for (int i = 0; i < size; i++) {
                        bases[i] = ReadInt(reader);
                        check[i] = ReadInt(reader);
                    }
                } 
            }
            catch (Exception) {
                return false;
            }
            return true;
        }

        public bool SerializeTo(string path) {
            try {
                using (var writer = IOUtil.NewOutputStream(path, BUF_SIZE)) {
                    writer.Write(this);
                }
            }
            catch (Exception) {
                return false;
            }
            return true;
        }

        public static DoubleArrayTrie<T> UnSerialize(string path) {
            //BinaryFormatter
            //using (var reader = IOUtil.NewInputStream(path, BUF_SIZE)) {
            //     reader.Read()
            //}
            //TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// 精确匹配
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int ExactMatchSearch(string key) {
            return ExactMatchSearch(key, 0, 0, 0);
        }

        public int ExactMatchSearch(string key, int pos, int len, int nodePos) {
            if (len <= 0)
                len = key.Length;
            if (nodePos <= 0)
                nodePos = 0;
            return ExactMatchSearch(key.ToArray(), pos, len, nodePos);
        }

        /// <summary>
        /// 精确查询
        /// </summary>
        /// <param name="keyChars">键的char数组</param>
        /// <param name="pos">char数组的起始位置</param>
        /// <param name="len">键的长度</param>
        /// <param name="nodePos">开始查找的位置（本参数允许从非根节点查询）</param>
        /// <returns>查到的节点代表的value ID，负数表示不存在</returns>
        public int ExactMatchSearch(char[] keyChars, int pos, int len, int nodePos) {
            int result = -1;

            int b = bases[nodePos];
            int p;

            for (int i = pos; i < len; i++) {
                p = b + keyChars[i] + 1;
                if (b == check[p])
                    b = bases[p];
                else
                    return result;
            }

            p = b;
            int n = bases[p];
            if (b == check[p] && n < 0) {
                result = -n - 1;
            }
            return result;
        }

        public List<int> CommonPrefixSearch(string key) {
            return CommonPrefixSearch(key, 0, 0, 0);
        }

        /// <summary>
        /// 前缀查询
        /// </summary>
        /// <param name="key">查询字串</param>
        /// <param name="pos">字串的开始位置</param>
        /// <param name="len">字串长度</param>
        /// <param name="nodePos">base中的开始位置</param>
        /// <returns>一个含有所有下标的list</returns>
        public List<int> CommonPrefixSearch(string key, int pos, int len, int nodePos) {
            if (len <= 0)
                len = key.Length;
            if (nodePos <= 0)
                nodePos = 0;

            List<int> result = new List<int>();

            char[] keyChars = key.ToArray();

            int b = bases[nodePos];
            int n;
            int p;

            for (int i = pos; i < len; i++) {
                p = b + keyChars[i] + 1;    // 状态转移 p = base[char[i-1]] + char[i] + 1
                if (b == check[p])                  // base[char[i-1]] == check[base[char[i-1]] + char[i] + 1]
                    b = bases[p];
                else
                    return result;
                p = b;
                n = bases[p];
                if (b == check[p] && n < 0)         // base[p] == check[p] && base[p] < 0 查到一个词
                {
                    result.Add(-n - 1);
                }
            }

            return result;
        }

        public LinkedList<KeyValuePair<string, T>> CommonPrefixSearchWithValue(string key) {
            return CommonPrefixSearchWithValue(key.ToArray());
        }

        /// <summary>
        /// 前缀查询，包含值
        /// </summary>
        public LinkedList<KeyValuePair<string, T>> CommonPrefixSearchWithValue(char[] keyChars, int begin = 0) {
            var result = new LinkedList<KeyValuePair<string, T>>();

            int b = bases[0];
            int n;
            int p;

            for (int i = begin; i < keyChars.Length; ++i) {
                p = b;
                n = bases[p];
                if (b == check[p] && n < 0)         // base[p] == check[p] && base[p] < 0 查到一个词
                {
                    result.AddLast(new KeyValuePair<string, T>(new string(keyChars, begin, i - begin), v[-n - 1]));
                }

                p = b + (int)(keyChars[i]) + 1;    // 状态转移 p = base[char[i-1]] + char[i] + 1
                                                   // 下面这句可能产生下标越界，不如改为if (p < size && b == check[p])，或者多分配一些内存
                if (b == check[p])                  // base[char[i-1]] == check[base[char[i-1]] + char[i] + 1]
                    b = bases[p];
                else
                    return result;
            }

            p = b;
            n = bases[p];

            if (b == check[p] && n < 0) {
                result.AddLast(new KeyValuePair<string, T>(new string(keyChars, begin, keyChars.Length - begin), v[-n - 1]));
            }

            return result;
        }

        public override string ToString() {
            return "DoubleArrayTrie{" +
               "size=" + size +
               ", allocSize=" + allocSize +
               ", key=" + key +
               ", keySize=" + keySize +
               ", progress=" + progress +
               ", nextCheckPos=" + nextCheckPos +
               ", error_=" + error_ +
               '}';
        }

        /// <summary>
        /// 树叶子节点个数
        /// </summary>
        /// <returns></returns>
        public int Length { get { return v.Length; } }

        /// <summary>
        /// 获取check数组引用
        /// </summary>
        public int[] Check { get { return check; } }

        /// <summary>
        /// 获取base数组引用
        /// </summary>
        public int[] Bases { get { return bases; } }

        /// <summary>
        /// 获取index对应的值
        /// </summary>
        public T this[int index] {
            get { return v[index]; }
            set { v[index] = value; }
        }

        /// <summary>
        /// 精确查询
        /// </summary>
        public T Get(string key) {
            int index = ExactMatchSearch(key);
            if (index >= 0) {
                return this[index];
            }
            return default(T);
        }

        /// <summary>
        /// 精确查询
        /// </summary>
        public T Get(char[] key) {
            int index = ExactMatchSearch(key, 0, key.Length, 0);
            if (index >= 0) {
                return this[index];
            }
            return default(T);
        }

        public T[] GetValueArray(T[] a) {
            int size = v.Length;
            if (a.Length < size) {
                a = new T[size];
            }
            Array.Copy(v, 0, a, 0, size);
            return a;
        }

        public bool ContainsKey(string key) {
            return ExactMatchSearch(key) >= 0;
        }

        /// <summary>
        /// 沿着路径转移状态
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected int Transition(string path) {
            return Transition(path.ToArray());
        }

        protected int Transition(char[] path) {
            int b = bases[0];
            int p;

            for (int i = 0; i < path.Length; ++i) {
                p = b + (int)(path[i]) + 1;
                if (b == check[p])
                    b = bases[p];
                else
                    return -1;
            }

            p = b;
            return p;
        }

        public int Transition(string path, int from) {
            int b = from;
            int p;

            for (int i = 0; i < path.Length; ++i) {
                p = b + (int)(path[i]) + 1;
                if (b == check[p])
                    b = bases[p];
                else
                    return -1;
            }

            p = b;
            return p;
        }

        public int Transition(char c, int from) {
            int b = from;
            int p;

            p = b + (int)(c) + 1;
            if (b == check[p])
                b = bases[p];
            else
                return -1;

            return b;
        }

        public T Output(int state) {
            if (state < 0) return default(T);
            int n = bases[state];
            if (state == check[state] && n < 0) {
                return v[-n - 1];
            }
            return default(T);
        }

        public DoubleArrayTrieSearcher<T> GetSearcher(string text, int offset) {
            return new DoubleArrayTrieSearcher<T>(offset, text.ToCharArray());
        }

        public DoubleArrayTrieSearcher<T> GetSearcher(char[] text, int offset) {
            return new DoubleArrayTrieSearcher<T>(offset, text);
        }
    }
}