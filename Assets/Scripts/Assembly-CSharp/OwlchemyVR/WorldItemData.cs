using System;
using UnityEngine;

namespace OwlchemyVR
{
	[Serializable]
	public class WorldItemData : ScriptableObject
	{
		public const float NEUTRAL_FLUID_VISCOSITY = 0.1f;

		[SerializeField]
		private SurfaceTypeData surfaceTypeData;

		[SerializeField]
		private string itemFullName;

		[SerializeField]
		private ActionEventData[] validActionEvents;

		[SerializeField]
		private float cost;

		[SerializeField]
		private Color overallColor = Color.white;

		[Range(0.01f, 1f)]
		[SerializeField]
		private float viscosityAsFluid = 0.1f;

		public SurfaceTypeData SurfaceTypeData
		{
			get
			{
				return surfaceTypeData;
			}
		}

		public string ItemName
		{
			get
			{
				return base.name;
			}
		}

		public string ItemFullName
		{
			get
			{
				if (string.IsNullOrEmpty(itemFullName))
				{
					return ItemName;
				}
				return itemFullName;
			}
		}

		public ActionEventData[] ValidActionEvents
		{
			get
			{
				return validActionEvents;
			}
		}

		public float Cost
		{
			get
			{
				return cost;
			}
		}

		public Color OverallColor
		{
			get
			{
				return overallColor;
			}
		}

		public float ViscosityAsFluid
		{
			get
			{
				return viscosityAsFluid;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return ((WorldItemData)obj).ItemName.Equals(ItemName);
		}

		public override int GetHashCode()
		{
			return ItemName.GetHashCode();
		}

		public bool DoesContainActionEvent(ActionEventData data)
		{
			if (validActionEvents != null)
			{
				for (int i = 0; i < validActionEvents.Length; i++)
				{
					if (validActionEvents[i] == data)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
