using System;
using UnityEngine;

namespace OwlchemyVR
{
	[Serializable]
	public class WeightedObjects
	{
		[SerializeField]
		protected UnityEngine.Object[] objects;

		[SerializeField]
		protected float[] weights;

		public UnityEngine.Object[] Objects
		{
			get
			{
				return objects;
			}
		}

		public UnityEngine.Object GetRandomObject()
		{
			float num = UnityEngine.Random.Range(0f, 1f);
			UnityEngine.Object result = null;
			int num2 = weights.Length - 1;
			while (num2 >= 0 && num <= weights[num2])
			{
				result = objects[num2];
				num2--;
			}
			return result;
		}

		internal void EqualizeWeights()
		{
			for (int i = 0; i < weights.Length; i++)
			{
				weights[i] = ((float)i + 1f) / (float)weights.Length;
			}
		}
	}
}
