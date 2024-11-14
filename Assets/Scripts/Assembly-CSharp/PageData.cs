using System;
using System.Collections.Generic;
using PSC;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class PageData : ScriptableObject
{
	public enum PageLayoutTypes
	{
		AUTO_GRID = 1,
		MANY_TO_ONE = 2,
		MANY_TO_ONE_WITH_FINAL_STEP = 3,
		MANY_TO_FINAL_STEP = 4
	}

	[SerializeField]
	private PageLayoutTypes pageLayoutType = PageLayoutTypes.AUTO_GRID;

	[SerializeField]
	private bool onlyAppearOnCertainLayouts;

	[SerializeField]
	private List<LayoutConfiguration> layoutsToAppearOn = new List<LayoutConfiguration>();

	[SerializeField]
	private List<SubtaskData> subtasks = new List<SubtaskData>();

	[SerializeField]
	private float secsOfBlankBeforeAnimatingIn;

	[SerializeField]
	private float secsToLingerOnCompletedSubtasks;

	[SerializeField]
	private float secsOfBlankAfterAnimatingOut;

	[SerializeField]
	private Sprite layoutDependantSpriteOne;

	public PageLayoutTypes PageLayoutType
	{
		get
		{
			return pageLayoutType;
		}
	}

	public bool OnlyAppearOnCertainLayouts
	{
		get
		{
			return onlyAppearOnCertainLayouts;
		}
	}

	public List<LayoutConfiguration> LayoutsToAppearOn
	{
		get
		{
			return layoutsToAppearOn;
		}
	}

	public List<SubtaskData> Subtasks
	{
		get
		{
			return subtasks;
		}
		set
		{
			subtasks = value;
		}
	}

	public float SecsOfBlankBeforeAnimatingIn
	{
		get
		{
			return secsOfBlankBeforeAnimatingIn;
		}
	}

	public float SecsToLingerOnCompletedSubtasks
	{
		get
		{
			return secsToLingerOnCompletedSubtasks;
		}
	}

	public float SecsOfBlankAfterAnimatingOut
	{
		get
		{
			return secsOfBlankAfterAnimatingOut;
		}
	}

	public Sprite LayoutDependantSpriteOne
	{
		get
		{
			return layoutDependantSpriteOne;
		}
	}

	public bool ShouldAppearOnLayout(LayoutConfiguration layout)
	{
		if (onlyAppearOnCertainLayouts)
		{
			return layoutsToAppearOn.Contains(layout);
		}
		return true;
	}

	public void EditorSetLayoutType(PageLayoutTypes t)
	{
		pageLayoutType = t;
	}

	public void EditorSetOnlyAppearOnCertainLayouts(bool o)
	{
		onlyAppearOnCertainLayouts = o;
	}

	public void EditorSetLayoutDependantSprite(Sprite sprite, int index)
	{
		if (index == 1)
		{
			layoutDependantSpriteOne = sprite;
		}
		else
		{
			Debug.LogError("Can't set sprite of index " + index);
		}
	}

	public void EditorSetSecsOfBlankBeforeAnimatingIn(float secs)
	{
		secsOfBlankBeforeAnimatingIn = secs;
	}

	public void EditorSetSecsToLingerOnCompletedSubtasks(float secs)
	{
		secsToLingerOnCompletedSubtasks = secs;
	}

	public void EditorSetSecsOfBlankAfterAnimatingOut(float secs)
	{
		secsOfBlankAfterAnimatingOut = secs;
	}
}
