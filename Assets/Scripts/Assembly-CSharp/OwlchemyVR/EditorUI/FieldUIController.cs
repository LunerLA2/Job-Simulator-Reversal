using UnityEngine;

namespace OwlchemyVR.EditorUI
{
	public class FieldUIController : MonoBehaviour
	{
		protected FieldOrPropertyInfo fieldOrPropInfo;

		protected object valueObj;

		public virtual void Awake()
		{
		}

		public virtual void OnEnable()
		{
		}

		public virtual void OnDisable()
		{
		}

		public virtual void Init(FieldOrPropertyInfo fieldOrPropInfo)
		{
			this.fieldOrPropInfo = fieldOrPropInfo;
			RefreshValues();
		}

		public virtual void RefreshValues()
		{
			valueObj = fieldOrPropInfo.GetValue();
		}

		public virtual void Update()
		{
			if (fieldOrPropInfo != null && valueObj != fieldOrPropInfo.GetValue())
			{
				RefreshValues();
			}
		}
	}
}
