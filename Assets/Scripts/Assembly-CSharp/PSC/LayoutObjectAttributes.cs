using System;
using UnityEngine;

namespace PSC
{
	[Serializable]
	public class LayoutObjectAttributes
	{
		[SerializeField]
		private bool makeStatic;

		[SerializeField]
		private bool removeFromLayout;

		[SerializeField]
		private Transform m_Transform;

		[SerializeField]
		private string m_Name;

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

		public bool MakeStatic
		{
			get
			{
				return makeStatic;
			}
			set
			{
				makeStatic = value;
			}
		}

		public bool RemoveFromLayout
		{
			get
			{
				return removeFromLayout;
			}
			set
			{
				removeFromLayout = value;
			}
		}

		public bool IsSuperfluous
		{
			get
			{
				return !makeStatic && !removeFromLayout;
			}
		}

		public LayoutObjectAttributes(LayoutObjectAttributes source)
		{
			m_Transform = source.transform;
			m_Name = source.m_Name;
			makeStatic = source.MakeStatic;
			removeFromLayout = source.RemoveFromLayout;
		}

		public LayoutObjectAttributes(Transform transform)
		{
			this.transform = transform;
		}

		public bool DiffersFrom(LayoutObjectAttributes attrib)
		{
			return attrib.makeStatic != makeStatic || attrib.RemoveFromLayout != removeFromLayout;
		}

		public void LoadPhaseOne()
		{
			if (transform != null && removeFromLayout && Application.isEditor)
			{
				transform.gameObject.SetActive(false);
			}
		}

		public void LoadPhaseTwo()
		{
			if (!(transform != null))
			{
			}
		}

		public void ApplyToRoom()
		{
			if (!(transform != null))
			{
			}
		}

		public void ApplyToSceneForBake()
		{
			if (transform != null)
			{
				if (removeFromLayout)
				{
					UnityEngine.Object.DestroyImmediate(transform.gameObject);
				}
				else if (!makeStatic)
				{
				}
			}
		}
	}
}
