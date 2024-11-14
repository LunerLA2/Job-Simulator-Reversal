using UnityEngine;

namespace PSC
{
	public static class Styles
	{
		private static bool m_StylesInit;

		private static GUIStyle m_EditTitle;

		private static GUIStyle m_ToolbarArrowButtonStyle;

		private static GUIStyle m_RecordingWindowFrame;

		private static GUIStyle m_ViewingWindowFrame;

		private static GUIStyle m_RevertButtonStyle;

		private static GUIContent m_SettingsIcon;

		public static GUIContent settingsIcon
		{
			get
			{
				InitStyles();
				return m_SettingsIcon;
			}
		}

		public static GUIStyle revertButtonStyle
		{
			get
			{
				InitStyles();
				return m_RevertButtonStyle;
			}
		}

		public static GUIStyle recordingWindowFrame
		{
			get
			{
				InitStyles();
				return m_RecordingWindowFrame;
			}
		}

		public static GUIStyle viewingWindowFrame
		{
			get
			{
				InitStyles();
				return m_ViewingWindowFrame;
			}
		}

		public static GUIStyle editTitle
		{
			get
			{
				InitStyles();
				return m_EditTitle;
			}
		}

		public static GUIStyle toolbarArrowButtonStyle
		{
			get
			{
				InitStyles();
				return m_ToolbarArrowButtonStyle;
			}
		}

		private static void InitStyles()
		{
			if (!m_StylesInit)
			{
				m_StylesInit = true;
				m_EditTitle = new GUIStyle("flow node 2");
				m_EditTitle.alignment = TextAnchor.MiddleCenter;
				m_EditTitle.fontSize = 12;
				m_EditTitle.fontStyle = FontStyle.Bold;
				m_EditTitle.contentOffset = Vector2.zero;
				m_EditTitle.padding = new RectOffset(5, 5, 5, 5);
				m_EditTitle.margin = new RectOffset(0, 0, 5, 5);
				m_RevertButtonStyle = new GUIStyle(GUI.skin.button);
				m_RevertButtonStyle.fontStyle = FontStyle.Bold;
				m_RevertButtonStyle.fontSize = 25;
				m_ToolbarArrowButtonStyle.fontSize = 20;
				m_RecordingWindowFrame = new GUIStyle("flow node 6 on");
				m_ViewingWindowFrame = new GUIStyle("flow node 1 on");
			}
		}
	}
}
