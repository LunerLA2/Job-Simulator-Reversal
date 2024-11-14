using System.Collections.Generic;
using UnityEngine;

namespace OwlchemyVR
{
	public class ImpactAudioMatrix : ScriptableObject
	{
		[SerializeField]
		private List<SurfaceTypeData> surfaceTypeAudioPriority;

		[SerializeField]
		private List<ImpactAudioData> impactAudioDataCollection;

		private Dictionary<string, ImpactAudioData> impactAudioDictionary;

		private Dictionary<SurfaceTypeData, int> surfaceTypeAudioPriorityDictionary;

		public List<SurfaceTypeData> SurfaceTypeAudioPriority
		{
			get
			{
				return surfaceTypeAudioPriority;
			}
		}

		public List<ImpactAudioData> ImpactAudioDataCollection
		{
			get
			{
				return impactAudioDataCollection;
			}
		}

		private void OnEnable()
		{
			Init();
		}

		public void Init()
		{
			impactAudioDictionary = new Dictionary<string, ImpactAudioData>();
			for (int i = 0; i < impactAudioDataCollection.Count; i++)
			{
				impactAudioDictionary.Add(impactAudioDataCollection[i].GetKey(), impactAudioDataCollection[i]);
			}
			surfaceTypeAudioPriorityDictionary = new Dictionary<SurfaceTypeData, int>();
			for (int j = 0; j < surfaceTypeAudioPriority.Count; j++)
			{
				surfaceTypeAudioPriorityDictionary.Add(surfaceTypeAudioPriority[j], surfaceTypeAudioPriority.Count - j);
			}
		}

		public ImpactAudioData GetAppropriateImpactAudioData(ImpactAudioData.ImpactAudioTypes impactAudioType, WorldItemData worldItemData, WorldItemData otherWorldItemData, SurfaceTypeData otherSurfaceItemData)
		{
			ImpactAudioData impactAudioData = null;
			if (worldItemData == null)
			{
				return null;
			}
			SurfaceTypeData surfaceTypeData = worldItemData.SurfaceTypeData;
			if (!(otherWorldItemData == null) || otherSurfaceItemData == null)
			{
			}
			impactAudioData = GetImpactAudioDataByWorldItemVsWorldItem(worldItemData, otherWorldItemData);
			impactAudioData = CheckIfContainsImpactAudioType(impactAudioData, impactAudioType);
			if (impactAudioData != null)
			{
				return impactAudioData;
			}
			impactAudioData = GetPriorityImpactAudioData(GetImpactAudioDataByWorldItemVsSurfaceType(worldItemData, otherSurfaceItemData), GetImpactAudioDataByWorldItemVsSurfaceType(otherWorldItemData, surfaceTypeData), surfaceTypeData, otherSurfaceItemData);
			impactAudioData = CheckIfContainsImpactAudioType(impactAudioData, impactAudioType);
			if (impactAudioData != null)
			{
				return impactAudioData;
			}
			impactAudioData = GetPriorityImpactAudioData(GetImpactAudioDataByWorldItemVsAnything(worldItemData), GetImpactAudioDataByWorldItemVsAnything(otherWorldItemData), surfaceTypeData, otherSurfaceItemData);
			impactAudioData = CheckIfContainsImpactAudioType(impactAudioData, impactAudioType);
			if (impactAudioData != null)
			{
				return impactAudioData;
			}
			impactAudioData = GetImpactAudioDataBySurfaceTypeVsSurfaceType(surfaceTypeData, otherSurfaceItemData);
			impactAudioData = CheckIfContainsImpactAudioType(impactAudioData, impactAudioType);
			if (impactAudioData != null)
			{
				return impactAudioData;
			}
			impactAudioData = GetPriorityImpactAudioData(GetImpactAudioDataBySurfaceTypeVsAnything(surfaceTypeData), GetImpactAudioDataBySurfaceTypeVsAnything(otherSurfaceItemData), surfaceTypeData, otherSurfaceItemData);
			impactAudioData = CheckIfContainsImpactAudioType(impactAudioData, impactAudioType);
			if (impactAudioData != null)
			{
				return impactAudioData;
			}
			impactAudioData = GetImpactAudioDataByAnythingVsAnything();
			return CheckIfContainsImpactAudioType(impactAudioData, impactAudioType);
		}

		private ImpactAudioData CheckIfContainsImpactAudioType(ImpactAudioData impactAudioData, ImpactAudioData.ImpactAudioTypes impactAudioType)
		{
			if (impactAudioData == null)
			{
				return null;
			}
			if (impactAudioData.DoesAudioClipImpactTypeExist(impactAudioType))
			{
				return impactAudioData;
			}
			return null;
		}

		private ImpactAudioData GetPriorityImpactAudioData(ImpactAudioData impactAudioData1, ImpactAudioData impactAudioData2, SurfaceTypeData surfaceTypeData1, SurfaceTypeData surfaceTypeData2)
		{
			if (impactAudioData1 != null && impactAudioData2 == null)
			{
				return impactAudioData1;
			}
			if (impactAudioData2 != null && impactAudioData1 == null)
			{
				return impactAudioData2;
			}
			if (impactAudioData1 != null && impactAudioData2 != null)
			{
				if (GetSurfaceTypePriority(surfaceTypeData1) >= GetSurfaceTypePriority(surfaceTypeData2))
				{
					return impactAudioData1;
				}
				return impactAudioData2;
			}
			return null;
		}

		public int GetSurfaceTypePriority(SurfaceTypeData surfaceTypeData)
		{
			int value = 0;
			if (surfaceTypeData != null)
			{
				surfaceTypeAudioPriorityDictionary.TryGetValue(surfaceTypeData, out value);
			}
			return value;
		}

		public void AddNewSurfaceTypeVsSurfaceTypeImpactAudioData(SurfaceTypeData surfaceTypeData1, SurfaceTypeData surfaceTypeData2)
		{
			ImpactAudioData impactAudioData = new ImpactAudioData();
			impactAudioData.SetSurfaceTypeVsSurfaceType(surfaceTypeData1, surfaceTypeData2);
			impactAudioDataCollection.Add(impactAudioData);
		}

		public ImpactAudioData GetImpactAudioDataBySurfaceTypeVsSurfaceType(SurfaceTypeData surfaceTypeData1, SurfaceTypeData surfaceTypeData2)
		{
			ImpactAudioData value = null;
			if (surfaceTypeData1 != null && surfaceTypeData2 != null)
			{
				impactAudioDictionary.TryGetValue(ImpactAudioData.GetKeyFromSurfaceTypeVsSurfaceType(surfaceTypeData1, surfaceTypeData2), out value);
			}
			return value;
		}

		public ImpactAudioData GetImpactAudioDataBySurfaceTypeVsAnything(SurfaceTypeData surfaceTypeData)
		{
			ImpactAudioData value = null;
			if (surfaceTypeData != null)
			{
				impactAudioDictionary.TryGetValue(ImpactAudioData.GetKeyFromSurfaceTypeVsAnything(surfaceTypeData), out value);
			}
			return value;
		}

		public ImpactAudioData GetImpactAudioDataByWorldItemVsSurfaceType(WorldItemData worldItemData, SurfaceTypeData surfaceTypeData)
		{
			ImpactAudioData value = null;
			if (worldItemData != null && surfaceTypeData != null)
			{
				impactAudioDictionary.TryGetValue(ImpactAudioData.GetKeyFromWorldItemVsSurfaceType(worldItemData, surfaceTypeData), out value);
			}
			return value;
		}

		public ImpactAudioData GetImpactAudioDataByAnythingVsAnything()
		{
			ImpactAudioData value;
			impactAudioDictionary.TryGetValue(ImpactAudioData.GetKeyFromAnythingVsAnything(), out value);
			return value;
		}

		public ImpactAudioData GetImpactAudioDataByWorldItemVsWorldItem(WorldItemData worldItemData1, WorldItemData worldItemData2)
		{
			ImpactAudioData value = null;
			if (worldItemData1 != null && worldItemData2 != null)
			{
				impactAudioDictionary.TryGetValue(ImpactAudioData.GetKeyFromWorldItemVsWorldItem(worldItemData1, worldItemData2), out value);
			}
			return value;
		}

		public ImpactAudioData GetImpactAudioDataByWorldItemVsAnything(WorldItemData worldItemData)
		{
			ImpactAudioData value = null;
			if (worldItemData != null)
			{
				impactAudioDictionary.TryGetValue(ImpactAudioData.GetKeyFromWorldItemVsAnything(worldItemData), out value);
			}
			return value;
		}

		public ImpactAudioData AddNewWorldItemVsSurfaceTypeImpactAudioData(WorldItemData worldItemData, SurfaceTypeData surfaceTypeData)
		{
			string keyFromWorldItemVsSurfaceType = ImpactAudioData.GetKeyFromWorldItemVsSurfaceType(worldItemData, surfaceTypeData);
			ImpactAudioData impactAudioData;
			if (!impactAudioDictionary.ContainsKey(keyFromWorldItemVsSurfaceType))
			{
				impactAudioData = new ImpactAudioData();
				impactAudioData.SetWorldItemVsSurfaceType(worldItemData, surfaceTypeData);
				impactAudioDictionary.Add(keyFromWorldItemVsSurfaceType, impactAudioData);
			}
			else
			{
				impactAudioData = null;
				Debug.LogWarning("Could not add new world item vs surface as it already existed:" + keyFromWorldItemVsSurfaceType);
			}
			return impactAudioData;
		}
	}
}
