using System.Collections.Generic;
using UnityEngine;

namespace OwlchemyVR
{
	public class PoseContainer : MonoBehaviour
	{
		[SerializeField]
		private bool bonesCanBeClearedAtRuntime;

		public Dictionary<int, Quaternion> storedBoneRotations = new Dictionary<int, Quaternion>();

		private static int nextChildId = 1;

		public static Dictionary<int, string> childIds = new Dictionary<int, string>();

		private void Awake()
		{
			Transform[] componentsInChildren = base.transform.GetComponentsInChildren<Transform>();
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				string fullPathToMyChild = GetFullPathToMyChild(transform);
				int num = -1;
				foreach (int key in childIds.Keys)
				{
					if (childIds[key] == fullPathToMyChild)
					{
						num = key;
					}
				}
				if (num == -1)
				{
					num = nextChildId;
					childIds[nextChildId] = fullPathToMyChild;
					nextChildId++;
				}
				storedBoneRotations[num] = transform.localRotation;
			}
			if (!bonesCanBeClearedAtRuntime)
			{
				return;
			}
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (componentsInChildren[j].gameObject != null && componentsInChildren[j] != base.transform)
				{
					Object.Destroy(componentsInChildren[j].gameObject);
				}
			}
		}

		private string GetFullPathToMyChild(Transform t)
		{
			string text = t.gameObject.name;
			Transform transform = t;
			while (transform != base.transform)
			{
				transform = transform.parent;
				text = transform.gameObject.name + "/" + text;
			}
			return text;
		}

		public Quaternion GetRotationOfTransform(int childId)
		{
			if (storedBoneRotations.ContainsKey(childId))
			{
				return storedBoneRotations[childId];
			}
			return Quaternion.identity;
		}

		public static string GetNameForId(int childId)
		{
			if (childIds.ContainsKey(childId))
			{
				return childIds[childId];
			}
			return null;
		}
	}
}
