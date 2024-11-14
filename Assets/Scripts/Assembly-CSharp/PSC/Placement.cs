using System;
using UnityEngine;

namespace PSC
{
	[Serializable]
	public class Placement
	{
		public enum MemberTypes
		{
			Position = 0,
			Rotation = 1,
			Scale = 2,
			ActiveState = 3
		}

		[SerializeField]
		private string m_Name;

		[NonSerialized]
		private bool m_SceneActiveState;

		[SerializeField]
		[Header("Changes")]
		private bool m_ActiveSelf;

		[SerializeField]
		private Transform m_Transform;

		[SerializeField]
		private Vector3 m_LocalPosition;

		[SerializeField]
		private Quaternion m_LocalRotation;

		[SerializeField]
		private Vector3 m_LocalScale;

		[Header("Changes Flags")]
		[SerializeField]
		private bool m_HasPositionChanges;

		[SerializeField]
		private bool m_HasRotationChanges;

		[SerializeField]
		private bool m_HasScaleChanges;

		[SerializeField]
		private bool m_HasActiveSelfChanges;

		private bool m_StackInitialized;

		private Vector3 m_ModStackPostion;

		private Vector3 m_ModStackScale;

		private bool m_ModStackActiveSelf;

		private Quaternion m_ModStackRotation;

		public bool activeSelf
		{
			get
			{
				return m_ActiveSelf;
			}
			set
			{
				m_ActiveSelf = value;
			}
		}

		public Quaternion localRotation
		{
			get
			{
				return m_LocalRotation;
			}
			set
			{
				m_LocalRotation = value;
			}
		}

		public Vector3 localScale
		{
			get
			{
				return m_LocalScale;
			}
			set
			{
				m_LocalScale = value;
			}
		}

		public Vector3 localPosition
		{
			get
			{
				return m_LocalPosition;
			}
			set
			{
				m_LocalPosition = value;
			}
		}

		public Transform transform
		{
			get
			{
				return m_Transform;
			}
			set
			{
				if (value != null)
				{
					m_Name = value.gameObject.name;
				}
				else
				{
					m_Name = string.Empty;
				}
				m_Transform = value;
			}
		}

		public string name
		{
			get
			{
				return m_Name;
			}
		}

		public bool hasPositionChange
		{
			get
			{
				return m_HasPositionChanges;
			}
		}

		public bool hasRotationChange
		{
			get
			{
				return m_HasRotationChanges;
			}
		}

		public bool hasScaleChange
		{
			get
			{
				return m_HasScaleChanges;
			}
		}

		public bool hasActiveSelfChange
		{
			get
			{
				return m_HasActiveSelfChanges;
			}
		}

		public bool hasLocalChanges
		{
			get
			{
				return !Helpers.ApproximatelyEqual(this, transform);
			}
		}

		public bool hasCustomValuesForLayout
		{
			get
			{
				return hasActiveSelfChange || hasPositionChange || hasScaleChange || hasRotationChange;
			}
		}

		public Placement(Transform transform)
		{
			this.transform = transform;
			Save();
		}

		public void ClearMemeberChangeFlags(MemberTypes memberFlagsToClear)
		{
			if ((memberFlagsToClear & MemberTypes.ActiveState) == MemberTypes.ActiveState)
			{
				m_HasActiveSelfChanges = false;
			}
			m_HasPositionChanges = false;
			if ((memberFlagsToClear & MemberTypes.Rotation) == MemberTypes.Rotation)
			{
				m_HasRotationChanges = false;
			}
			if ((memberFlagsToClear & MemberTypes.Scale) == MemberTypes.Scale)
			{
				m_HasScaleChanges = false;
			}
		}

		public void CheckDifferences(Placement defaultPlacement)
		{
			bool flag = false;
			if (defaultPlacement.localPosition != localPosition)
			{
				if (!hasPositionChange)
				{
					m_HasPositionChanges = true;
					flag = true;
				}
			}
			else if (hasPositionChange)
			{
				m_HasPositionChanges = false;
				flag = true;
			}
			if (defaultPlacement.localRotation != localRotation)
			{
				if (!hasRotationChange)
				{
					m_HasRotationChanges = true;
					flag = true;
				}
			}
			else if (hasRotationChange)
			{
				m_HasRotationChanges = false;
				flag = true;
			}
			if (defaultPlacement.localScale != localScale)
			{
				if (!hasScaleChange)
				{
					m_HasScaleChanges = true;
					flag = true;
				}
			}
			else if (hasScaleChange)
			{
				m_HasScaleChanges = false;
				flag = true;
			}
			if (defaultPlacement.activeSelf != activeSelf)
			{
				if (!hasActiveSelfChange)
				{
					m_HasActiveSelfChanges = true;
					flag = true;
				}
			}
			else if (hasActiveSelfChange)
			{
				m_HasActiveSelfChanges = false;
				flag = true;
			}
			if (!flag)
			{
			}
		}

		public void LoadPhaseOne()
		{
			if (transform != null)
			{
				if (hasActiveSelfChange)
				{
					m_SceneActiveState = transform.gameObject.activeSelf;
				}
				if (hasPositionChange)
				{
					transform.localPosition = m_LocalPosition;
				}
				if (hasRotationChange)
				{
					transform.localRotation = m_LocalRotation;
				}
				if (hasScaleChange)
				{
					transform.localScale = m_LocalScale;
				}
			}
		}

		public void LoadPhaseTwo()
		{
			if (transform != null)
			{
				bool active = m_SceneActiveState ^ transform.gameObject.activeSelf ^ m_ActiveSelf;
				if (hasActiveSelfChange)
				{
					transform.gameObject.SetActive(active);
				}
			}
		}

		public void ApplyToRoom(MemberTypes membersToApply, bool isLoadingDefault = false)
		{
			if (transform != null)
			{
				if (!isLoadingDefault && transform.gameObject.isStatic && Application.isPlaying)
				{
					Debug.LogError("You are attempting to runtime change the position or activation state of a static object:" + transform.name, transform.gameObject);
				}
				if (hasPositionChange || isLoadingDefault)
				{
					transform.localPosition = m_LocalPosition;
				}
				if ((membersToApply & MemberTypes.Rotation) == MemberTypes.Rotation && (hasRotationChange || isLoadingDefault))
				{
					transform.localRotation = m_LocalRotation;
				}
				if ((membersToApply & MemberTypes.Scale) == MemberTypes.Scale && (hasScaleChange || isLoadingDefault))
				{
					transform.localScale = m_LocalScale;
				}
				if ((membersToApply & MemberTypes.ActiveState) == MemberTypes.ActiveState && (hasActiveSelfChange || isLoadingDefault) && transform.gameObject.activeInHierarchy != activeSelf)
				{
					transform.gameObject.SetActive(activeSelf);
				}
			}
		}

		public void Save()
		{
			if (transform != null)
			{
				m_LocalPosition = transform.localPosition;
				m_LocalRotation = transform.localRotation;
				m_LocalScale = transform.localScale;
				m_ActiveSelf = transform.gameObject.activeSelf;
			}
		}

		public void UpdateModificationStack(ref ModifictionStack stack)
		{
			if (!m_StackInitialized)
			{
				m_StackInitialized = true;
				m_ModStackPostion = transform.localPosition;
				m_ModStackScale = transform.localScale;
				m_ModStackRotation = transform.localRotation;
				m_ModStackActiveSelf = transform.gameObject.activeSelf;
				return;
			}
			if (m_ModStackPostion != transform.localPosition)
			{
				m_ModStackPostion = transform.localPosition;
				stack.ModifcationMade(this, MemberTypes.Position);
			}
			if (m_ModStackScale != transform.localScale)
			{
				m_ModStackScale = transform.localScale;
				stack.ModifcationMade(this, MemberTypes.Scale);
			}
			if (m_ModStackRotation != transform.localRotation)
			{
				m_ModStackRotation = transform.localRotation;
				stack.ModifcationMade(this, MemberTypes.Rotation);
			}
			if (m_ModStackActiveSelf != transform.gameObject.activeSelf)
			{
				m_ModStackActiveSelf = transform.gameObject.activeSelf;
				stack.ModifcationMade(this, MemberTypes.ActiveState);
			}
		}
	}
}
