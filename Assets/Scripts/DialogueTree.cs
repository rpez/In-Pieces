using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class DialogueTree
{
    public TreeNode<IDialogue> Root { get; }
    public TreeNode<IDialogue> CurrentLine { get; set; }
    public List<TreeNode<IDialogue>> Options { get; set; }

    public DialogueTree(TreeNode<IDialogue> rootNode)
    {
        Root = rootNode;
        CurrentLine = rootNode;
    }

    public List<IDialogue> ToList()
    {
        return Root.Flatten().ToList();
    }

    public TreeNode<IDialogue> FindById(int id)
    {
        Stack<TreeNode<IDialogue>> stack = new Stack<TreeNode<IDialogue>>();
        stack.Push(Root);

        while (stack.Count > 0)
        {
            TreeNode<IDialogue> currentNode = stack.Pop();

            if (currentNode.Id == id) return currentNode;

            foreach (var child in currentNode.Children)
                stack.Push(child);
        }

        return null;
    }
}
