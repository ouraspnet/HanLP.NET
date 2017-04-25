using System;

namespace HanLP.Net.Collection.Trie.Bintrie
{
    public class Node<T> : BaseNode<T>
    {
        public Node() {
        }

        public Node(char c, Status status, T value) {
            _c = c;
            NodeStatus = status;
            Value = value;
        }

        public override BaseNode<T> GetChild(char c) {
            if (_child == null) return null;
            int index = Array.BinarySearch(_child, c);
            if (index < 0) return null;

            return _child[index];
        }

        protected override bool AddChild(BaseNode<T> node) {
            var add = false;
            if (_child == null) {
                _child = new BaseNode<T>[0];
            }
            int index = Array.BinarySearch(_child, node);
            if (index >= 0) {
                var target = _child[index];
                switch (node.NodeStatus) {
                    case Status.UNDEFINED_0:
                        if (target.NodeStatus != Status.NOT_WORD_1) {
                            target.NodeStatus = Status.NOT_WORD_1;
                            target.Value = default(T);
                            add = true;
                        }
                        break;

                    case Status.NOT_WORD_1:
                        if (target.NodeStatus == Status.WORD_END_3) {
                            target.NodeStatus = Status.WORD_MIDDLE_2;
                        }
                        break;

                    case Status.WORD_END_3:
                        if (target.NodeStatus != Status.WORD_END_3) {
                            target.NodeStatus = Status.WORD_MIDDLE_2;
                        }
                        if (target.Value == null) {
                            add = true;
                        }
                        target.Value = node.Value;
                        break;
                }
            }
            else {
                BaseNode<T>[] newChild = new BaseNode<T>[_child.Length + 1];
                int insert = -(index + 1);
                Array.Copy(_child, 0, newChild, 0, insert);
                Array.Copy(_child, insert, newChild, insert + 1, _child.Length - insert);
                newChild[insert] = node;
                _child = newChild;
                add = true;
            }
            return add;
        }
    }
}