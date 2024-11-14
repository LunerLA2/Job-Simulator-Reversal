using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PSC
{
	[Serializable]
	[DebuggerDisplay("Name: {config.name}")]
	public class Layout
	{
		[SerializeField]
		private LayoutConfiguration m_Configuartion;

		[SerializeField]
		private List<Placement> m_Placements = new List<Placement>();

		[SerializeField]
		private List<LayoutObjectAttributes> m_Attributes = new List<LayoutObjectAttributes>();

		[SerializeField]
		private List<LayoutObjectAttributes> m_PendingAttributes = new List<LayoutObjectAttributes>();

		public LayoutConfiguration config
		{
			get
			{
				return m_Configuartion;
			}
			set
			{
				m_Configuartion = value;
			}
		}

		public List<Placement> placements
		{
			get
			{
				return m_Placements;
			}
			set
			{
				m_Placements = value;
			}
		}

		public Placement this[int index]
		{
			get
			{
				return m_Placements[index];
			}
			set
			{
				m_Placements[index] = value;
			}
		}

		public Placement this[Transform trans]
		{
			get
			{
				for (int i = 0; i < m_Placements.Count; i++)
				{
					if (m_Placements[i].transform == trans)
					{
						return m_Placements[i];
					}
				}
				return null;
			}
		}

		public int PlacementCount
		{
			get
			{
				return m_Placements.Count;
			}
		}

		public void RemovePlacementAt(int index)
		{
			m_Placements.RemoveAt(index);
		}

		public bool TryToGetPlacement(Transform trans, ref Placement placement)
		{
			for (int i = 0; i < m_Placements.Count; i++)
			{
				if (m_Placements[i].transform == trans)
				{
					placement = m_Placements[i];
					return true;
				}
			}
			placement = null;
			return false;
		}

		public void VerifyPlacementsAndAttributes()
		{
			for (int num = m_Placements.Count - 1; num >= 0; num--)
			{
				if (m_Placements[num].transform == null)
				{
					UnityEngine.Debug.LogWarning("The placement '" + m_Placements[num].name + "' was destroyed and will be removed from " + m_Configuartion.name + " layout");
				}
				m_Placements.RemoveAt(num);
			}
			for (int num2 = m_Attributes.Count - 1; num2 >= 0; num2--)
			{
				if (m_Attributes[num2].transform == null)
				{
					UnityEngine.Debug.LogWarning("The attributes object '" + m_Attributes[num2].name + "' was destroyed and will be removed from " + m_Configuartion.name + " layout");
				}
				m_Attributes.RemoveAt(num2);
			}
		}

		public LayoutObjectAttributes GetExistingAttributes(Transform trans)
		{
			foreach (LayoutObjectAttributes attribute in m_Attributes)
			{
				if (attribute.transform.Equals(trans))
				{
					return attribute;
				}
			}
			return null;
		}

		public LayoutObjectAttributes GetRecordableAttributes(Transform trans)
		{
			foreach (LayoutObjectAttributes pendingAttribute in m_PendingAttributes)
			{
				if (pendingAttribute.transform.Equals(trans))
				{
					return pendingAttribute;
				}
			}
			foreach (LayoutObjectAttributes attribute in m_Attributes)
			{
				if (attribute.transform.Equals(trans))
				{
					LayoutObjectAttributes layoutObjectAttributes = new LayoutObjectAttributes(attribute);
					m_PendingAttributes.Add(layoutObjectAttributes);
					return layoutObjectAttributes;
				}
			}
			LayoutObjectAttributes layoutObjectAttributes2 = new LayoutObjectAttributes(trans);
			m_PendingAttributes.Add(layoutObjectAttributes2);
			return layoutObjectAttributes2;
		}

		private void CullAttributes()
		{
			for (int num = m_Attributes.Count - 1; num >= 0; num--)
			{
				if (m_Attributes[num].IsSuperfluous)
				{
					m_Attributes.RemoveAt(num);
				}
			}
		}

		public bool HasPendingAttributeChanges()
		{
			foreach (LayoutObjectAttributes pendingAttribute in m_PendingAttributes)
			{
				bool flag = false;
				for (int num = m_Attributes.Count - 1; num >= 0; num--)
				{
					if (pendingAttribute.transform.Equals(m_Attributes[num].transform))
					{
						flag = true;
						if (!pendingAttribute.DiffersFrom(m_Attributes[num]))
						{
							break;
						}
						return true;
					}
				}
				if (!flag && !pendingAttribute.IsSuperfluous)
				{
					return true;
				}
			}
			return false;
		}

		public void SavePendingAttributes()
		{
			foreach (LayoutObjectAttributes pendingAttribute in m_PendingAttributes)
			{
				for (int num = m_Attributes.Count - 1; num >= 0; num--)
				{
					if (pendingAttribute.transform.Equals(m_Attributes[num].transform))
					{
						m_Attributes.RemoveAt(num);
						break;
					}
				}
				m_Attributes.Add(pendingAttribute);
			}
			m_PendingAttributes.Clear();
			CullAttributes();
		}

		public void ClearPendingAttributes()
		{
			m_PendingAttributes.Clear();
		}

		public void LoadPhaseOne()
		{
			for (int i = 0; i < m_Placements.Count; i++)
			{
				m_Placements[i].LoadPhaseOne();
			}
			for (int j = 0; j < m_Attributes.Count; j++)
			{
				m_Attributes[j].LoadPhaseOne();
			}
		}

		public void LoadPhaseTwo()
		{
			for (int i = 0; i < m_Placements.Count; i++)
			{
				m_Placements[i].LoadPhaseTwo();
			}
			for (int j = 0; j < m_Attributes.Count; j++)
			{
				m_Attributes[j].LoadPhaseTwo();
			}
		}

		public void Apply(Placement.MemberTypes membersToApply)
		{
			if (m_Placements != null)
			{
				for (int i = 0; i < m_Placements.Count; i++)
				{
					m_Placements[i].ApplyToRoom(membersToApply);
				}
			}
			if (m_Attributes != null && Application.isPlaying && Application.isEditor)
			{
				for (int j = 0; j < m_Attributes.Count; j++)
				{
					m_Attributes[j].ApplyToRoom();
				}
			}
		}

		public void ApplyToSceneForBake(Placement.MemberTypes membersToApply)
		{
			if (m_Placements != null)
			{
				for (int i = 0; i < m_Placements.Count; i++)
				{
					m_Placements[i].ApplyToRoom(membersToApply);
				}
			}
			if (m_Attributes != null && !Application.isPlaying && Application.isEditor)
			{
				for (int j = 0; j < m_Attributes.Count; j++)
				{
					m_Attributes[j].ApplyToSceneForBake();
				}
			}
		}
	}
}
