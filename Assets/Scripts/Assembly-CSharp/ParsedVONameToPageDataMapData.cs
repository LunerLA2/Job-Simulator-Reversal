using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parsed VO Name To Page Data Map")]
public class ParsedVONameToPageDataMapData : ScriptableObject
{
	[Serializable]
	private class ParsedNamePageDataPair
	{
		public string pageName;

		public PageData pageData;
	}

	[SerializeField]
	private ParsedNamePageDataPair[] nameToDataMap;

	public Dictionary<string, List<PageData>> GetDictionary()
	{
		Dictionary<string, List<PageData>> dictionary = new Dictionary<string, List<PageData>>();
		if (nameToDataMap != null && nameToDataMap.Length != 0)
		{
			ParsedNamePageDataPair[] array = nameToDataMap;
			foreach (ParsedNamePageDataPair parsedNamePageDataPair in array)
			{
				List<PageData> value;
				dictionary.TryGetValue(parsedNamePageDataPair.pageName, out value);
				if (value == null)
				{
					value = new List<PageData>();
					dictionary.Add(parsedNamePageDataPair.pageName, value);
				}
				value.Add(parsedNamePageDataPair.pageData);
			}
		}
		return dictionary;
	}
}
