using System;
using UnityEngine;
using UnityEngine.UI;

namespace OwlchemyVR.EditorUI
{
	public class NumericalDataEntryUIController : DataEntryUIController
	{
		[SerializeField]
		private Transform singleValueContainer;

		[SerializeField]
		private Text singleValueText;

		[SerializeField]
		private Vector3ValueFieldController vector3ValueController;

		[SerializeField]
		private SimpleButton[] allSimpleButtons;

		[SerializeField]
		private SimpleButton decimalBtn;

		[SerializeField]
		private SimpleButton copyXToYAndZBtn;

		private bool wasLastButtonPressedDecimal;

		public override void OnEnable()
		{
			base.OnEnable();
			SimpleButton[] array = allSimpleButtons;
			foreach (SimpleButton simpleButton in array)
			{
				simpleButton.OnPress.AddListener(ButtonPress);
			}
			Vector3ValueFieldController vector3ValueFieldController = vector3ValueController;
			vector3ValueFieldController.OnFieldChange = (Action<Vector3ValueFieldController>)Delegate.Combine(vector3ValueFieldController.OnFieldChange, new Action<Vector3ValueFieldController>(Vector3FieldChange));
		}

		public override void OnDisable()
		{
			base.OnDisable();
			SimpleButton[] array = allSimpleButtons;
			foreach (SimpleButton simpleButton in array)
			{
				simpleButton.OnPress.RemoveListener(ButtonPress);
			}
			Vector3ValueFieldController vector3ValueFieldController = vector3ValueController;
			vector3ValueFieldController.OnFieldChange = (Action<Vector3ValueFieldController>)Delegate.Remove(vector3ValueFieldController.OnFieldChange, new Action<Vector3ValueFieldController>(Vector3FieldChange));
		}

		private void Vector3FieldChange(Vector3ValueFieldController controller)
		{
			wasLastButtonPressedDecimal = false;
		}

		public override void Init(FieldOrPropertyInfo fieldOrPropInfo)
		{
			base.Init(fieldOrPropInfo);
			if (fieldOrPropInfo.GetFieldPropType() == typeof(int))
			{
				decimalBtn.gameObject.SetActive(false);
			}
			if (fieldOrPropInfo.GetFieldPropType() == typeof(Vector3))
			{
				singleValueContainer.gameObject.SetActive(false);
				vector3ValueController.gameObject.SetActive(true);
				vector3ValueController.Init(fieldOrPropInfo);
				copyXToYAndZBtn.gameObject.SetActive(true);
			}
			else
			{
				singleValueContainer.gameObject.SetActive(true);
				vector3ValueController.gameObject.SetActive(false);
				copyXToYAndZBtn.gameObject.SetActive(false);
			}
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (fieldOrPropInfo.GetFieldPropType() == typeof(Vector3))
			{
				vector3ValueController.RefreshValues();
			}
			else
			{
				singleValueText.text = fieldOrPropInfo.GetValueAsString();
			}
		}

		private void ButtonPress(UIItem uiItem, HandUIPointerController handUIPointerController)
		{
			SimpleButton simpleButton = uiItem as SimpleButton;
			if (simpleButton != null)
			{
				string iD = simpleButton.ID;
				ApplyActionByButtonID(iD.ToLower());
			}
		}

		private void ApplyActionByButtonID(string btnID)
		{
			string text = ((fieldOrPropInfo.GetFieldPropType() != typeof(Vector3)) ? fieldOrPropInfo.GetValueAsString() : vector3ValueController.GetCurrentSelectedFieldAsString());
			if (text == "0")
			{
				text = string.Empty;
			}
			switch (btnID)
			{
			case "decimal":
				if ((fieldOrPropInfo.GetFieldPropType() == typeof(float) || fieldOrPropInfo.GetFieldPropType() == typeof(Vector3)) && !text.Contains("."))
				{
					wasLastButtonPressedDecimal = true;
				}
				break;
			case "clear":
				text = "0";
				wasLastButtonPressedDecimal = false;
				break;
			case "back":
				if (wasLastButtonPressedDecimal)
				{
					wasLastButtonPressedDecimal = false;
				}
				else if (text.Length > 0)
				{
					text = text.Substring(0, text.Length - 1);
					if (text.Length > 0 && text[text.Length - 1] == '.')
					{
						wasLastButtonPressedDecimal = true;
						Debug.Log("Last button pressed was decimal");
					}
				}
				break;
			case "plus1":
			case "minus1":
				wasLastButtonPressedDecimal = false;
				if (fieldOrPropInfo.GetFieldPropType() == typeof(Vector3))
				{
					float num = vector3ValueController.GetCurrentlySelectedValue();
					if (btnID == "plus1")
					{
						num += 1f;
					}
					else if (btnID == "minus1")
					{
						num -= 1f;
					}
					text = num.ToString();
				}
				else if (fieldOrPropInfo.GetFieldPropType() == typeof(int))
				{
					int num2 = (int)fieldOrPropInfo.GetValue();
					if (btnID == "plus1")
					{
						num2++;
					}
					else if (btnID == "minus1")
					{
						num2--;
					}
					text = num2.ToString();
				}
				else
				{
					float num3 = (float)fieldOrPropInfo.GetValue();
					if (btnID == "plus1")
					{
						num3 += 1f;
					}
					else if (btnID == "minus1")
					{
						num3 -= 1f;
					}
					text = num3.ToString();
				}
				break;
			case "copyxtoyandz":
				if (fieldOrPropInfo.GetFieldPropType() == typeof(Vector3))
				{
					vector3ValueController.CopyXToYAndZ();
				}
				break;
			default:
			{
				int result = -1;
				if (int.TryParse(btnID, out result))
				{
					if (wasLastButtonPressedDecimal)
					{
						text += ".";
						wasLastButtonPressedDecimal = false;
					}
					text = text + string.Empty + result;
				}
				else
				{
					Debug.LogWarning("Could not find button id action:" + btnID);
				}
				break;
			}
			}
			if (text == string.Empty)
			{
				text = "0";
			}
			if (fieldOrPropInfo.GetFieldPropType() == typeof(Vector3))
			{
				vector3ValueController.SetCurrentSelectedFieldAsString(text);
			}
			else
			{
				fieldOrPropInfo.SetValueWithString(text);
			}
			RefreshValues();
		}

		public override void Update()
		{
			base.Update();
			Type fieldPropType = fieldOrPropInfo.GetFieldPropType();
			if (fieldPropType == typeof(float))
			{
				if (currSelectingHandUIPointerController.InteractionHandController.IsTrackPadButton())
				{
					Vector2 trackPadVector = currSelectingHandUIPointerController.InteractionHandController.GetTrackPadVector2();
					float num = (float)fieldOrPropInfo.GetValue();
					num += trackPadVector.x * Time.deltaTime;
					fieldOrPropInfo.SetValue(num);
					RefreshValues();
				}
			}
			else if (fieldPropType == typeof(int))
			{
				if (currSelectingHandUIPointerController.InteractionHandController.IsTrackPadButtonDown())
				{
					Vector2 trackPadVector2 = currSelectingHandUIPointerController.InteractionHandController.GetTrackPadVector2();
					int num2 = (int)fieldOrPropInfo.GetValue();
					if (Mathf.Abs(trackPadVector2.x) > 0f)
					{
						num2 = ((!(trackPadVector2.x < 0f)) ? (num2 + 1) : (num2 - 1));
					}
					fieldOrPropInfo.SetValue(num2);
					RefreshValues();
				}
			}
			else if (fieldPropType != typeof(Vector3))
			{
				Debug.LogWarning("Unsupported field prop type:" + fieldPropType);
			}
		}
	}
}
