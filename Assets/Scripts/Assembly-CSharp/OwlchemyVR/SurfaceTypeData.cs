using System;
using UnityEngine;

namespace OwlchemyVR
{
	[Serializable]
	public class SurfaceTypeData : ScriptableObject
	{
		public string SurfaceTypeName
		{
			get
			{
				return base.name;
			}
		}
	}
}
