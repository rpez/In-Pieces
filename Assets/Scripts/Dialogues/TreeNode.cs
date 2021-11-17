using System;
using System.Collections.Generic;
using System.Linq;

public class TreeNode<T>
{

    public List<TreeNode<T>> Children { get; } = new List<TreeNode<T>>();

    public TreeNode<T> Parent { get; private set; }

    public int Id { get; }

    public T Value { get; }

    // Each node has an id and a value.
    // In the case of dialogue files, the id will correspond to the dialogue line's number.
    public TreeNode(int id, T val)
    {
        Id = id;
        Value = val;
    }

    public TreeNode<T> this[int i]
    {
        get { return Children[i]; }
    }

    public TreeNode<T> AddChild(int id, T val)
    {
        var node = new TreeNode<T>(id, val) {Parent = this};
        Children.Add(node);
        return node;
    }

    public bool RemoveChild(TreeNode<T> node)
    {
        return Children.Remove(node);
    }

    public void Traverse(Action<T> action)
    {
        action(Value);
        foreach (var child in Children)
            child.Traverse(action);
    }

    public IEnumerable<T> Flatten()
    {
        return new[] {Value}.Concat(Children.SelectMany(x => x.Flatten()));
    }
}
