using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SlideshowPageController : MonoBehaviour
{
	[SerializeField]
	private Text[] unthemedTexts;

	[SerializeField]
	[HideInInspector]
	private int optionIndex = -1;

	private GameObject[] options;

	private Image background;

	private SlideshowTheme theme;

	public int OptionIndex
	{
		get
		{
			return optionIndex;
		}
	}

	public string[] OptionNames
	{
		get
		{
			return options.Select((GameObject o) => o.name).ToArray();
		}
	}

	private void Awake()
	{
		if (options == null)
		{
			FetchOptions();
		}
	}

	private void FetchOptions()
	{
		options = new GameObject[base.transform.childCount];
		for (int i = 0; i < base.transform.childCount; i++)
		{
			options[i] = base.transform.GetChild(i).gameObject;
		}
	}

	public void SetTheme(SlideshowTheme theme)
	{
		this.theme = theme;
		GetComponent<Image>().color = theme.BackColor;
	}

	public void ShowOption(int optionIndex)
	{
		if (options == null)
		{
			FetchOptions();
		}
		this.optionIndex = optionIndex;
		for (int i = 0; i < options.Length; i++)
		{
			options[i].gameObject.SetActive(i == optionIndex);
		}
		Text[] componentsInChildren = options[optionIndex].GetComponentsInChildren<Text>();
		foreach (Text text in componentsInChildren)
		{
			if (Array.IndexOf(unthemedTexts, text) == -1)
			{
				text.color = theme.TextColor;
			}
		}
	}
}
