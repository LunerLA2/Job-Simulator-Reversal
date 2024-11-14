using System;
using System.Collections.Generic;
using UnityEngine;

namespace OwlchemyVR
{
	[Serializable]
	public class PhysicalSpaceLayoutObject
	{
		public enum PhysicalSpaceTypes
		{
			Default = 0,
			SteamVR_400x300 = 1,
			SteamVR_300x225 = 2,
			SteamVR_200x150 = 3,
			Oculus = 4,
			Morpheus = 5
		}

		public const PhysicalSpaceTypes DEFAULT_SPACE_TYPE = PhysicalSpaceTypes.SteamVR_300x225;

		[SerializeField]
		private PhysicalSpaceTypes physicalSpaceType;

		[SerializeField]
		private List<PhysicalSpaceCustomTransformInfo> customTransformInfoList = new List<PhysicalSpaceCustomTransformInfo>();

		public PhysicalSpaceTypes PhysicalSpaceType
		{
			get
			{
				return physicalSpaceType;
			}
		}

		public List<PhysicalSpaceCustomTransformInfo> CustomTransformInfoList
		{
			get
			{
				return customTransformInfoList;
			}
			set
			{
				customTransformInfoList = value;
			}
		}

		public PhysicalSpaceLayoutObject(PhysicalSpaceTypes physicalSpaceType)
		{
			this.physicalSpaceType = physicalSpaceType;
		}

		public void AddNew(PhysicalSpaceCustomTransformInfo transformInfoObj)
		{
			customTransformInfoList.Add(transformInfoObj);
		}

		public bool DoesHaveAnyDifferentChanges(PhysicalSpaceLayoutObject other)
		{
			if (other == null)
			{
				if (customTransformInfoList.Count == 0)
				{
					return false;
				}
				return true;
			}
			if (other.customTransformInfoList.Count != customTransformInfoList.Count)
			{
				return true;
			}
			bool flag = false;
			for (int i = 0; i < customTransformInfoList.Count; i++)
			{
				PhysicalSpaceCustomTransformInfo physicalSpaceCustomTransformInfo = customTransformInfoList[i];
				flag = false;
				for (int j = 0; j < other.customTransformInfoList.Count; j++)
				{
					if (other.customTransformInfoList[j].Transform == physicalSpaceCustomTransformInfo.Transform)
					{
						flag = true;
						if (physicalSpaceCustomTransformInfo.IsPropertiesDifferent(other.customTransformInfoList[j]))
						{
							return true;
						}
					}
				}
				if (!flag)
				{
					return true;
				}
			}
			return false;
		}
	}
}
