using System;
using UnityEngine;

[Serializable]
public class UniqueElement : Element
{
	public GameObject objectPrefab;

	public UniqueElement()
	{
		state = StateOfMatter.Unique;
	}
}
