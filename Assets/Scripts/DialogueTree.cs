using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class DialogueTree
{
    private readonly TreeNode<Dialogue> _rootNode;

    public TreeNode<Dialogue> Root { get { return _rootNode; } }
    public TreeNode<Dialogue> Current { get; set; }

    public DialogueTree(TreeNode<Dialogue> rootNode)
    {
        _rootNode = rootNode;
        Current = rootNode;
    }

    public ReadOnlyCollection<TreeNode<Dialogue>> SelectOption(int x)
    {
        if (x < 0 || x > Current.Children.Count - 1)
        {
            Debug.LogError("ConversationError: Selected option out of bounds.");
            return null;
        }

        Current = Current.Children[x];
        return Current.Children;
    }

    private TreeNode<Dialogue> FindById(int id, TreeNode<Dialogue> currentNode = null)
    {
        currentNode ??= Root;

        if (currentNode.Id == id)
        {
            return currentNode;
        }

        foreach (var child in currentNode.Children)
            return FindById(id, child);

        return null;
    }
}
