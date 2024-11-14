using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PSC
{
	[ExecuteInEditMode]
	public class Room : MonoBehaviour, IRoomEditor
	{
		[SerializeField]
		private List<Layout> m_Layouts;

		[SerializeField]
		private RoomVisualizer m_Visualizer;

		private bool m_SingleStepLayout = true;

		private bool m_IsRecording;

		private SerializedScene m_SerializedScene = new SerializedScene();

		public static LayoutConfiguration m_DefaultLayoutToLoad;

		public static RoomLayoutChangedDelegate m_OnRoomChanged;

		private LayoutConfiguration m_Configuration;

		private static Room m_activeRoom;

		[SerializeField]
		private string m_ID;

		public static Room activeRoom
		{
			get
			{
				return m_activeRoom;
			}
		}

		public static bool currentSceneHasActiveRoom
		{
			get
			{
				return m_activeRoom != null;
			}
		}

		public static bool hasActiveConfiguration
		{
			get
			{
				if (currentSceneHasActiveRoom)
				{
					return activeRoom.configuration != null;
				}
				return false;
			}
		}

		public static LayoutConfiguration defaultLayoutToLoad
		{
			get
			{
				return m_DefaultLayoutToLoad;
			}
			set
			{
				m_DefaultLayoutToLoad = value;
			}
		}

		public bool isRecording
		{
			get
			{
				return m_IsRecording;
			}
			set
			{
				m_IsRecording = value;
			}
		}

		public Scene scene
		{
			get
			{
				return base.gameObject.scene;
			}
		}

		public string ID
		{
			get
			{
				return m_ID;
			}
		}

		public RoomVisualizer visualizer
		{
			get
			{
				return m_Visualizer;
			}
			set
			{
				m_Visualizer = value;
			}
		}

		public bool hasVisualizer
		{
			get
			{
				return m_Visualizer != null;
			}
		}

		public LayoutConfiguration configuration
		{
			get
			{
				return m_Configuration;
			}
		}

		public Layout activeLayout
		{
			get
			{
				return GetLayoutFromConfiguration(configuration);
			}
		}

		public List<Layout> layouts
		{
			get
			{
				return m_Layouts;
			}
			set
			{
				m_Layouts = value;
			}
		}

		public static event RoomLayoutChangedDelegate OnRoomLayoutChanged
		{
			add
			{
				m_OnRoomChanged = (RoomLayoutChangedDelegate)Delegate.Combine(m_OnRoomChanged, m_OnRoomChanged);
			}
			remove
			{
				m_OnRoomChanged = (RoomLayoutChangedDelegate)Delegate.Remove(m_OnRoomChanged, m_OnRoomChanged);
			}
		}

		public Layout GetLayoutFromConfiguration(LayoutConfiguration configuration)
		{
			if (layouts != null)
			{
				for (int i = 0; i < layouts.Count; i++)
				{
					if (layouts[i].config == configuration)
					{
						return layouts[i];
					}
				}
			}
			else
			{
				layouts = new List<Layout>();
			}
			return null;
		}

		public void Awake()
		{
			if (!Application.isEditor && defaultLayoutToLoad != null)
			{
				LoadLayout(defaultLayoutToLoad);
			}
		}

		public void OnEnable()
		{
			if (string.IsNullOrEmpty(m_ID))
			{
				m_ID = scene.name;
			}
			if (m_activeRoom == null)
			{
				m_activeRoom = this;
			}
			else
			{
				Debug.LogWarning("Two instances of Room are in the current scene. Please delete the other one.");
			}
		}

		public void OnDisable()
		{
		}

		public Layout CreateNewLayout(LayoutConfiguration config)
		{
			Layout layout = new Layout();
			layout.config = config;
			layouts.Add(layout);
			return layout;
		}

		private void LoadLayoutTwoStep(LayoutConfiguration config)
		{
			if (Application.isPlaying)
			{
				StartCoroutine(LoadLayoutRoutine(config));
			}
			else
			{
				Debug.LogError("LoadLayoutTwoStep can only work at runtime since it uses a coroutine. Please use LoadLayout for edit time logic");
			}
		}

		private IEnumerator LoadLayoutRoutine(LayoutConfiguration config)
		{
			m_Configuration = config;
			if (visualizer != null)
			{
				visualizer.OnLayoutChanged(config);
			}
			if (config != null)
			{
				Layout layout = GetLayoutFromConfiguration(config);
				if (layout != null)
				{
					layout.LoadPhaseOne();
					yield return new WaitForEndOfFrame();
					layout.LoadPhaseTwo();
				}
				else
				{
					Debug.Log("Layout is null");
				}
			}
		}

		public void LoadLayout(LayoutConfiguration config)
		{
			LayoutConfiguration from = m_Configuration;
			m_Configuration = config;
			if (m_SingleStepLayout || !Application.isPlaying)
			{
				if (visualizer != null)
				{
					visualizer.OnLayoutChanged(m_Configuration);
				}
				if (config != null)
				{
					Layout layoutFromConfiguration = GetLayoutFromConfiguration(config);
					if (layoutFromConfiguration != null)
					{
						layoutFromConfiguration.Apply(Placement.MemberTypes.ActiveState);
					}
					else
					{
						layoutFromConfiguration = CreateNewLayout(configuration);
					}
				}
			}
			else
			{
				LoadLayoutTwoStep(config);
			}
			if (m_OnRoomChanged != null)
			{
				m_OnRoomChanged(this, from, m_Configuration);
			}
		}
	}
}
