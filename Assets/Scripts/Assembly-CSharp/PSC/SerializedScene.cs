using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PSC
{
	[Serializable]
	public class SerializedScene
	{
		[SerializeField]
		private List<Placement> m_Placements;

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

		public string tempFolderPath
		{
			get
			{
				string dataPath = Application.dataPath;
				dataPath = dataPath.Replace("/Assets", "/Temp/PSC/");
				if (!Directory.Exists(dataPath))
				{
					Directory.CreateDirectory(dataPath);
				}
				return dataPath;
			}
		}

		public List<Placement> GetSceneChanges()
		{
			List<Placement> list = new List<Placement>();
			List<Transform> set = new List<Transform>();
			GameObject[] rootGameObjects = Room.activeRoom.scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				GetTransforms(rootGameObjects[i].transform, ref set);
			}
			foreach (Placement placement2 in m_Placements)
			{
				foreach (Transform item in set)
				{
					if (item == placement2.transform && !Helpers.ApproximatelyEqual(placement2, item))
					{
						Placement placement = new Placement(item);
						placement.Save();
						placement.CheckDifferences(placement2);
						list.Add(placement);
					}
				}
			}
			return list;
		}

		public void SaveScene()
		{
			m_Placements = new List<Placement>();
			Room activeRoom = Room.activeRoom;
			GameObject[] rootGameObjects = activeRoom.scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				EncodeTransform(rootGameObjects[i].transform, ref m_Placements);
			}
		}

		public void CheckDefaultLayoutDifferenceWithLayout(Layout layout)
		{
			for (int i = 0; i < m_Placements.Count; i++)
			{
				Placement placement = layout[m_Placements[i].transform];
				if (placement != null)
				{
					placement.CheckDifferences(m_Placements[i]);
				}
			}
		}

		public void Clear()
		{
			m_Placements = new List<Placement>();
		}

		public void RevertScene()
		{
			if (m_Placements == null)
			{
				return;
			}
			for (int num = m_Placements.Count - 1; num >= 0; num--)
			{
				if (placements[num].transform == null)
				{
					Debug.LogWarning("Placement for " + placements[num].name + " has been removed since the transform has been deleted");
					placements.RemoveAt(num);
				}
				else
				{
					placements[num].ApplyToRoom(Placement.MemberTypes.ActiveState, true);
				}
			}
		}

		private void GetTransforms(Transform transform, ref List<Transform> set)
		{
			set.Add(transform);
			for (int i = 0; i < transform.childCount; i++)
			{
				GetTransforms(transform.GetChild(i), ref set);
			}
		}

		private void EncodeTransform(Transform transform, ref List<Placement> set)
		{
			Placement placement = new Placement(transform);
			placement.Save();
			set.Add(placement);
			for (int i = 0; i < transform.childCount; i++)
			{
				EncodeTransform(transform.GetChild(i), ref set);
			}
		}
	}
}
