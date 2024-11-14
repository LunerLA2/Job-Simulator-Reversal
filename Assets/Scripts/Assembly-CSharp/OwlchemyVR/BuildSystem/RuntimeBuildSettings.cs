using System;
using UnityEngine;

namespace OwlchemyVR.BuildSystem
{
	[Serializable]
	public class RuntimeBuildSettings : ScriptableObject
	{
		private const string ASSET_PATH = "RuntimeBuildSettings";

		[SerializeField]
		private bool useBakedScenes;

		private static RuntimeBuildSettings instance;

		public static RuntimeBuildSettings Instance
		{
			get
			{
				if (instance == null)
				{
					LoadInstance(true);
				}
				return instance;
			}
		}

		public static bool UseBakedScenes
		{
			get
			{
				if (instance == null)
				{
					LoadInstance();
					if (instance != null)
					{
						return instance.useBakedScenes;
					}
					return false;
				}
				return instance.useBakedScenes;
			}
			set
			{
				if (Application.isEditor && !Application.isPlaying && instance == null)
				{
					LoadInstance(true);
					if (instance != null)
					{
						instance.useBakedScenes = value;
					}
				}
			}
		}

		private static void LoadInstance(bool createIfNotExist = false)
		{
			instance = Resources.Load<RuntimeBuildSettings>("RuntimeBuildSettings");
			if (instance == null && !createIfNotExist)
			{
				Debug.LogWarning("Could not find the RuntimeBuildSettings asset, so baked scenes will NOT be loaded. Looked at this path: Assets/Resources/RuntimeBuildSettings");
			}
		}
	}
}
