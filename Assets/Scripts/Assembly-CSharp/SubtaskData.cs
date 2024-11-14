using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SubtaskData : ScriptableObject
{
	public enum SubtaskIconLayoutTypes
	{
		DEFAULT = 0,
		SINGLEICON = 1,
		COMPLETELYHIDDEN = 2
	}

	[SerializeField]
	private SubtaskIconLayoutTypes subtaskIconLayoutType;

	[SerializeField]
	private List<ActionEventCondition> actionEventConditions = new List<ActionEventCondition>();

	[SerializeField]
	private Sprite layoutDependantSpriteOne;

	[SerializeField]
	private Sprite layoutDependantSpriteTwo;

	[SerializeField]
	private Sprite layoutDependantSpriteThree;

	[SerializeField]
	private Sprite layoutDependantSpriteFour;

	[SerializeField]
	private Sprite layoutDependantSpriteFive;

	[SerializeField]
	private int counterSize;

	[SerializeField]
	private bool hideCounterOnJobBoard;

	[SerializeField]
	private bool onlyTrackProgressWhenVisible;

	[SerializeField]
	private List<SubtaskData> subtasksToAutoComplete = new List<SubtaskData>();

	[SerializeField]
	private bool disallowUncompleteOnAutoCompletedSubtasks;

	public SubtaskIconLayoutTypes SubtaskIconLayoutType
	{
		get
		{
			return subtaskIconLayoutType;
		}
	}

	public List<ActionEventCondition> ActionEventConditions
	{
		get
		{
			return actionEventConditions;
		}
	}

	public int CounterSize
	{
		get
		{
			return counterSize;
		}
	}

	public bool HideCounterOnJobBoard
	{
		get
		{
			return hideCounterOnJobBoard;
		}
	}

	public bool OnlyTrackProgressWhenVisible
	{
		get
		{
			return onlyTrackProgressWhenVisible;
		}
	}

	public List<SubtaskData> SubtasksToAutoComplete
	{
		get
		{
			return subtasksToAutoComplete;
		}
	}

	public bool DisallowUncompleteOnAutoCompletedSubtasks
	{
		get
		{
			return disallowUncompleteOnAutoCompletedSubtasks;
		}
	}

	public Sprite LayoutDependantSpriteOne
	{
		get
		{
			return layoutDependantSpriteOne;
		}
		set
		{
			layoutDependantSpriteOne = value;
		}
	}

	public Sprite LayoutDependantSpriteTwo
	{
		get
		{
			return layoutDependantSpriteTwo;
		}
	}

	public Sprite LayoutDependantSpriteThree
	{
		get
		{
			return layoutDependantSpriteThree;
		}
	}

	public Sprite LayoutDependantSpriteFour
	{
		get
		{
			return layoutDependantSpriteFour;
		}
	}

	public Sprite LayoutDependantSpriteFive
	{
		get
		{
			return layoutDependantSpriteFive;
		}
	}

	public void EditorSetSubtaskIconLayoutType(SubtaskIconLayoutTypes type)
	{
		subtaskIconLayoutType = type;
	}

	public void EditorSetCounterSize(int size)
	{
		counterSize = size;
	}

	public void EditorSetHideCounterOnJobBoard(bool hide)
	{
		hideCounterOnJobBoard = hide;
	}

	public void EditorSetOnlyTrackProgressWhenVisible(bool track)
	{
		onlyTrackProgressWhenVisible = track;
	}

	public void EditorSetDisallowUncompleteOnAutoCompletedSubtasks(bool v)
	{
		disallowUncompleteOnAutoCompletedSubtasks = v;
	}

	public void EditorSetLayoutDependantSprite(Sprite sprite, int index)
	{
		switch (index)
		{
		case 1:
			layoutDependantSpriteOne = sprite;
			break;
		case 2:
			layoutDependantSpriteTwo = sprite;
			break;
		case 3:
			layoutDependantSpriteThree = sprite;
			break;
		case 4:
			layoutDependantSpriteFour = sprite;
			break;
		case 5:
			layoutDependantSpriteFive = sprite;
			break;
		default:
			Debug.LogError("Can't set sprite of index " + index);
			break;
		}
	}
}
