using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PageWithAlternatePages", menuName = "Page with Alternate Pages Data")]
public class PageWithAlternatePagesData : ScriptableObject
{
	[SerializeField]
	private PageData basePage;

	[SerializeField]
	private List<PageWithDefaultWorldItemIconData> alternatePages;

	[SerializeField]
	[Range(0f, 1f)]
	private float percentChanceToUseAlternate = 0.5f;

	private int previousIndex;

	private WorldItemWithIconData currentWorldItemWithIconData;

	public PageData BasePage
	{
		get
		{
			return basePage;
		}
	}

	public List<PageWithDefaultWorldItemIconData> AlternatePages
	{
		get
		{
			return alternatePages;
		}
	}

	public WorldItemWithIconData CurrentWorldItemWithIconData
	{
		get
		{
			return currentWorldItemWithIconData;
		}
	}

	public PageData GetRandomPage()
	{
		currentWorldItemWithIconData = null;
		if (Random.Range(0f, 1f) >= percentChanceToUseAlternate)
		{
			return basePage;
		}
		if (alternatePages.Count == 1)
		{
			currentWorldItemWithIconData = alternatePages[0].WorldItemWithIconData;
			return alternatePages[0].Page;
		}
		int num;
		for (num = Random.Range(0, alternatePages.Count); num == previousIndex; num = Random.Range(0, alternatePages.Count))
		{
		}
		previousIndex = num;
		if (num < alternatePages.Count)
		{
			currentWorldItemWithIconData = alternatePages[num].WorldItemWithIconData;
			return alternatePages[num].Page;
		}
		return basePage;
	}
}
