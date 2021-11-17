# Game state documentation

## GameManager.cs

`GameManager` is a singleton class with game state inside the property `GameManager.State`.

Can **get** any state value with a string like this: `GameManager.GetStateValue("TUTORIAL_LISTEN");`.

Can **set** any state value with a string like this: `GameManager.SetStateValue("TUTORIAL_COUNT_SENSES", 3);`

## GameState.cs

Has all the game state as class properties. Don't do stuff here directly: access and modify the data here through the `GameManager` functions.

# Dialogue documentation

## DialogueManager.cs

`DialogueManager` is a singleton class. __This class is responsible for giving the necessary data to draw dialogue on screen__.

`DialogueManager.AllDialogue` returns a dictionary of the form `<string, DialogueTree>` where the key (string) corresponds to a .dlg file filename without extension.

`DialogueManager.DialogueAvailable(string dialogue, GameManager gameManager)` checks if a dialogue line should be drawn on the screen based on the line's Condition.

```
# Still missing at least these properties and functions inside `DialogueManager`:

// properties
public DialogueTree DialogueManager.CurrentDialogue { get; set; }
public TreeNode<IDialogue> DialogueManager.CurrentDialogue.CurrentLine { get; set; }
public bool DialogueManager.DialogueScreenVisible { get; set }

// functions
public TreeNode<IDialogue> GetCurrentLineChildren()  // used to draw the next set of dialogue
```

## DialogueTree.cs

`DialogueTree` is basically a tree structure corresponding to a single dialogue file.

## Dialogue.cs

`IDialogue` is an interface that corresponds to a single dialogue line. `IConditionalDialogue` is an `IDialogue` that contains a `Condition` and `Actions`.

There are four types of `IDialogue`s here:

`ActorDialogue : IConditionalDialogue`  represents a line that can't be interacted on (Mostly lines by other NPCs)
`PlayerDialogue : IConditionalDialogue` represents a line that can be interacted by the player
`EndDialogue`    represents a button to close the dialogue screen
`RefDialogue`    represents a line that references another line (used to jump/return to a previous node in a dialogue)

Easy way to check for type and do casting in a single line:

```
if (dialogue is ActorDialogue actorDialogue)
    // do something with actorDialogue
```

### Conditions

There are two types of `IDialogueCondition`

`BoolDialogueCondition`  represents a Condition that corresponds to a boolean in `GameManager.State`
`IntDialogueCondition`   represents a Condition that corresponds to an integer in `GameManager.State`

### Actions

There are three types of `IDialogueAction`

`SetDialogueAction` sets a boolean to some value in `GameManager.State`
`AddDialogueAction` adds a value to some integer in `GameManager.State`
`RollDialogueAction` rolls the dice in conversation, sets `GameManager.State.ROLL_SUCCESS = true` if successful

## TreeNode.cs

Generic tree node with an **id** and an **object** that the node stores (the **object** being IDialogue in this case).
