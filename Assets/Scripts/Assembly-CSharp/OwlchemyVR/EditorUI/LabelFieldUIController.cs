using UnityEngine;
using UnityEngine.UI;

namespace OwlchemyVR.EditorUI
{
	public class LabelFieldUIController : FieldUIController
	{
		[SerializeField]
		private Text labelText;

		public override void RefreshValues()
		{
			base.RefreshValues();
			labelText.text = fieldOrPropInfo.GetValueAsString();
		}
	}
}
