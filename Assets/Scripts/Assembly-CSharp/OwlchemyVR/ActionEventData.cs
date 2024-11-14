using System;
using UnityEngine;

namespace OwlchemyVR
{
	[Serializable]
	public class ActionEventData : ScriptableObject
	{
		public enum FormatTypes
		{
			NoWorldItemData = 0,
			SingleWorldItemData = 1,
			WorldItemDataAppliedToWorldItemData = 2
		}

		[SerializeField]
		private FormatTypes formatType;

		[SerializeField]
		private bool containsAmount;

		private string actionEventName;

		public FormatTypes FormatType
		{
			get
			{
				return formatType;
			}
		}

		public bool ContainsAmount
		{
			get
			{
				return containsAmount;
			}
		}

		public string ActionEventName
		{
			get
			{
				return actionEventName;
			}
		}

		private void OnEnable()
		{
			actionEventName = base.name;
		}
	}
}
