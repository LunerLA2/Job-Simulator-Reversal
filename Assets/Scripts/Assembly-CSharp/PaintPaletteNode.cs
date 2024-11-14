using UnityEngine;
using UnityEngine.UI;

public class PaintPaletteNode : MonoBehaviour
{
	[SerializeField]
	private Image background;

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

	public void SetColor(Color color)
	{
		background.color = color;
	}
}
