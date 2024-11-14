using System;
using UnityEngine;

namespace OwlchemyVR.EditorUI
{
	public class UIComponentFieldController : SimpleButton
	{
		[SerializeField]
		private Transform fieldContainer;

		[SerializeField]
		private InputFieldUIController inputFieldUIControllerPrefab;

		[SerializeField]
		private LabelFieldUIController labelFieldUIControllerPrefab;

		private Action<string> OnValueChange;

		private FieldOrPropertyInfo fieldOrPropInfo;

		private FieldUIController fieldUIController;

		public FieldOrPropertyInfo FieldOrPropInfo
		{
			get
			{
				return fieldOrPropInfo;
			}
		}

		public void Init(FieldOrPropertyInfo fieldOrPropInfo)
		{
			this.fieldOrPropInfo = fieldOrPropInfo;
			labelText.text = fieldOrPropInfo.Name;
			Type fieldPropType = fieldOrPropInfo.GetFieldPropType();
			FieldUIController fieldUIController = null;
			fieldUIController = ((fieldPropType == typeof(float)) ? inputFieldUIControllerPrefab : ((fieldPropType == typeof(string)) ? labelFieldUIControllerPrefab : ((fieldPropType != typeof(int)) ? ((FieldUIController)labelFieldUIControllerPrefab) : ((FieldUIController)inputFieldUIControllerPrefab))));
			this.fieldUIController = CreateCustomField(fieldUIController);
			this.fieldUIController.Init(fieldOrPropInfo);
		}

		private FieldUIController CreateCustomField(FieldUIController fieldUIControllerPrefab)
		{
			FieldUIController fieldUIController = UnityEngine.Object.Instantiate(fieldUIControllerPrefab);
			fieldUIController.transform.SetParent(fieldContainer, false);
			return fieldUIController;
		}
	}
}
