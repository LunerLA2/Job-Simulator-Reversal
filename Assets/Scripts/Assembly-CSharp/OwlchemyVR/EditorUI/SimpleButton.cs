using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OwlchemyVR.EditorUI
{
	public class SimpleButton : UIItem
	{
		public class SimpleButtonEvent : UnityEvent<SimpleButton>
		{
		}

		[SerializeField]
		private string id;

		[SerializeField]
		protected Text labelText;

		[SerializeField]
		private Image background;

		private Color selectedColor = Color.yellow;

		private Color hoverColor = Color.white;

		public SimpleButtonEvent OnSelect = new SimpleButtonEvent();

		public SimpleButtonEvent OnDeselect = new SimpleButtonEvent();

		[SerializeField]
		private bool useManualSelection;

		private bool isSelected;

		private Color defaultColor;

		public string ID
		{
			get
			{
				return id;
			}
		}

		private void Awake()
		{
			defaultColor = background.color;
		}

		public void SetLabel(string labelStr)
		{
			labelText.text = labelStr;
		}

		protected override void HoverOver(HandUIPointerController uiPointerController)
		{
			base.HoverOver(uiPointerController);
			if (useManualSelection)
			{
				if (!isSelected)
				{
					background.color = hoverColor;
				}
			}
			else if (!base.IsCurrDown)
			{
				background.color = hoverColor;
			}
		}

		protected override void HoverOut(HandUIPointerController uiPointerController)
		{
			base.HoverOut(uiPointerController);
			if (useManualSelection)
			{
				if (!isSelected)
				{
					background.color = defaultColor;
				}
			}
			else if (!base.IsCurrDown)
			{
				background.color = defaultColor;
			}
		}

		protected override void Press(HandUIPointerController uiPointerController)
		{
			base.Press(uiPointerController);
			if (!useManualSelection)
			{
				Select();
			}
		}

		protected override void Release(HandUIPointerController uiPointerController)
		{
			base.Release(uiPointerController);
			if (!useManualSelection)
			{
				Deselect();
			}
		}

		public void Select()
		{
			if (!isSelected)
			{
				isSelected = true;
				background.color = selectedColor;
				OnSelect.Invoke(this);
			}
		}

		public void Deselect()
		{
			if (isSelected)
			{
				isSelected = false;
				if (base.IsCurrHover)
				{
					background.color = hoverColor;
				}
				else
				{
					background.color = defaultColor;
				}
				OnDeselect.Invoke(this);
			}
		}
	}
}
