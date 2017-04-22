﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HanLP.Net.Corpus.IO;

namespace HanLP.Net.Collection.Trie.Bintrie
{
    public abstract class BaseNode<T> : IComparable<BaseNode<T>>
    {
        static Status[] Array_Status = Enum.GetValues(typeof(Status)) as Status[];

        protected BaseNode<T>[] _child;

        protected Status _status;

        protected char _c;

        protected T Value { get; set; }

        public BaseNode<T> Transition(char[] path, int begin) {
            BaseNode<T> cur = this;
            for (int i = begin; i < path.Length; ++i) {
                cur = cur.getChild(path[i]);
                if (cur == null || cur._status == Status.UNDEFINED_0) return null;
            }
            return cur;
        }

        protected abstract bool AddChild(BaseNode<T> node);

        protected bool hasChild(char c) {
            return getChild(c) != null;
        }

        public abstract BaseNode<T> getChild(char c);

        protected char getChar() {
            return _c;
        }

        public int CompareTo(BaseNode<T> other) {
            if (_c > other.getChar()) {
                return 1;
            }
            if (_c < other.getChar()) {
                return -1;
            }
            return 0;
        }

        public Status getStatus() {
            return _status;
        }

        protected void walk(StringBuilder sb, Dictionary<String, T> entrySet) {
            sb.Append(_c);
            if (_status == Status.WORD_MIDDLE_2 || _status == Status.WORD_END_3) {
                entrySet.Add(sb.ToString(), Value);
            }
            if (_child == null) return;
            foreach (BaseNode<T> node in _child) {
                if (node == null) continue;
                node.walk(new StringBuilder(sb.ToString()), entrySet);
            }
        }

        protected void walkToSave(Stream outStream) {

            using (BinaryWriter writer = new BinaryWriter(outStream)) {
                writer.Write(_c);
                writer.Write((int)_status);
                int childSize = 0;
                if (_child != null) childSize = _child.Length;
                writer.Write(childSize);
                if (_child == null) return;
                foreach (BaseNode<T> node in _child) {
                    node.walkToSave(outStream);
                }
            }
        }


        protected void walkToSave(BinaryWriter writer) {
            writer.Write(_c);
            writer.Write((int)_status);
            if (_status == Status.WORD_END_3 || _status == Status.WORD_MIDDLE_2) {
                //writer.writeObject(value);
            }
            int childSize = 0;
            if (_child != null) childSize = _child.Length;
            writer.Write(childSize);
            if (_child == null) return;
            foreach (BaseNode<T> node in _child) {
                node.walkToSave(writer);
            }
        }

        protected void walkToLoad(ByteArray byteArray, ValueArray<T> valueArray) {
            _c = byteArray.NextChar();
            _status = Array_Status[byteArray.NextInt()];
            if (_status == Status.WORD_END_3 || _status == Status.WORD_MIDDLE_2) {
                Value = valueArray.NextValue();
            }
            int childSize = byteArray.NextInt();
            _child = new BaseNode<T>[childSize];
            for (int i = 0; i < childSize; ++i) {
                //todo: _child[i] = new Node<V>();
                _child[i].walkToLoad(byteArray, valueArray);
            }
        }

        protected void walkToLoad(BinaryReader byteArray) {
            _c = byteArray.ReadChar();
            _status = Array_Status[byteArray.ReadInt32()];
            if (_status == Status.WORD_END_3 || _status == Status.WORD_MIDDLE_2) {
                //Value = (T) byteArray..readobj();
            }
            int childSize = byteArray.ReadInt32();
            _child = new BaseNode<T>[childSize];
            for (int i = 0; i < childSize; ++i) {
                //_child[i] = new Node<V>();
                _child[i].walkToLoad(byteArray);
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
    }
}