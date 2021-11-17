using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class DialogueTree
{
    public TreeNode<IDialogue> Root { get; }
    public TreeNode<IDialogue> Current { get; set; }

    public DialogueTree(TreeNode<IDialogue> rootNode)
    {
        Root = rootNode;
        Current = rootNode;
    }

    public List<IDialogue> ToList()
    {
        return Root.Flatten().ToList();
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
