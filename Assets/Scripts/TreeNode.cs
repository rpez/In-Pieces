using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class TreeNode<T>
    {
        private readonly int _id;
        private readonly T _value;
        private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();

        // Each node has an id and a value.
        // In the case of dialogue files, the id will correspond to the dialogue line's number.
        public TreeNode(int id, T value)
        {
            _id = id;
            _value = value;
        }

        public TreeNode<T> this[int i]
        {
            get { return _children[i]; }
        }

        public TreeNode<T> Parent { get; private set; }

        public int Id { get { return _id; } }

        public T Value { get { return _value; } }

        public ReadOnlyCollection<TreeNode<T>> Children
        {
            get { return _children.AsReadOnly(); }
        }

        public TreeNode<T> AddChild(int id, T value)
        {
            var node = new TreeNode<T>(id, value) {Parent = this};
            _children.Add(node);
            return node;
        }

        public bool RemoveChild(TreeNode<T> node)
        {
            return _children.Remove(node);
        }

        public void Traverse(Action<T> action)
        {
            action(Value);
            foreach (var child in _children)
                child.Traverse(action);
        }

        public IEnumerable<T> Flatten()
        {
            return new[] {Value}.Concat(_children.SelectMany(x => x.Flatten()));
        }
    }
