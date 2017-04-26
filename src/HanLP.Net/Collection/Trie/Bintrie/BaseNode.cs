using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HanLP.Net.Corpus.IO;

namespace HanLP.Net.Collection.Trie.Bintrie
{
    public abstract partial class BaseNode< T> : IComparable<BaseNode<T>>
    {
        private static Status[] Array_Status = Enum.GetValues(typeof(Status)) as Status[];

        protected BaseNode<T>[] _child;

        public Status NodeStatus { get; set; }

        protected char _c;

        public T Value { get; set; }

        public BaseNode<T> Transition(char[] path, int begin) {
            BaseNode<T> cur = this;
            for (int i = begin; i < path.Length; ++i) {
                cur = cur.GetChild(path[i]);
                if (cur == null || cur.NodeStatus == Status.UNDEFINED_0) return null;
            }
            return cur;
        }

        public abstract bool AddChild(BaseNode<T> node);

        public bool HasChild(char c) {
            return GetChild(c) != null;
        }
                

        public abstract BaseNode<T> GetChild(char c);

        public char GetChar() {
            return _c;
        }

        public int CompareTo(BaseNode<T> other) {
            if (_c > other.GetChar()) {
                return 1;
            }
            if (_c < other.GetChar()) {
                return -1;
            }
            return 0;
        }

        public void Walk(StringBuilder sb, Dictionary<String, T> entrySet) {
            sb.Append(_c);
            if (NodeStatus == Status.WORD_MIDDLE_2 || NodeStatus == Status.WORD_END_3) {
                entrySet.Add(sb.ToString(), Value);
            }
            if (_child == null) return;
            foreach (BaseNode<T> node in _child) {
                if (node == null) continue;
                node.Walk(new StringBuilder(sb.ToString()), entrySet);
            }
        }

        protected void WalkToSave(Stream outStream) {
            using (BinaryWriter writer = new BinaryWriter(outStream)) {
                writer.Write(_c);
                writer.Write((int)NodeStatus);
                int childSize = 0;
                if (_child != null) childSize = _child.Length;
                writer.Write(childSize);
                if (_child == null) return;
                foreach (BaseNode<T> node in _child) {
                    node.WalkToSave(outStream);
                }
            }
        }

        protected void WalkToSave(BinaryWriter writer) {
            writer.Write(_c);
            writer.Write((int)NodeStatus);
            if (NodeStatus == Status.WORD_END_3 || NodeStatus == Status.WORD_MIDDLE_2) {
                //writer.writeObject(value);
            }
            int childSize = 0;
            if (_child != null) childSize = _child.Length;
            writer.Write(childSize);
            if (_child == null) return;
            foreach (BaseNode<T> node in _child) {
                node.WalkToSave(writer);
            }
        }

        protected void WalkToLoad(ByteArray byteArray, ValueArray<T> valueArray) {
            _c = byteArray.NextChar();
            NodeStatus = Array_Status[byteArray.NextInt()];
            if (NodeStatus == Status.WORD_END_3 || NodeStatus == Status.WORD_MIDDLE_2) {
                Value = valueArray.NextValue();
            }
            int childSize = byteArray.NextInt();
            _child = new BaseNode<T>[childSize];
            for (int i = 0; i < childSize; ++i) {
                //todo: _child[i] = new Node<V>();
                _child[i].WalkToLoad(byteArray, valueArray);
            }
        }

        protected void WalkToLoad(BinaryReader byteArray) {
            _c = byteArray.ReadChar();
            NodeStatus = Array_Status[byteArray.ReadInt32()];
            if (NodeStatus == Status.WORD_END_3 || NodeStatus == Status.WORD_MIDDLE_2) {
                //Value = (T) byteArray..readobj();
            }
            int childSize = byteArray.ReadInt32();
            _child = new BaseNode<T>[childSize];
            for (int i = 0; i < childSize; ++i) {
                //_child[i] = new Node<V>();
                _child[i].WalkToLoad(byteArray);
            }
        }

        public enum Status
        {
            /**
             * 未指定，用于删除词条
             */
            UNDEFINED_0,
            /**
             * 不是词语的结尾
             */
            NOT_WORD_1,
            /**
             * 是个词语的结尾，并且还可以继续
             */
            WORD_MIDDLE_2,
            /**
             * 是个词语的结尾，并且没有继续
             */
            WORD_END_3,
        }

        public override String ToString() {
            return "BaseNode{" +
                    "child=" + _child.Length +
                    ", status=" + NodeStatus +
                    ", c=" + _c +
                    ", value=" + Value +
                    '}';
        }
    }
}