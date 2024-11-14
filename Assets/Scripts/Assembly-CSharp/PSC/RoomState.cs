using System.Collections.Generic;
using UnityEngine;

namespace PSC
{
	[ExecuteInEditMode]
	public class RoomState : MonoBehaviour
	{
		private static IEnumerable<RoomState> m_CachedStates;

		[SerializeField]
		private LayoutConfiguration m_ActiveLayout;

		[SerializeField]
		private LayoutConfiguration m_BuildLayoutConfiguration;

		[SerializeField]
		private bool m_BuildWasStarted;

		[SerializeField]
		private string m_ID;

		[SerializeField]
		private bool m_HasSceneBuildError;

		public LayoutConfiguration activeConfiguration
		{
			get
			{
				return m_ActiveLayout;
			}
			set
			{
				m_ActiveLayout = value;
			}
		}

		public bool buildWasStarted
		{
			get
			{
				return m_BuildWasStarted;
			}
		}

		public bool hasSceneBuildError
		{
			get
			{
				return m_HasSceneBuildError;
			}
			set
			{
				m_HasSceneBuildError = value;
			}
		}

		public string ID
		{
			get
			{
				return m_ID;
			}
			set
			{
				m_ID = value;
			}
		}

		public static RoomState Get(Room context)
		{
			if (m_CachedStates != null)
			{
				foreach (RoomState cachedState in m_CachedStates)
				{
					if (cachedState.ID == context.ID)
					{
						return cachedState;
					}
				}
			}
			List<RoomState> list = new List<RoomState>(Resources.FindObjectsOfTypeAll<RoomState>());
			foreach (RoomState item in list)
			{
				if (item.ID == context.ID)
				{
					m_CachedStates = list;
					return item;
				}
			}
			GameObject gameObject = new GameObject("Room State " + context.ID);
			RoomState roomState = gameObject.AddComponent<RoomState>();
			roomState.ID = context.ID;
			list.Add(roomState);
			m_CachedStates = list;
			return roomState;
		}

		public void SetPostBuildConfigurationToLoad(LayoutConfiguration config)
		{
			m_BuildWasStarted = true;
			m_BuildLayoutConfiguration = config;
		}

		public LayoutConfiguration GetAndResetLastBuildConfiguration()
		{
			m_BuildWasStarted = false;
			LayoutConfiguration buildLayoutConfiguration = m_BuildLayoutConfiguration;
			m_BuildLayoutConfiguration = null;
			return buildLayoutConfiguration;
		}

		private void Start()
		{
			base.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
			base.gameObject.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
		}
	}
}
