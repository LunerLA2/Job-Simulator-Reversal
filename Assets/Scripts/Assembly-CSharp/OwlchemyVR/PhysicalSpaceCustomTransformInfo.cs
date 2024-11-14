using System;
using UnityEngine;

namespace OwlchemyVR
{
	[Serializable]
	public class PhysicalSpaceCustomTransformInfo
	{
		[SerializeField]
		public Transform transform;

		[SerializeField]
		public Vector3 localPosition;

		[SerializeField]
		public Quaternion localRotation;

		[SerializeField]
		public Vector3 localScale;

		[SerializeField]
		public bool activeSelf;

		public Transform Transform
		{
			get
			{
				return transform;
			}
		}

		public bool ActiveSelf
		{
			get
			{
				return activeSelf;
			}
		}

		public PhysicalSpaceCustomTransformInfo(Transform t)
		{
			SetTransform(t);
			UpdateInfo();
		}

		public void SetTransform(Transform t)
		{
			transform = t;
		}

		public void UpdateInfo()
		{
			localPosition = transform.localPosition;
			localRotation = transform.localRotation;
			localScale = transform.localScale;
			activeSelf = transform.gameObject.activeSelf;
		}

		public void Apply()
		{
			transform.localPosition = localPosition;
			transform.localRotation = localRotation;
			transform.localScale = localScale;
			transform.gameObject.SetActive(activeSelf);
		}

		public bool HasTransformChanged()
		{
			if (localPosition != transform.localPosition)
			{
				return true;
			}
			if (localRotation != transform.localRotation)
			{
				return true;
			}
			if (localScale != transform.localScale)
			{
				return true;
			}
			if (activeSelf != transform.gameObject.activeSelf)
			{
				return true;
			}
			return false;
		}

		public bool IsPropertiesDifferent(PhysicalSpaceCustomTransformInfo other)
		{
			if (localPosition != other.localPosition)
			{
				return true;
			}
			if (localRotation != other.localRotation)
			{
				return true;
			}
			if (localScale != other.localScale)
			{
				return true;
			}
			if (activeSelf != other.Transform.gameObject.activeSelf)
			{
				return true;
			}
			return false;
		}
	}
}
