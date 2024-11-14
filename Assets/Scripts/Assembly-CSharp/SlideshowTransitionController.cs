using System.Linq;
using UnityEngine;

public class SlideshowTransitionController : MonoBehaviour
{
	[SerializeField]
	[HideInInspector]
	private int optionIndex = -1;

	private GameObject[] options;

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
	}
}
