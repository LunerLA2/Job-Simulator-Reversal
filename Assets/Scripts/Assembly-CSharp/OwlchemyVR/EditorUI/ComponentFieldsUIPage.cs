using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OwlchemyVR.EditorUI
{
	public class ComponentFieldsUIPage : UIPage
	{
		public class ComponentFieldSelectionEvent : UnityEvent<FieldOrPropertyInfo, HandUIPointerController>
		{
		}

		[SerializeField]
		private RectTransform fieldUIListContainer;

		[SerializeField]
		private UIComponentFieldController uiFieldControllerPrefab;

		[SerializeField]
		private Text titleText;

		private List<UIComponentFieldController> uiFieldControllersList = new List<UIComponentFieldController>();

		private UIComponentFieldController currActiveUIInteractionItem;

		public ComponentFieldSelectionEvent OnIndividualFieldSelectionChange = new ComponentFieldSelectionEvent();

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
				if (currActiveUIInteractionItem != null)
				{
					currActiveUIInteractionItem.Deselect();
					currActiveUIInteractionItem = null;
				}
				base.gameObject.SetActive(false);
			}
			base.IsPageOpen = false;
		}

		public void Init(ComponentInfoObject componentInfoObj)
		{
			foreach (UIComponentFieldController uiFieldControllers in uiFieldControllersList)
			{
				uiFieldControllers.OnPress.RemoveAllListeners();
				Object.Destroy(uiFieldControllers.gameObject);
			}
			uiFieldControllersList.Clear();
			titleText.text = componentInfoObj.Name;
			float num = 0f;
			foreach (FieldOrPropertyInfo fieldsAndProperty in componentInfoObj.FieldsAndProperties)
			{
				UIComponentFieldController uIComponentFieldController = Object.Instantiate(uiFieldControllerPrefab);
				uIComponentFieldController.transform.SetParent(fieldUIListContainer);
				uIComponentFieldController.transform.localRotation = Quaternion.identity;
				uIComponentFieldController.transform.localScale = Vector3.one;
				uIComponentFieldController.transform.localPosition = new Vector3(0f, num, 0f);
				num -= 48f;
				uIComponentFieldController.Init(fieldsAndProperty);
				uIComponentFieldController.OnPress.AddListener(UIFieldPressed);
				uiFieldControllersList.Add(uIComponentFieldController);
			}
		}

		private void UIFieldPressed(UIItem uiItem, HandUIPointerController uiPointerController)
		{
			Debug.Log("UIFieldPressed:" + uiItem.name);
			UIComponentFieldController uIComponentFieldController = uiItem as UIComponentFieldController;
			if (uIComponentFieldController != null)
			{
				if (currActiveUIInteractionItem != null)
				{
					currActiveUIInteractionItem.Deselect();
				}
				currActiveUIInteractionItem = uIComponentFieldController;
				currActiveUIInteractionItem.Select();
				OnIndividualFieldSelectionChange.Invoke(currActiveUIInteractionItem.FieldOrPropInfo, uiPointerController);
			}
		}
	}
}
