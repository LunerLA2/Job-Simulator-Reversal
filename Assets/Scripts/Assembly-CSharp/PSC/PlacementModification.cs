using System;
using UnityEngine;

namespace PSC
{
	public class PlacementModification
	{
		[SerializeField]
		private string m_LastEditedBy;

		[SerializeField]
		private int m_LastEditAt;

		[SerializeField]
		private Transform m_Transform;

		[SerializeField]
		private Placement.MemberTypes m_Type;

		private int m_PreviouslyEditedAt;

		private string m_PreviouslyEditedBy;

		public int lastEditedAt
		{
			get
			{
				return m_LastEditAt;
			}
			private set
			{
				m_LastEditAt = value;
			}
		}

		public string lastEditedBy
		{
			get
			{
				return m_LastEditedBy;
			}
			private set
			{
				m_LastEditedBy = value;
			}
		}

		public Placement.MemberTypes type
		{
			get
			{
				return m_Type;
			}
			set
			{
				m_Type = value;
			}
		}

		public Transform target
		{
			get
			{
				return m_Transform;
			}
			set
			{
				m_Transform = value;
			}
		}

		public void ModificationMade(Placement placement, Placement.MemberTypes type)
		{
			m_Type = type;
			m_Transform = placement.transform;
			m_PreviouslyEditedAt = lastEditedAt;
			m_PreviouslyEditedBy = lastEditedBy;
			lastEditedBy = Environment.UserName;
			lastEditedAt = DateTime.Now.Second;
		}
	}
}
