using System;
using UnityEngine;

namespace PSC
{
	[Serializable]
	[CreateAssetMenu]
	public class LayoutConfiguration : ScriptableObject
	{
		[SerializeField]
		private Vector2 m_SizeInMeters;

		public Vector2 sizeInMeters
		{
			get
			{
				return m_SizeInMeters;
			}
			set
			{
				m_SizeInMeters = value;
			}
		}

		public void OnEnable()
		{
			base.hideFlags = HideFlags.None;
		}
	}
}
