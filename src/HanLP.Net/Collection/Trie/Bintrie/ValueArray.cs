using System;
using System.Collections.Generic;
using System.Text;

namespace HanLP.Net.Collection.Trie.Bintrie
{
    public class ValueArray<T>
    {
        private int _offset;
        private T[] _value;

        protected ValueArray() {

        }

        public ValueArray(T[] values) {
            _value = values;
        }

        public virtual T NextValue() {
            return _value[_offset++];
        }

        public ValueArray<T> SetValue(T[] values) {
            _value = values;
            return this;
        }
    }
}
