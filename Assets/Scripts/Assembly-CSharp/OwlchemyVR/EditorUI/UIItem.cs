using UnityEngine;
using UnityEngine.Events;

namespace OwlchemyVR.EditorUI
{
	public class UIItem : MonoBehaviour
	{
		public class UIItemEvent : UnityEvent<UIItem, HandUIPointerController>
		{
		}

		public UIItemEvent OnPress = new UIItemEvent();

		public UIItemEvent OnRelease = new UIItemEvent();

		public UIItemEvent OnHoverOver = new UIItemEvent();

		public UIItemEvent OnHoverOut = new UIItemEvent();

		private bool isCurrHover;

		private bool isCurrDown;

		public bool IsCurrHover
		{
			get
			{
				return isCurrHover;
			}
		}

		public bool IsCurrDown
		{
			get
			{
				return isCurrDown;
			}
		}

		public void PressMessage(HandUIPointerController uiPointerController)
		{
			if (!isCurrDown)
			{
				isCurrDown = true;
				Press(uiPointerController);
			}
			else
			{
				Debug.LogWarning("Unable to press because it was already pressed, most likely a bug in the ui system");
			}
		}

		public void ReleaseMessage(HandUIPointerController uiPointerController)
		{
			if (isCurrDown)
			{
				isCurrDown = false;
				Release(uiPointerController);
			}
			else
			{
				Debug.LogWarning("Unable to release because it wasn't pressed, most likely a bug in the ui system");
			}
		}

		public void HoverOverMessage(HandUIPointerController uiPointerController)
		{
			if (!isCurrHover)
			{
				isCurrHover = true;
				HoverOver(uiPointerController);
			}
			else
			{
				Debug.LogWarning("Unable to hoverover because already hovered over");
			}
		}

		public void HoverOutMessage(HandUIPointerController uiPointerController)
		{
			if (isCurrHover)
			{
				isCurrHover = false;
				HoverOut(uiPointerController);
			}
			else
			{
				Debug.LogWarning("Unable to hoverover because already hovered over");
			}
		}

		protected virtual void Press(HandUIPointerController uiPointerController)
		{
			OnPress.Invoke(this, uiPointerController);
		}

		protected virtual void Release(HandUIPointerController uiPointerController)
		{
			OnRelease.Invoke(this, uiPointerController);
		}

		protected virtual void HoverOver(HandUIPointerController uiPointerController)
		{
			OnHoverOver.Invoke(this, uiPointerController);
		}

		protected virtual void HoverOut(HandUIPointerController uiPointerController)
		{
			OnHoverOut.Invoke(this, uiPointerController);
		}
	}
}
