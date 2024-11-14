using System.Collections.Generic;
using UnityEngine;

public class ComputerToggleGroup : MonoBehaviour
{
	private List<ComputerTogglable> choices = new List<ComputerTogglable>();

	private ComputerTogglable selection;

	public ComputerTogglable Selection
	{
		get
		{
			return selection;
		}
	}

	public int SelectionIndex
	{
		get
		{
			return choices.IndexOf(selection);
		}
	}

	public ComputerTogglable[] Choices
	{
		get
		{
			return choices.ToArray();
		}
	}

	public event ComputerToggleGroupSelectionChangedHandler SelectionChanged;

	protected void OnEnable()
	{
		RefreshChoices();
	}

	public void RefreshChoices()
	{
		for (int i = 0; i < choices.Count; i++)
		{
			choices[i].SelectionChanged -= OnChoiceSelectionChanged;
		}
		selection = null;
		choices.Clear();
		for (int j = 0; j < base.transform.childCount; j++)
		{
			ComputerTogglable component = base.transform.GetChild(j).GetComponent<ComputerTogglable>();
			if (component != null)
			{
				choices.Add(component);
				component.SelectionChanged += OnChoiceSelectionChanged;
			}
		}
		for (int k = 0; k < choices.Count; k++)
		{
			if (choices[k].IsSelected)
			{
				selection = choices[k];
			}
		}
	}

	public void ClearSelection()
	{
		selection = null;
		for (int i = 0; i < choices.Count; i++)
		{
			choices[i].ForceSelectionNoEvents(false);
		}
	}

	private void OnChoiceSelectionChanged(ComputerTogglable togglable, bool isSelected)
	{
		ComputerTogglable prevSelection = selection;
		if (togglable == selection && !isSelected)
		{
			togglable.ForceSelectionNoEvents(true);
		}
		else if (togglable != selection && isSelected)
		{
			selection = togglable;
			for (int i = 0; i < choices.Count; i++)
			{
				if (selection != choices[i])
				{
					choices[i].ForceSelectionNoEvents(false);
				}
			}
		}
		if (this.SelectionChanged != null)
		{
			this.SelectionChanged(this, selection, prevSelection);
		}
	}
}
