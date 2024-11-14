using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace OwlchemyVR.EditorUI
{
	public class Vector3ValueFieldController : MonoBehaviour
	{
		private enum Vector3ValueType
		{
			X = 0,
			Y = 1,
			Z = 2
		}

		[SerializeField]
		private Text xValueText;

		[SerializeField]
		private Text yValueText;

		[SerializeField]
		private Text zValueText;

		[SerializeField]
		private SimpleButton xBtn;

		[SerializeField]
		private SimpleButton yBtn;

		[SerializeField]
		private SimpleButton zBtn;

		private SimpleButton currBtnSelected;

		private Vector3ValueType currVector3ValueType;

		private FieldOrPropertyInfo fieldOrPropInfo;

		public Action<Vector3ValueFieldController> OnFieldChange;

		private Vector3ValueType CurrVector3ValueType
		{
			get
			{
				return currVector3ValueType;
			}
		}

		public void Init(FieldOrPropertyInfo fieldOrPropInfo)
		{
			this.fieldOrPropInfo = fieldOrPropInfo;
			currBtnSelected = xBtn;
			currVector3ValueType = Vector3ValueType.X;
			currBtnSelected.Select();
			RefreshValues();
		}

		private void OnEnable()
		{
			xBtn.OnPress.AddListener(NewFieldSelected);
			yBtn.OnPress.AddListener(NewFieldSelected);
			zBtn.OnPress.AddListener(NewFieldSelected);
		}

		private void OnDisable()
		{
			xBtn.OnPress.RemoveListener(NewFieldSelected);
			yBtn.OnPress.RemoveListener(NewFieldSelected);
			zBtn.OnPress.RemoveListener(NewFieldSelected);
		}

		private void NewFieldSelected(UIItem uiItem, HandUIPointerController handUIPointerController)
		{
			SimpleButton simpleButton = (SimpleButton)uiItem;
			if (simpleButton != currBtnSelected)
			{
				if (OnFieldChange != null)
				{
					OnFieldChange(this);
				}
				if (currBtnSelected != null)
				{
					currBtnSelected.Deselect();
				}
				currBtnSelected = simpleButton;
				currBtnSelected.Select();
				if (currBtnSelected == xBtn)
				{
					currVector3ValueType = Vector3ValueType.X;
				}
				else if (currBtnSelected == yBtn)
				{
					currVector3ValueType = Vector3ValueType.Y;
				}
				else if (currBtnSelected == zBtn)
				{
					currVector3ValueType = Vector3ValueType.Z;
				}
			}
		}

		public string GetCurrentSelectedFieldAsString()
		{
			Vector3 vector = (Vector3)fieldOrPropInfo.GetValue();
			if (currVector3ValueType == Vector3ValueType.X)
			{
				return vector.x.ToString();
			}
			if (currVector3ValueType == Vector3ValueType.Y)
			{
				return vector.y.ToString();
			}
			if (currVector3ValueType == Vector3ValueType.Z)
			{
				return vector.z.ToString();
			}
			return string.Empty;
		}

		public void SetCurrentSelectedFieldAsString(string value)
		{
			Vector3 vector = (Vector3)fieldOrPropInfo.GetValue();
			float result;
			if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				if (currVector3ValueType == Vector3ValueType.X)
				{
					vector.x = result;
				}
				else if (currVector3ValueType == Vector3ValueType.Y)
				{
					vector.y = result;
				}
				else if (currVector3ValueType == Vector3ValueType.Z)
				{
					vector.z = result;
				}
			}
			fieldOrPropInfo.SetValue(vector);
			vector = (Vector3)fieldOrPropInfo.GetValue();
			RefreshValues();
		}

		public float GetCurrentlySelectedValue()
		{
			Vector3 vector = (Vector3)fieldOrPropInfo.GetValue();
			if (currVector3ValueType == Vector3ValueType.X)
			{
				return vector.x;
			}
			if (currVector3ValueType == Vector3ValueType.Y)
			{
				return vector.y;
			}
			if (currVector3ValueType == Vector3ValueType.Z)
			{
				return vector.z;
			}
			return 0f;
		}

		public void CopyXToYAndZ()
		{
			Vector3 vector = (Vector3)fieldOrPropInfo.GetValue();
			vector.y = vector.x;
			vector.z = vector.x;
			fieldOrPropInfo.SetValue(vector);
			RefreshValues();
		}

		public void RefreshValues()
		{
			if (fieldOrPropInfo != null)
			{
				Vector3 vector = (Vector3)fieldOrPropInfo.GetValue();
				xValueText.text = vector.x.ToString();
				yValueText.text = vector.y.ToString();
				zValueText.text = vector.z.ToString();
			}
		}
	}
}
