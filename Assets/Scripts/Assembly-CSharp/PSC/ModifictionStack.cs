using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PSC
{
	[Serializable]
	public class ModifictionStack
	{
		private const int MAX_HISTORY_COUNT = 50;

		[SerializeField]
		private List<PlacementModification> m_Modifications = new List<PlacementModification>();

		public void ModifcationMade(Placement target, Placement.MemberTypes type)
		{
			Debug.Log("Mod Made: " + target.transform.name + " type: " + type);
			for (int i = 0; i < m_Modifications.Count; i++)
			{
				if (m_Modifications[i].target == target.transform && m_Modifications[i].type == type)
				{
					m_Modifications[i].ModificationMade(target, type);
					return;
				}
			}
			if (m_Modifications.Count >= 50)
			{
				m_Modifications.RemoveAt(0);
				PlacementModification placementModification = new PlacementModification();
				placementModification.ModificationMade(target, type);
				m_Modifications.Add(placementModification);
			}
		}

		public static ModifictionStack LoadModifictionStack(Scene scene)
		{
			string userData = SceneMetaUtils.GetUserData(scene);
			if (!string.IsNullOrEmpty(userData))
			{
				return JsonUtility.FromJson<ModifictionStack>(userData);
			}
			return new ModifictionStack();
		}

		public static void SaveModificationStack(Scene scene, ModifictionStack stack)
		{
			if (stack != null)
			{
				string json = JsonUtility.ToJson(stack);
				SceneMetaUtils.SetUserData(scene, json);
			}
		}
	}
}
