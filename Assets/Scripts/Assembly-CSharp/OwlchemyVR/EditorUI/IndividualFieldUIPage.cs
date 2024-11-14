using System;
using UnityEngine;
using UnityEngine.UI;

namespace OwlchemyVR.EditorUI
{
	public class IndividualFieldUIPage : UIPage
	{
		[SerializeField]
		private Text titleText;

		[SerializeField]
		private Transform contentContainer;

		[SerializeField]
		private NumericalDataEntryUIController numericalDataEntryUIControllerPrefab;

		private DataEntryUIController currDataEntryUIController;

		public override void Open()
		{
			if (!base.IsPageOpen)
			{
				base.gameObject.SetActive(true);
			}
			base.IsPageOpen = true;
		}

		public override void Close()
		{
			if (base.IsPageOpen)
			{
				if (currDataEntryUIController != null)
				{
					UnityEngine.Object.Destroy(currDataEntryUIController.gameObject);
				}
				currDataEntryUIController = null;
				base.gameObject.SetActive(false);
			}
			base.IsPageOpen = false;
		}

		public bool IsDataTypeSupported(Type dataType)
		{
			if (dataType == typeof(float) || dataType == typeof(int) || dataType == typeof(Vector3))
			{
				return true;
			}
			return false;
		}

		public void Init(FieldOrPropertyInfo fieldOrPropInfo, HandUIPointerController uiPointerController)
		{
			titleText.text = fieldOrPropInfo.Name;
			if (currDataEntryUIController != null)
			{
				UnityEngine.Object.Destroy(currDataEntryUIController.gameObject);
			}
			Type fieldPropType = fieldOrPropInfo.GetFieldPropType();
			DataEntryUIController dataEntryUIController = null;
			Debug.Log("DataType:" + fieldPropType);
			dataEntryUIController = ((fieldPropType != typeof(float) && fieldPropType != typeof(int) && fieldPropType != typeof(Vector3)) ? null : numericalDataEntryUIControllerPrefab);
			if (dataEntryUIController != null)
			{
				currDataEntryUIController = CreateDataEntryContent(dataEntryUIController);
				currDataEntryUIController.Init(fieldOrPropInfo);
				currDataEntryUIController.SetupHandUIPointer(uiPointerController);
			}
			else
			{
				currDataEntryUIController = null;
			}
		}

		private DataEntryUIController CreateDataEntryContent(DataEntryUIController dataEntryUIControllerPrefab)
		{
			DataEntryUIController dataEntryUIController = UnityEngine.Object.Instantiate(dataEntryUIControllerPrefab);
			dataEntryUIController.transform.SetParent(contentContainer, false);
			dataEntryUIController.transform.localPosition = Vector3.zero;
			dataEntryUIController.transform.localRotation = Quaternion.identity;
			dataEntryUIController.transform.localScale = Vector3.one;
			return dataEntryUIController;
		}
	}
}
