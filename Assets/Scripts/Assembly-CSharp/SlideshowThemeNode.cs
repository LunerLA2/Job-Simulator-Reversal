using UnityEngine;
using UnityEngine.UI;

public class SlideshowThemeNode : MonoBehaviour
{
	[SerializeField]
	private Image background;

	[SerializeField]
	private Text text;

	[SerializeField]
	private GameObject outline;

	private ComputerTogglable togglable;

	private void Awake()
	{
		togglable = GetComponent<ComputerTogglable>();
		togglable.SelectionChanged += OnSelectionChanged;
	}

	private void OnSelectionChanged(ComputerTogglable togglable, bool isSelected)
	{
		outline.SetActive(isSelected);
	}

	public void SetTheme(SlideshowTheme theme)
	{
		background.color = theme.BackColor;
		text.color = theme.TextColor;
	}
}
