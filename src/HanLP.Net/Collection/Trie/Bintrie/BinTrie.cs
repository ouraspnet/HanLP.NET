using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HanLP.Net.Corpus.IO;

namespace HanLP.Net.Collection.Trie.Bintrie
{
    [Serializable]
    public class BinTrie<T> : Node<T>, ITrie<T>,ICollection<T>
    {
        private int _size;

        public int Length => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public BinTrie() {
            _child = new BaseNode<T>[65535 + 1];    // (int)Character.MAX_VALUE
            _size = 0;
            NodeStatus = Status.NOT_WORD_1;
        }

        /**
         * 插入一个词
         *
         * @param key
         * @param value
         */
        public void Add(String key, T value) {
            if (key.Length == 0) return;      
            char[] chars = key.ToCharArray();
            for (int i = 0; i < chars.Length - 1; ++i) {
                // 除了最后一个字外，都是继续
                AddChild(new Node<T>(chars[i], Status.NOT_WORD_1, default(T)));
                GetChild(chars[i]);
            }
            // 最后一个字加入时属性为end
            if (AddChild(new Node<T>(chars[chars.Length - 1], Status.WORD_END_3, value))) {
                ++_size; // 维护_size
            }
        }


        public void put(char[] key, T value) {          
            for (int i = 0; i < key.Length - 1; ++i) {
                // 除了最后一个字外，都是继续
                AddChild(new Node<T>(key[i], Status.NOT_WORD_1, default(T)));
                GetChild(key[i]);
            }
            // 最后一个字加入时属性为end
            if (branch.AddChild(new Node<T>(key[key.Length - 1], Status.WORD_END_3, value))) {
                ++_size; // 维护_size
            }
        }

        /// <summary>
        /// 设置键值对，当键不存在的时候会自动插入
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void set(String key, T value) {
            put(key.ToCharArray(), value);
        }

        /**
         * 删除一个词
         *
         * @param key
         */
        public void remove(String key) {           
            char[] chars = key.ToCharArray();
            for (int i = 0; i < chars.Length - 1; ++i) {
                this.GetChild(chars[i]);
            }
            // 最后一个字设为undefined
            if (AddChild(new Node<T>(chars[chars.Length - 1], Status.UNDEFINED_0, Value))) {
                --_size;
            }
        }

        public bool containsKey(String key) {
            char[] chars = key.ToCharArray();
            foreach (char aChar in chars) {
                GetChild(aChar);
            }

            return this != null && (NodeStatus == Status.WORD_END_3 || NodeStatus == Status.WORD_MIDDLE_2);
        }

        public T get(String key) {
            BaseNode<T> branch = this;
            char[] chars = key.ToCharArray();
            foreach (char aChar in chars) {
                if (branch == null) return default(T);
                branch = branch.GetChild(aChar);
            }

            if (branch == null) return default(T);
            // 下面这句可以保证只有成词的节点被返回
            if (!(branch.NodeStatus == Status.WORD_END_3 || branch.NodeStatus == Status.WORD_MIDDLE_2)) return default(T);
            return (T)branch.Value;
        }

        public T get(char[] key) {
            BaseNode<T> branch = this;
            foreach (char aChar in key) {
                if (branch == null) return default(T);
                branch = branch.GetChild(aChar);
            }

            if (branch == null) return default(T);
            // 下面这句可以保证只有成词的节点被返回
            if (!(branch.NodeStatus == Status.WORD_END_3 || branch.NodeStatus == Status.WORD_MIDDLE_2)) return default(T);
            return (T)branch.Value;
        }


        public T[] getValueArray(T[] a) {
            if (a.Length < _size)
                a = (T[])Array.CreateInstance(typeof(T), _size);            
            int i = 0;
            foreach (KeyValuePair<string, T> entry in entrySet()) {
                a[i++] = entry.Value;
            }
            return a;
        }

        /**
         * 获取键值对集合
         *
         * @return
         */
        public Dictionary<String, T> entrySet() {
            var entrySet = new Dictionary<String, T>();
            StringBuilder sb = new StringBuilder();
            foreach (BaseNode<T> node in _child) {
                if (node == null) continue;
                node.Walk(new StringBuilder(sb.ToString()), entrySet);
            }
            return entrySet;
        }

        /**
         * 键集合
         * @return
         */
        public List<String> keySet() {
            List<String> keySet = new List<string>();
            foreach (var item in entrySet()) {
                keySet.Add(item.Key);
            }

            return keySet;
        }

        /**
         * 前缀查询
         *
         * @param key 查询串
         * @return 键值对
         */
        public Dictionary<string,T> prefixSearch(String key) {
            var entrySet = new Dictionary<String, T>();
            StringBuilder sb = new StringBuilder(key.Substring(0, key.Length - 1));
            BaseNode<T> branch = this;
            char[] chars = key.ToCharArray();
            foreach (char aChar in chars) {
                if (branch == null) return entrySet;
                branch = branch.GetChild(aChar);
            }

            if (branch == null) return entrySet;
            branch.Walk(sb, entrySet);
            return entrySet;
        }

        /**
         * 前缀查询，包含值
         *
         * @param key 键
         * @return 键值对列表
         */
        public LinkedList<KeyValuePair<String, T>> commonPrefixSearchWithValue(String key) {
            char[] chars = key.ToCharArray();
            return commonPrefixSearchWithValue(chars, 0);
        }

        /**
         * 前缀查询，通过字符数组来表示字符串可以优化运行速度
         *
         * @param chars 字符串的字符数组
         * @param begin 开始的下标
         * @return
         */
        public LinkedList<KeyValuePair<String, T>> commonPrefixSearchWithValue(char[] chars, int begin) {
            var result = new LinkedList<KeyValuePair<String, T>>();
            StringBuilder sb = new StringBuilder();
            BaseNode<T> branch = this;
            for (int i = begin; i < chars.Length; ++i) {
                char aChar = chars[i];
                branch = branch.GetChild(aChar);
                if (branch == null || branch.NodeStatus == Status.UNDEFINED_0) return result;
                sb.Append(aChar);
                if (branch.NodeStatus == Status.WORD_MIDDLE_2 || branch.NodeStatus == Status.WORD_END_3) {
                    result.AddLast(new KeyValuePair<String, T>(sb.ToString(), (T)branch.Value));
                }
            }

            return result;
        }


        public override bool AddChild(BaseNode<T> node) {
            bool add = false;
            char c = node.GetChar();
            BaseNode<T> target = GetChild(c);
            if (target == null) {
                _child[c] = node;
                add = true;
            }
            else {
                switch (node.NodeStatus) {
                    case Status.UNDEFINED_0:
                        if (target.NodeStatus != Status.NOT_WORD_1) {
                            target.NodeStatus = Status.NOT_WORD_1;
                            add = true;
                        }
                        break;
                    case Status.NOT_WORD_1:
                        if (target.NodeStatus == Status.WORD_END_3) {
                            target.NodeStatus = Status.WORD_MIDDLE_2;
                        }
                        break;
                    case Status.WORD_END_3:
                        if (target.NodeStatus == Status.NOT_WORD_1) {
                            target.NodeStatus = Status.WORD_MIDDLE_2;
                        }
                        if (target.Value == null) {
                            add = true;
                        }
                        target.Value=node.Value;
                        break;
                }
            }
            return add;
        }

        public int Size() {
            return _size;
        }


        protected char getChar() {
            return Char.MaxValue;   // 根节点没有char
        }


        public override BaseNode<T> GetChild(char c) {
            return _child[c];
        }

        public int Build(SortedDictionary<string, T> keyValueMap) {
            foreach (var entry in keyValueMap) {
                Add(entry.Key, entry.Value);
            }
            return 0;
        }

        public bool Save(Stream outStream) {
            throw new NotImplementedException();
        }

        public bool Load(ByteArray byteArray, T[] value) {
            throw new NotImplementedException();
        }

        public T Get(char[] key) {
            throw new NotImplementedException();
        }

        public T Get(string key) {
            throw new NotImplementedException();
        }

        public T[] GetValueArray(T[] a) {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key) {
            throw new NotImplementedException();
        }

        public void Add(T item) {
            throw new NotImplementedException();
        }

        public void Clear() {
            throw new NotImplementedException();
        }

        public bool Contains(T item) {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(T item) {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        //    public bool save(String path) {
        //        try {
        //            DataOutputStream outStream = new DataOutputStream(IOUtil.newOutputStream(path));
        //            for (BaseNode<T> node : _child) {
        //                if (node == null) {
        //                outStream.writeInt(0);
        //                }
        //                else {
        //                outStream.writeInt(1);
        //                    node.walkToSave(outStream);
        //                }
        //            }
        //        outStream.close();
        //        }
        //        catch (Exception e) {
        //            logger.warning("保存到" + path + "失败" + TextUtility.exceptionToString(e));
        //            return false;
        //        }

        //        return true;
        //    }


        //    public int build(TreeMap<String, T> keyValueMap) {
        //        for (Map.Entry<String, T> entry : keyValueMap.entrySet()) {
        //            put(entry.getKey(), entry.getValue());
        //        }
        //        return 0;
        //    }

        //    /**
        //     * 保存到二进制输出流
        //     *
        //     * @param outStream
        //     * @return
        //     */
        //    public bool save(DataOutputStream outStream) {
        //        try {
        //            for (BaseNode<T> node : _child) {
        //                if (node == null) {
        //                outStream.writeInt(0);
        //                }
        //                else {
        //                outStream.writeInt(1);
        //                    node.walkToSave(outStream);
        //                }
        //            }
        //        }
        //        catch (Exception e) {
        //            logger.warning("保存到" + outStream +"失败" + TextUtility.exceptionToString(e));
        //            return false;
        //        }

        //        return true;
        //    }

        //    /**
        //     * 从磁盘加载二分数组树
        //     *
        //     * @param path  路径
        //     * @param value 额外提供的值数组，按照值的字典序。（之所以要求提供它，是因为泛型的保存不归树管理）
        //     * @return 是否成功
        //     */
        //    public bool load(String path, T[] value) {
        //        byte[] bytes = IOUtil.readBytes(path);
        //        if (bytes == null) return false;
        //        _ValueArray valueArray = new _ValueArray(value);
        //        ByteArray byteArray = new ByteArray(bytes);
        //        for (int i = 0; i < _child.Length; ++i) {
        //            int flag = byteArray.NextInt();
        //            if (flag == 1) {
        //                _child[i] = new Node<T>< T > ();
        //                _child[i].WalkToLoad(byteArray, valueArray);
        //            }
        //        }
        //        _size = value.Length;

        //        return true;
        //    }

        //    /**
        //     * 只加载值，此时相当于一个set
        //     *
        //     * @param path
        //     * @return
        //     */
        //    public bool load(String path) {
        //        byte[] bytes = IOUtil.ReadBytes(path);
        //        if (bytes == null) return false;
        //        _ValueArray valueArray = new _EmptyValueArray();
        //        ByteArray byteArray = new ByteArray(bytes);
        //        for (int i = 0; i < _child.Length; ++i) {
        //            int flag = byteArray.NextInt();
        //            if (flag == 1) {
        //                _child[i] = new Node<T>< T > ();
        //                _child[i].walkToLoad(byteArray, valueArray);
        //            }
        //        }
        //        _size = -1;  // 不知道有多少

        //        return true;
        //    }

        //    public bool load(ByteArray byteArray, _ValueArray valueArray) {
        //        for (int i = 0; i < _child.Length; ++i) {
        //            int flag = byteArray.NextInt();
        //            if (flag == 1) {
        //                _child[i] = new Node<T>< T > ();
        //                _child[i].walkToLoad(byteArray, valueArray);
        //            }
        //        }
        //        _size = valueArray.value.Length;

        //        return true;
        //    }

        //    public bool load(ByteArray byteArray, T[] value) {
        //        return load(byteArray, newValueArray().setValue(value));
        //    }

        //    public _ValueArray newValueArray() {
        //        return new _ValueArray();
        //    }


        //    public void writeExternal(ObjectOutput outStream)  {
        //    outStream.writeInt(_size);
        //    for (BaseNode<T> node : _child)
        //    {
        //        if (node == null)
        //        {
        //            outStream.writeInt(0);
        //}
        //        else
        //        {
        //            outStream.writeInt(1);
        //node.walkToSave(outStream);
        //        }
        //    }
        //}


        //public void readExternal(ObjectInput in)
        //{
        //    _size = in.readInt();
        //    for (int i = 0; i<_child.Length; ++i)
        //    {
        //        int flag = in.readInt();
        //        if (flag == 1)
        //        {
        //            _child[i] = new Node<T><T>();
        //            _child[i].walkToLoad(in);
        //        }
        //    }
        //}
    }
}
