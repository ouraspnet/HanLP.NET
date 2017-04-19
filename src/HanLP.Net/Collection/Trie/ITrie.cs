using System;
using System.Collections.Generic;
using System.IO;
using HanLP.Net.Corpus.IO;

namespace HanLP.Net.Collection.Trie
{
    internal interface ITrie<T>
    {
        int Build(SortedDictionary<string, T> keyValueMap);

        bool Save(Stream outStream);

        bool Load(ByteArray byteArray, T[] value);

        T Get(char[] key);

        T Get(String key);

        T[] GetValueArray(T[] a);

        bool ContainsKey(String key);

        int Length { get; }
    }
}