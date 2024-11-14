using System;
using UnityEngine;

namespace OwlchemyVR
{
	public class ScratcherController : MonoBehaviour
	{
		[SerializeField]
		private WorldItemData[] validItemData;

		private Material material;

		private float[] panels = new float[10];

		private float cardWidth;

		private int panelsScratched;

		public Action OnScratchOffCompleted;

		private void Start()
		{
			for (int i = 0; i < panels.Length; i++)
			{
				panels[i] = 1f;
			}
			material = GetComponent<Renderer>().material;
			material.SetFloatArray("_Panels", panels);
			BoxCollider component = GetComponent<BoxCollider>();
			cardWidth = component.bounds.extents.x;
		}

		private void OnCollisionStay(Collision c)
		{
			if (c.rigidbody == null)
			{
				return;
			}
			WorldItem component = c.rigidbody.GetComponent<WorldItem>();
			if (component == null)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < validItemData.Length; i++)
			{
				if (component.Data == validItemData[i])
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				Vector3 vector = base.transform.InverseTransformPoint(c.contacts[0].point);
				float num = Mathf.InverseLerp(cardWidth, 0f - cardWidth, vector.x);
				int num2 = (int)Mathf.Clamp(num * (float)panels.Length, 0f, panels.Length - 1);
				if (panels[num2] > 0f)
				{
					panelsScratched++;
				}
				panels[num2] = 0f;
				material.SetFloatArray("_Panels", panels);
				if (panelsScratched >= panels.Length && OnScratchOffCompleted != null)
				{
					OnScratchOffCompleted();
				}
			}
		}
	}
}
