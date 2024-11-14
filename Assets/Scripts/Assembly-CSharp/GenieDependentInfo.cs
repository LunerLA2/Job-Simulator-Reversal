using System;
using UnityEngine;

[Serializable]
public class GenieDependentInfo
{
	[SerializeField]
	private JobGenieCartridge.GenieModeTypes genieType;

	[SerializeField]
	private bool setMyStateTo = true;

	public JobGenieCartridge.GenieModeTypes GenieType
	{
		get
		{
			return genieType;
		}
	}

	public bool SetMyStateTo
	{
		get
		{
			return setMyStateTo;
		}
	}
}
