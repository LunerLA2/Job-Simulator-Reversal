using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OwlchemyVR.EditorUI
{
	public class ComponentsSelectionUIPage : UIPage
	{
		public class ComponentSelectionEvent : UnityEvent<ComponentInfoObject>
		{
		}

		[SerializeField]
		private Text titleText;

		[SerializeField]
		private RectTransform selectionUIListContainer;

		[SerializeField]
		private UIComponentController uiSelectionControllerPrefab;

		private List<UIComponentController> uiComponentControllerList = new List<UIComponentController>();

		private UIComponentController currSelectedUIComponentController;

		private GameObjectInfoObj currGameObjectInfoObj;

		public ComponentSelectionEvent OnComponentSelectionChange = new ComponentSelectionEvent();

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
				base.gameObject.SetActive(false);
				currGameObjectInfoObj = null;
				if (currSelectedUIComponentController != null)
				{
					currSelectedUIComponentController.Deselect();
					currSelectedUIComponentController = null;
				}
			}
			base.IsPageOpen = false;
		}

		public void SelectedNewGameObject(GameObject selectedGO)
		{
			if (currGameObjectInfoObj == null || currGameObjectInfoObj.GameObject != selectedGO)
			{
				currGameObjectInfoObj = GameObjectComponentInfoBuilder.BuildGameObjectInfoObj(selectedGO);
				SetupPage();
			}
		}

		private void SetupPage()
		{
			foreach (UIComponentController uiComponentController in uiComponentControllerList)
			{
				uiComponentController.OnPress.RemoveAllListeners();
				Object.Destroy(uiComponentController.gameObject);
			}
			uiComponentControllerList.Clear();
			if (currGameObjectInfoObj != null)
			{
				titleText.text = currGameObjectInfoObj.GameObject.name;
				UIComponentController uIComponentController = null;
				uiComponentControllerList = new List<UIComponentController>();
				float num = 0f;
				{
					foreach (ComponentInfoObject componentInfoObj in currGameObjectInfoObj.ComponentInfoObjs)
					{
						uIComponentController = Object.Instantiate(uiSelectionControllerPrefab);
						uIComponentController.transform.SetParent(selectionUIListContainer);
						uIComponentController.transform.localRotation = Quaternion.identity;
						uIComponentController.transform.localScale = Vector3.one;
						uIComponentController.transform.localPosition = new Vector3(0f, num, 0f);
						num -= 45f;
						uIComponentController.Init(componentInfoObj);
						uIComponentController.OnPress.AddListener(ComponentSelectionChange);
						uiComponentControllerList.Add(uIComponentController);
					}
					return;
				}
			}
			titleText.text = "No GameObject Selected";
		}

		private void ComponentSelectionChange(UIItem uiItem, HandUIPointerController uiPointerController)
		{
			UIComponentController uIComponentController = uiItem as UIComponentController;
			if (uIComponentController != null)
			{
				if (currSelectedUIComponentController != null)
				{
					currSelectedUIComponentController.Deselect();
				}
				uIComponentController.Select();
				currSelectedUIComponentController = uIComponentController;
				OnComponentSelectionChange.Invoke(currSelectedUIComponentController.ComponentInfoObj);
			}
		}
	}
}
