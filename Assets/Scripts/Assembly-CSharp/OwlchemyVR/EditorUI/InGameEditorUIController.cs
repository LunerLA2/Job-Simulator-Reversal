using UnityEngine;

namespace OwlchemyVR.EditorUI
{
	public class InGameEditorUIController : MonoBehaviour
	{
		[SerializeField]
		private ComponentsSelectionUIPage componentsSelectionUIPage;

		[SerializeField]
		private ComponentFieldsUIPage componentsFieldsUIPage;

		[SerializeField]
		private IndividualFieldUIPage individualFieldUIPage;

		private void Awake()
		{
			componentsFieldsUIPage.gameObject.SetActive(false);
			individualFieldUIPage.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			componentsSelectionUIPage.OnComponentSelectionChange.AddListener(ComponentSelectionChange);
			componentsFieldsUIPage.OnIndividualFieldSelectionChange.AddListener(IndividualFieldSelectionChange);
		}

		private void OnDisable()
		{
			componentsSelectionUIPage.OnComponentSelectionChange.RemoveListener(ComponentSelectionChange);
			componentsFieldsUIPage.OnIndividualFieldSelectionChange.RemoveListener(IndividualFieldSelectionChange);
			componentsFieldsUIPage.Close();
			componentsSelectionUIPage.Close();
			individualFieldUIPage.Close();
		}

		public void NewNonUIPointerCollider(Collider c, HandUIPointerController handUIPointerController)
		{
			GameObject gameObject = null;
			if (c != null && c.enabled)
			{
				gameObject = ((!(c.attachedRigidbody != null)) ? c.gameObject : c.attachedRigidbody.gameObject);
			}
			componentsFieldsUIPage.Close();
			individualFieldUIPage.Close();
			if (gameObject != null)
			{
				componentsSelectionUIPage.Open();
				componentsSelectionUIPage.SelectedNewGameObject(gameObject);
			}
			else
			{
				componentsSelectionUIPage.Close();
			}
		}

		private void ComponentSelectionChange(ComponentInfoObject componentInfoObj)
		{
			componentsFieldsUIPage.Open();
			componentsFieldsUIPage.Init(componentInfoObj);
			individualFieldUIPage.Close();
		}

		private void IndividualFieldSelectionChange(FieldOrPropertyInfo fieldOrPropertyInfo, HandUIPointerController uiPointerController)
		{
			Debug.Log("IndvididualFieldSelectionChange:" + fieldOrPropertyInfo.Name);
			if (individualFieldUIPage.IsDataTypeSupported(fieldOrPropertyInfo.GetFieldPropType()))
			{
				individualFieldUIPage.Open();
				individualFieldUIPage.Init(fieldOrPropertyInfo, uiPointerController);
			}
			else
			{
				individualFieldUIPage.Close();
			}
		}

		public void ToggleMenuHandedness()
		{
			Vector3 localPosition = componentsFieldsUIPage.transform.localPosition;
			localPosition.x = 0f - localPosition.x;
			componentsFieldsUIPage.transform.localPosition = localPosition;
			Vector3 localEulerAngles = componentsFieldsUIPage.transform.localEulerAngles;
			localEulerAngles.y = 0f - localEulerAngles.y;
			componentsFieldsUIPage.transform.localEulerAngles = localEulerAngles;
			localEulerAngles = componentsSelectionUIPage.transform.localEulerAngles;
			localEulerAngles.y = 0f - localEulerAngles.y;
			componentsSelectionUIPage.transform.localEulerAngles = localEulerAngles;
			localPosition = individualFieldUIPage.transform.localPosition;
			localPosition.x = 0f - localPosition.x;
			individualFieldUIPage.transform.localPosition = localPosition;
			localEulerAngles = individualFieldUIPage.transform.localEulerAngles;
			localEulerAngles.y = 0f - localEulerAngles.y;
			individualFieldUIPage.transform.localEulerAngles = localEulerAngles;
		}
	}
}
