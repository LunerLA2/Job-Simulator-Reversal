using UnityEngine;
using UnityEngine.UI;

namespace OwlchemyVR.EditorUI
{
	public class InputFieldUIController : FieldUIController
	{
		[SerializeField]
		private InputField inputField;

		public override void RefreshValues()
		{
			base.RefreshValues();
			inputField.text = fieldOrPropInfo.GetValueAsString();
		}

		public override void OnEnable()
		{
			base.OnEnable();
			inputField.onValueChanged.AddListener(InputFieldValueChange);
		}

		public override void OnDisable()
		{
			base.OnDisable();
			inputField.onValueChanged.RemoveListener(InputFieldValueChange);
		}

		private void InputFieldValueChange(string value)
		{
			if (valueObj != fieldOrPropInfo.GetValue())
			{
				fieldOrPropInfo.SetValueWithString(value);
			}
			RefreshValues();
		}

		public override void Update()
		{
			base.Update();
		}
	}
}
