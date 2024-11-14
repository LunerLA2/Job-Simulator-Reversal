using System.Collections.Generic;
using UnityEngine;

namespace SiliconTrail
{
	public static class WeightedProbableThingUtility
	{
		public static T SelectRandom<T>(this IList<T> options) where T : WeightedProbableThing
		{
			T result = (T)null;
			float num = 0f;
			for (int i = 0; i < options.Count; i++)
			{
				num += options[i].ProbabilityWeight;
			}
			float num2 = Random.Range(0f, num);
			foreach (T option in options)
			{
				if (num2 <= option.ProbabilityWeight)
				{
					result = option;
					break;
				}
				num2 -= option.ProbabilityWeight;
			}
			return result;
		}
	}
}
