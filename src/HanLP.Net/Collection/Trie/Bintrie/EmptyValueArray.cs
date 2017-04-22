using System;
using System.Collections.Generic;
using System.Text;

namespace HanLP.Net.Collection.Trie.Bintrie
{
    public class EmptyValueArray<T> : ValueArray<T>
    {
        public EmptyValueArray() {

        }

        public override T NextValue() {
            return default(T);
        }
    }
}
