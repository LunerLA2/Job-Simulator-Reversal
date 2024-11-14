using System;
using UnityEngine;

[Serializable]
public class BusinessCardletInfo
{
	[Multiline(2)]
	public string Heading;

	[Multiline(4)]
	public string Body;
}
