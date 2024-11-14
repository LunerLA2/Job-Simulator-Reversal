using UnityEngine;

public class ComputerTogglable : ComputerClickable
{
	[SerializeField]
	private bool isSelected;

	[SerializeField]
	private GameObject[] visibleWhenSelected;

	[SerializeField]
	private GameObject[] visibleWhenDeselected;

	public bool IsSelected
	{
		get
		{
			return isSelected;
		}
	}

	public event ComputerTogglableSelectionChangedHandler SelectionChanged;

	public override void Awake()
	{
		base.Awake();
		ForceSelectionNoEvents(isSelected);
	}

	public void Toggle()
	{
		SetSelection(!isSelected);
	}

	public void Select()
	{
		SetSelection(true);
	}

	public void Deselect()
	{
		SetSelection(false);
	}

	public void ForceSelectionNoEvents(bool isSelected)
	{
		this.isSelected = isSelected;
		ReflectSelection();
	}

	public void SetSelection(bool isSelected)
	{
		this.isSelected = isSelected;
		ReflectSelection();
		if (this.SelectionChanged != null)
		{
			this.SelectionChanged(this, isSelected);
		}
	}

	private void ReflectSelection()
	{
		for (int i = 0; i < visibleWhenSelected.Length; i++)
		{
			visibleWhenSelected[i].SetActive(isSelected);
		}
		for (int j = 0; j < visibleWhenDeselected.Length; j++)
		{
			visibleWhenDeselected[j].SetActive(!isSelected);
		}
	}

	public override void Click()
	{
		base.Click();
		Toggle();
	}
}
