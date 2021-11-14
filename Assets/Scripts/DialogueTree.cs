using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class DialogueTree
{
    private readonly TreeNode<IDialogue> _rootNode;

    public TreeNode<IDialogue> Root { get { return _rootNode; } }
    public TreeNode<IDialogue> Current { get; set; }

    public DialogueTree(TreeNode<IDialogue> rootNode)
    {
        _rootNode = rootNode;
        Current = rootNode;
    }

    public ReadOnlyCollection<TreeNode<IDialogue>> SelectOption(int x)
    {
        if (x < 0 || x > Current.Children.Count - 1)
        {
            Debug.LogError("ConversationError: Selected option out of bounds.");
            return null;
        }

        Current = Current.Children[x];
        return Current.Children;
    }

    private TreeNode<IDialogue> FindById(int id, TreeNode<IDialogue> currentNode = null)
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
