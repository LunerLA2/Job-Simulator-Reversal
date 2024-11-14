using UnityEngine;
using UnityEngine.Audio;

namespace OwlchemyVR
{
	public class GameSettings : MonoBehaviour
	{
		[SerializeField]
		private Color highlightColor;

		[SerializeField]
		private Color interactColor;

		[SerializeField]
		private ModelOutlineContainer modelOutlinePrefab;

		[SerializeField]
		private ImpactAudioMatrix impactAudioMatrix;

		[SerializeField]
		private AudioMixer spatializerMixer;

		[SerializeField]
		private AudioMixerSnapshot defaultSnapshot;

		[SerializeField]
		private PhysicMaterial rubberModePhysicMaterial;

		[SerializeField]
		private ImpactAudioData rubberModeCustomImpactAudioData;

		private static GameSettings _instance;

		public Color HighlightColor
		{
			get
			{
				return highlightColor;
			}
		}

		public Color InteractColor
		{
			get
			{
				return interactColor;
			}
		}

		public ModelOutlineContainer ModelOutlinePrefab
		{
			get
			{
				return modelOutlinePrefab;
			}
		}

		public ImpactAudioMatrix ImpactAudioMatrix
		{
			get
			{
				return impactAudioMatrix;
			}
		}

		public PhysicMaterial RubberModePhysicMaterial
		{
			get
			{
				return rubberModePhysicMaterial;
			}
		}

		public ImpactAudioData RubberModeCustomImpactAudioData
		{
			get
			{
				return rubberModeCustomImpactAudioData;
			}
		}

		public static GameSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Object.FindObjectOfType(typeof(GameSettings)) as GameSettings;
					if (_instance == null)
					{
						GameSettings gameSettings = LoadGameSettingsPrefabDirectly();
						_instance = ((GameObject)Object.Instantiate(gameSettings.gameObject, Vector3.zero, Quaternion.identity)).GetComponent<GameSettings>();
						_instance.gameObject.RemoveCloneFromName();
					}
				}
				return _instance;
			}
		}

		public static GameSettings LoadGameSettingsPrefabDirectly()
		{
			return Resources.Load<GameSettings>("GameSettings");
		}

		private void Awake()
		{
			if (_instance == null)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
			else if (_instance != this)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
