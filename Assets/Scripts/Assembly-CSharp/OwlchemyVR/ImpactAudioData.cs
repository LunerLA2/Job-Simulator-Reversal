using System;
using System.Collections.Generic;
using UnityEngine;

namespace OwlchemyVR
{
	[Serializable]
	public class ImpactAudioData
	{
		public enum ImpactAudioTypes
		{
			Impact = 0,
			SlowImpact = 1,
			Slide = 2
		}

		public enum ImpactVsTypes
		{
			WorldItemVsWorldItem = 0,
			WorldItemVsSurfaceType = 1,
			WorldItemVsAnything = 2,
			SurfaceTypeVsSurfaceType = 3,
			SurfaceTypeVsAnything = 4,
			AnythingVsAnything = 5
		}

		private const string TYPE_SEPERATOR = "-";

		[SerializeField]
		private AudioClip[] impactAudioClips = new AudioClip[1];

		[SerializeField]
		private AudioClip[] slowImpactAudioClips = new AudioClip[0];

		[SerializeField]
		private AudioClip slideAudioClip;

		[SerializeField]
		private ImpactVsTypes impactVsType;

		[SerializeField]
		private WorldItemData worldItemData1;

		[SerializeField]
		private WorldItemData worldItemData2;

		[SerializeField]
		private SurfaceTypeData surfaceTypeData1;

		[SerializeField]
		private SurfaceTypeData surfaceTypeData2;

		private int lastRetrievedImpactAudioClipIndex = -1;

		private int lastRetrievedSlowImpactAudioClipIndex = -1;

		public ImpactVsTypes ImpactVsType
		{
			get
			{
				return impactVsType;
			}
		}

		public WorldItemData WorldItemData1
		{
			get
			{
				return worldItemData1;
			}
		}

		public WorldItemData WorldItemData2
		{
			get
			{
				return worldItemData2;
			}
		}

		public SurfaceTypeData SurfaceTypeData1
		{
			get
			{
				return surfaceTypeData1;
			}
		}

		public SurfaceTypeData SurfaceTypeData2
		{
			get
			{
				return surfaceTypeData2;
			}
		}

		public bool DoesAudioClipImpactTypeExist(ImpactAudioTypes impactAudioType)
		{
			switch (impactAudioType)
			{
			case ImpactAudioTypes.Impact:
				return impactAudioClips.Length > 0 && impactAudioClips[0] != null;
			case ImpactAudioTypes.SlowImpact:
				if (slowImpactAudioClips.Length > 0 && slowImpactAudioClips[0] != null)
				{
					return true;
				}
				return impactAudioClips.Length > 0 && impactAudioClips[0] != null;
			case ImpactAudioTypes.Slide:
				return slideAudioClip != null;
			default:
				return false;
			}
		}

		public AudioClip GetAudioClipByType(ImpactAudioTypes impactAudioType)
		{
			switch (impactAudioType)
			{
			case ImpactAudioTypes.Impact:
				return GetRandomImpactAudioClip();
			case ImpactAudioTypes.SlowImpact:
				if (slowImpactAudioClips.Length > 0 && slowImpactAudioClips[0] != null)
				{
					return GetRandomSlowImpactAudioClip();
				}
				return GetRandomImpactAudioClip();
			case ImpactAudioTypes.Slide:
				return slideAudioClip;
			default:
				Debug.LogWarning("Unsupport impactAudioType:" + impactAudioType);
				return null;
			}
		}

		public AudioClip GetAudioClipByTypeAndByIndex(ImpactAudioTypes impactAudioType, int index)
		{
			switch (impactAudioType)
			{
			case ImpactAudioTypes.Impact:
				if (impactAudioClips.Length > index)
				{
					return impactAudioClips[index];
				}
				return null;
			case ImpactAudioTypes.SlowImpact:
				if (slowImpactAudioClips.Length > index)
				{
					return slowImpactAudioClips[index];
				}
				return null;
			default:
				Debug.LogWarning(string.Concat("Impact Audio Type(", impactAudioType, ") does not support by index"));
				return null;
			}
		}

		private AudioClip GetRandomImpactAudioClip()
		{
			if (impactAudioClips.Length == 0)
			{
				return null;
			}
			int num;
			if (lastRetrievedImpactAudioClipIndex < 0 || impactAudioClips.Length <= 2)
			{
				num = UnityEngine.Random.Range(0, impactAudioClips.Length);
			}
			else
			{
				num = UnityEngine.Random.Range(0, impactAudioClips.Length - 1);
				if (num >= lastRetrievedImpactAudioClipIndex)
				{
					num++;
				}
			}
			lastRetrievedImpactAudioClipIndex = num;
			return impactAudioClips[num];
		}

		private AudioClip GetRandomSlowImpactAudioClip()
		{
			if (slowImpactAudioClips.Length == 0)
			{
				return null;
			}
			int num;
			if (lastRetrievedSlowImpactAudioClipIndex < 0 || slowImpactAudioClips.Length <= 2)
			{
				num = UnityEngine.Random.Range(0, slowImpactAudioClips.Length);
			}
			else
			{
				num = UnityEngine.Random.Range(0, slowImpactAudioClips.Length - 1);
				if (num >= lastRetrievedSlowImpactAudioClipIndex)
				{
					num++;
				}
			}
			lastRetrievedSlowImpactAudioClipIndex = num;
			return slowImpactAudioClips[num];
		}

		public void InternalSetWorldItemData1(WorldItemData worldItemData)
		{
			worldItemData1 = worldItemData;
		}

		public void InternalSetWorldItemData2(WorldItemData worldItemData)
		{
			worldItemData2 = worldItemData;
		}

		public void InternalSetSurfaceTypeData1(SurfaceTypeData surfaceTypeData)
		{
			surfaceTypeData1 = surfaceTypeData;
		}

		public void InternalSetSurfaceTypeData2(SurfaceTypeData surfaceTypeData)
		{
			surfaceTypeData2 = surfaceTypeData;
		}

		public void SetImpactAudioClip(AudioClip audioClip, int index, ImpactAudioTypes impactAudioType)
		{
			switch (impactAudioType)
			{
			case ImpactAudioTypes.Impact:
				if (index >= impactAudioClips.Length)
				{
					Array.Resize(ref impactAudioClips, impactAudioClips.Length + 1);
					impactAudioClips[impactAudioClips.Length - 1] = audioClip;
				}
				else
				{
					impactAudioClips[index] = audioClip;
				}
				CleanupAudioClipArray(ref impactAudioClips);
				break;
			case ImpactAudioTypes.SlowImpact:
				if (index >= slowImpactAudioClips.Length)
				{
					Array.Resize(ref slowImpactAudioClips, slowImpactAudioClips.Length + 1);
					slowImpactAudioClips[slowImpactAudioClips.Length - 1] = audioClip;
				}
				else
				{
					slowImpactAudioClips[index] = audioClip;
				}
				CleanupAudioClipArray(ref slowImpactAudioClips);
				break;
			default:
				Debug.LogWarning("Unsupported impact type:" + impactAudioType);
				break;
			}
		}

		public void CleanupAudioClipArray(ref AudioClip[] array)
		{
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			List<AudioClip> list = new List<AudioClip>(array);
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j] == null)
				{
					list.RemoveAt(j);
					j--;
				}
			}
			array = list.ToArray();
		}

		public void SetWorldItemVsWorldItem(WorldItemData worldItemData1, WorldItemData worldItemData2)
		{
			impactVsType = ImpactVsTypes.WorldItemVsWorldItem;
			this.worldItemData1 = worldItemData1;
			this.worldItemData2 = worldItemData2;
			surfaceTypeData1 = null;
			surfaceTypeData2 = null;
		}

		public void SetWorldItemVsSurfaceType(WorldItemData worldItemData1, SurfaceTypeData surfaceTypeData1)
		{
			impactVsType = ImpactVsTypes.WorldItemVsSurfaceType;
			this.worldItemData1 = worldItemData1;
			this.surfaceTypeData1 = surfaceTypeData1;
			worldItemData2 = null;
			surfaceTypeData2 = null;
		}

		public void SetWorldItemVsAnything(WorldItemData worldItemData1)
		{
			impactVsType = ImpactVsTypes.WorldItemVsAnything;
			this.worldItemData1 = worldItemData1;
			worldItemData2 = null;
			surfaceTypeData1 = null;
			surfaceTypeData2 = null;
		}

		public void SetSurfaceTypeVsSurfaceType(SurfaceTypeData surfaceTypeData1, SurfaceTypeData surfaceTypeData2)
		{
			impactVsType = ImpactVsTypes.SurfaceTypeVsSurfaceType;
			this.surfaceTypeData1 = surfaceTypeData1;
			this.surfaceTypeData2 = surfaceTypeData2;
			worldItemData1 = null;
			worldItemData2 = null;
		}

		public void SetSurfaceTypeVsAnything(SurfaceTypeData surfaceTypeData1)
		{
			impactVsType = ImpactVsTypes.SurfaceTypeVsAnything;
			this.surfaceTypeData1 = surfaceTypeData1;
			surfaceTypeData2 = null;
			worldItemData1 = null;
			worldItemData2 = null;
		}

		public void SetAnythingVsAnything()
		{
			impactVsType = ImpactVsTypes.AnythingVsAnything;
			worldItemData1 = null;
			worldItemData2 = null;
			surfaceTypeData1 = null;
			surfaceTypeData2 = null;
		}

		public bool DoesCorrectWorldItemsAndSurfaceTypesDataStillExist()
		{
			if (impactVsType == ImpactVsTypes.WorldItemVsWorldItem)
			{
				if (worldItemData1 == null || worldItemData2 == null)
				{
					return false;
				}
			}
			else if (impactVsType == ImpactVsTypes.WorldItemVsSurfaceType)
			{
				if (worldItemData1 == null || surfaceTypeData1 == null)
				{
					return false;
				}
			}
			else if (impactVsType == ImpactVsTypes.WorldItemVsAnything)
			{
				if (worldItemData1 == null)
				{
					return false;
				}
			}
			else if (impactVsType == ImpactVsTypes.SurfaceTypeVsSurfaceType)
			{
				if (surfaceTypeData1 == null || surfaceTypeData2 == null)
				{
					return false;
				}
			}
			else if (impactVsType == ImpactVsTypes.SurfaceTypeVsAnything)
			{
				if (surfaceTypeData1 == null)
				{
					return false;
				}
			}
			else if (impactVsType != ImpactVsTypes.AnythingVsAnything)
			{
				Debug.LogWarning("Impact vs type not supported");
			}
			return true;
		}

		public string GetKey()
		{
			if (impactVsType == ImpactVsTypes.WorldItemVsWorldItem)
			{
				return GetKeyFromWorldItemVsWorldItem(worldItemData1, worldItemData2);
			}
			if (impactVsType == ImpactVsTypes.WorldItemVsSurfaceType)
			{
				return GetKeyFromWorldItemVsSurfaceType(worldItemData1, surfaceTypeData1);
			}
			if (impactVsType == ImpactVsTypes.WorldItemVsAnything)
			{
				return GetKeyFromWorldItemVsAnything(worldItemData1);
			}
			if (impactVsType == ImpactVsTypes.SurfaceTypeVsSurfaceType)
			{
				return GetKeyFromSurfaceTypeVsSurfaceType(surfaceTypeData1, surfaceTypeData2);
			}
			if (impactVsType == ImpactVsTypes.SurfaceTypeVsAnything)
			{
				return GetKeyFromSurfaceTypeVsAnything(surfaceTypeData1);
			}
			if (impactVsType == ImpactVsTypes.AnythingVsAnything)
			{
				return GetKeyFromAnythingVsAnything();
			}
			Debug.LogWarning("ImpactVSType was not set could not retrieve key");
			return string.Empty;
		}

		public bool DoesContainSurfaceTypeData(SurfaceTypeData surfaceTypeData)
		{
			if (impactVsType == ImpactVsTypes.SurfaceTypeVsSurfaceType)
			{
				if (surfaceTypeData1 == surfaceTypeData || surfaceTypeData2 == surfaceTypeData)
				{
					return true;
				}
			}
			else if (impactVsType == ImpactVsTypes.WorldItemVsSurfaceType)
			{
				if (surfaceTypeData1 == surfaceTypeData)
				{
					return true;
				}
			}
			else if (impactVsType == ImpactVsTypes.SurfaceTypeVsAnything && surfaceTypeData1 == surfaceTypeData)
			{
				return true;
			}
			return false;
		}

		public SurfaceTypeData GetOtherSurfaceTypeData(SurfaceTypeData surfaceTypeData)
		{
			if (impactVsType == ImpactVsTypes.SurfaceTypeVsSurfaceType)
			{
				if (surfaceTypeData1 == surfaceTypeData)
				{
					return surfaceTypeData2;
				}
				if (surfaceTypeData2 == surfaceTypeData)
				{
					return surfaceTypeData1;
				}
				Debug.LogError("Surface type was not in the list, something has gone wrong");
				return null;
			}
			Debug.LogError("Can not get other surface type if you are not using surface type vs surface type");
			return null;
		}

		public bool DoesContainWorldItemData(WorldItemData worldItemData)
		{
			if (impactVsType == ImpactVsTypes.WorldItemVsWorldItem)
			{
				if (worldItemData1 == worldItemData || worldItemData2 == worldItemData)
				{
					return true;
				}
			}
			else if (impactVsType == ImpactVsTypes.WorldItemVsSurfaceType)
			{
				if (worldItemData1 == worldItemData)
				{
					return true;
				}
			}
			else if (impactVsType == ImpactVsTypes.WorldItemVsAnything && worldItemData1 == worldItemData)
			{
				return true;
			}
			return false;
		}

		public static string GetKeyFromWorldItemVsSurfaceType(WorldItemData worldItemData, SurfaceTypeData surfaceTypeData)
		{
			return 1 + "-" + GetIDFromWorldItem(worldItemData) + "-" + GetIDFromSurfaceType(surfaceTypeData);
		}

		public static string GetKeyFromWorldItemVsAnything(WorldItemData worldItemData)
		{
			return 2 + "-" + GetIDFromWorldItem(worldItemData);
		}

		public static string GetKeyFromSurfaceTypeVsSurfaceType(SurfaceTypeData surfaceType1, SurfaceTypeData surfaceType2)
		{
			int num = GetIDFromSurfaceType(surfaceType1);
			int num2 = GetIDFromSurfaceType(surfaceType2);
			if (num2 < num)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			return 3 + "-" + num + "-" + num2;
		}

		public static string GetKeyFromSurfaceTypeVsAnything(SurfaceTypeData surfaceType)
		{
			return 4 + "-" + GetIDFromSurfaceType(surfaceType);
		}

		public static string GetKeyFromWorldItemVsWorldItem(WorldItemData worldItemData1, WorldItemData worldItemData2)
		{
			int num = GetIDFromWorldItem(worldItemData1);
			int num2 = GetIDFromWorldItem(worldItemData2);
			if (num2 < num)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			return 0 + "-" + num + "-" + num2;
		}

		public static string GetKeyFromAnythingVsAnything()
		{
			return 5 + "-";
		}

		public static int GetIDFromWorldItem(WorldItemData worldItemData)
		{
			return worldItemData.GetInstanceID();
		}

		public static int GetIDFromSurfaceType(SurfaceTypeData surfaceTypeData)
		{
			return surfaceTypeData.GetInstanceID();
		}

		public string FormattedToString()
		{
			string text = string.Concat("ImpactType(", impactVsType, "): ");
			if (impactVsType == ImpactVsTypes.WorldItemVsWorldItem)
			{
				if (worldItemData1 == null || worldItemData2 == null)
				{
					text = ((!(worldItemData1 == null)) ? (text + worldItemData1.ItemName + " vs ") : (text + "Undefined World Item Data vs "));
					if (worldItemData2 == null)
					{
						return text + "Undefined World Item Data";
					}
					return text + worldItemData2.ItemName;
				}
				return text + worldItemData1.ItemName + " vs " + worldItemData2.ItemName;
			}
			if (impactVsType == ImpactVsTypes.WorldItemVsSurfaceType)
			{
				if (worldItemData1 == null || surfaceTypeData1 == null)
				{
					text = ((!(worldItemData1 == null)) ? (text + worldItemData1.ItemName + " vs ") : (text + "Undefined World Item Data vs "));
					if (surfaceTypeData1 == null)
					{
						return text + "Undefined Surface Type Data";
					}
					return text + surfaceTypeData1.SurfaceTypeName;
				}
				return text + worldItemData1.ItemName + " vs " + surfaceTypeData1.SurfaceTypeName;
			}
			if (impactVsType == ImpactVsTypes.WorldItemVsAnything)
			{
				if (worldItemData1 == null)
				{
					return text + "Undefined World Item Data vs Anything";
				}
				return text + worldItemData1.ItemName + " vs Anything";
			}
			if (impactVsType == ImpactVsTypes.SurfaceTypeVsSurfaceType)
			{
				if (surfaceTypeData1 == null || surfaceTypeData2 == null)
				{
					text = ((!(surfaceTypeData1 == null)) ? (text + surfaceTypeData1.SurfaceTypeName + " vs ") : (text + "Undefined Surface Type Data vs "));
					if (surfaceTypeData2 == null)
					{
						return text + "Undefined Surface Type Data";
					}
					return text + surfaceTypeData2.SurfaceTypeName;
				}
				return text + surfaceTypeData1.SurfaceTypeName + " vs " + surfaceTypeData2.SurfaceTypeName;
			}
			if (impactVsType == ImpactVsTypes.SurfaceTypeVsAnything)
			{
				if (surfaceTypeData1 == null)
				{
					return text + "Undefined Surface Type Data vs Anything";
				}
				return text + surfaceTypeData1.SurfaceTypeName + " vs Anything";
			}
			if (impactVsType == ImpactVsTypes.AnythingVsAnything)
			{
				return text + "Anything vs Anything";
			}
			Debug.LogError("Unrecognized impactvstype:" + impactVsType);
			return text + string.Empty;
		}

		public override string ToString()
		{
			string text = "ImpactAudioData:" + impactVsType;
			if (worldItemData1 != null)
			{
				text = text + "WorldItemData1:" + worldItemData1.ItemName + ",";
			}
			if (worldItemData2 != null)
			{
				text = text + "WorldItemData2:" + worldItemData2.ItemName + ",";
			}
			if (surfaceTypeData1 != null)
			{
				text = text + "SurfaceTypeData1:" + surfaceTypeData1.SurfaceTypeName + ",";
			}
			if (surfaceTypeData2 != null)
			{
				text = text + "SurfaceTypeData2:" + surfaceTypeData2.SurfaceTypeName + ",";
			}
			return text;
		}
	}
}
