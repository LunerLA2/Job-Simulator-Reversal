using OwlchemyVR.EditorUI;
using UnityEngine;

namespace OwlchemyVR
{
	public class HandUIPointerController : MonoBehaviour
	{
		[SerializeField]
		private PointerController pointerControllerPrefab;

		[SerializeField]
		private Transform pointerAndUIParent;

		[SerializeField]
		private HandController handController;

		[SerializeField]
		private InteractionHandController interactionHandController;

		[SerializeField]
		private InGameEditorUIController inGameEditorUIControllerPrefab;

		[SerializeField]
		private Transform handModelContainer;

		private PointerController pointerController;

		private InGameEditorUIController editorUIController;

		private bool isPointerActive;

		private bool isEditorUIActive;

		private UIItem lastUIItem;

		private HandUIPointerController otherHand;

		public InteractionHandController InteractionHandController
		{
			get
			{
				return interactionHandController;
			}
		}

		private void PointerOnUpdate()
		{
			Collider c = pointerController.LineUpdate();
			otherHand.PointerCollider(c, this, handController.GetButton(HandController.HandControllerButton.Trigger), handController.GetButtonDown(HandController.HandControllerButton.Trigger));
		}

		private void EditorUIOnUpdate()
		{
		}

		private void BeginPointer()
		{
			isPointerActive = true;
			if (pointerController == null)
			{
				SetupPointer();
			}
			if (otherHand == null)
			{
				LinkOtherHand();
			}
			pointerController.gameObject.SetActive(true);
			interactionHandController.DeactivateHandInteractions();
			otherHand.BeginEditorUI();
		}

		private void EndPointer()
		{
			isPointerActive = false;
			pointerController.gameObject.SetActive(false);
			interactionHandController.ReactivateHandInteractions();
			otherHand.EndEditorUI();
		}

		public void BeginEditorUI()
		{
			isEditorUIActive = true;
			if (editorUIController == null)
			{
				SetupEditorUI();
			}
			editorUIController.gameObject.SetActive(true);
			interactionHandController.DeactivateHandInteractions();
		}

		public void EndEditorUI()
		{
			isEditorUIActive = false;
			editorUIController.gameObject.SetActive(false);
			interactionHandController.ReactivateHandInteractions();
		}

		private void SetupPointer()
		{
			pointerController = (PointerController)Object.Instantiate(pointerControllerPrefab, Vector3.zero, Quaternion.identity);
			pointerController.transform.SetParent(pointerAndUIParent, false);
		}

		private void SetupEditorUI()
		{
			editorUIController = Object.Instantiate(inGameEditorUIControllerPrefab);
			editorUIController.transform.SetParent(pointerAndUIParent, false);
			editorUIController.transform.localPosition = Vector3.zero;
			editorUIController.gameObject.SetActive(false);
			if (handModelContainer.transform.localScale.x < 0f)
			{
				editorUIController.transform.localEulerAngles = new Vector3(0f, 270f, 180f);
				editorUIController.ToggleMenuHandedness();
			}
			else
			{
				editorUIController.transform.localEulerAngles = new Vector3(0f, 270f, 0f);
			}
		}

		public void PointerCollider(Collider c, HandUIPointerController handUIPointerController, bool isTriggerButton, bool isTriggerButtonDown)
		{
			UIItem uIItem = null;
			bool flag = false;
			if (c == null)
			{
				uIItem = null;
			}
			else if (c.gameObject.layer == 5)
			{
				uIItem = c.GetComponent<UIItem>();
				if (uIItem == null)
				{
					uIItem = c.GetComponentInParent<UIItem>();
				}
			}
			else
			{
				uIItem = null;
				flag = true;
			}
			if (uIItem != lastUIItem && lastUIItem != null)
			{
				if (lastUIItem.IsCurrDown)
				{
					lastUIItem.ReleaseMessage(handUIPointerController);
				}
				lastUIItem.HoverOutMessage(handUIPointerController);
			}
			if (uIItem != null)
			{
				if (!uIItem.IsCurrHover)
				{
					uIItem.HoverOverMessage(handUIPointerController);
				}
				if (isTriggerButtonDown)
				{
					if (!uIItem.IsCurrDown)
					{
						uIItem.PressMessage(handUIPointerController);
					}
				}
				else if (!isTriggerButton && uIItem.IsCurrDown)
				{
					uIItem.ReleaseMessage(handUIPointerController);
				}
			}
			lastUIItem = uIItem;
			if (flag && isTriggerButtonDown)
			{
				editorUIController.NewNonUIPointerCollider(c, handUIPointerController);
			}
		}

		private void LinkOtherHand()
		{
			HandUIPointerController[] array = Object.FindObjectsOfType<HandUIPointerController>();
			if (array.Length == 2)
			{
				if (array[0] == this)
				{
					otherHand = array[1];
				}
				else
				{
					otherHand = array[0];
				}
			}
			else
			{
				Debug.LogError("Two hands required for editor ui editing");
			}
		}
	}
}
