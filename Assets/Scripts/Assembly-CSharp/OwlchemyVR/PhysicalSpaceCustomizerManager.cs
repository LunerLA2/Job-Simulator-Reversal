using System.Collections.Generic;
using UnityEngine;

namespace OwlchemyVR
{
	public class PhysicalSpaceCustomizerManager : MonoBehaviour
	{
		[SerializeField]
		public List<PhysicalSpaceLayoutObject> layouts = new List<PhysicalSpaceLayoutObject>();

		private void Awake()
		{
			if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.PSVR)
			{
				ApplyLayout(PhysicalSpaceLayoutObject.PhysicalSpaceTypes.Morpheus);
			}
			if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.Oculus)
			{
				ApplyLayout(PhysicalSpaceLayoutObject.PhysicalSpaceTypes.Oculus);
			}
		}

		public PhysicalSpaceLayoutObject GetLayoutByType(PhysicalSpaceLayoutObject.PhysicalSpaceTypes type)
		{
			for (int i = 0; i < layouts.Count; i++)
			{
				if (layouts[i].PhysicalSpaceType == type)
				{
					return layouts[i];
				}
			}
			return null;
		}

		public void SetLayout(PhysicalSpaceLayoutObject layoutObject)
		{
			for (int i = 0; i < layouts.Count; i++)
			{
				if (layouts[i].PhysicalSpaceType == layoutObject.PhysicalSpaceType)
				{
					layouts[i] = layoutObject;
					return;
				}
			}
			layouts.Add(layoutObject);
		}

		public void ApplyLayout(PhysicalSpaceLayoutObject.PhysicalSpaceTypes layoutType)
		{
			PhysicalSpaceLayoutObject layoutByType = GetLayoutByType(layoutType);
			if (layoutByType == null)
			{
				return;
			}
			for (int i = 0; i < layoutByType.CustomTransformInfoList.Count; i++)
			{
				if (layoutByType.CustomTransformInfoList[i].Transform != null)
				{
					layoutByType.CustomTransformInfoList[i].Apply();
					continue;
				}
				Debug.LogError("Could not find transform related to custom layout, array item #" + i + ", layout Save Required to clean up data");
				layoutByType.CustomTransformInfoList.RemoveAt(i);
				i--;
			}
		}
	}
}
