using UnityEngine;

namespace NobleMuffins.MuffinSlicer
{
	public class VectorAccumulator
	{
		private Vector3 aggregatedFigures = Vector3.zero;

		private int count;

		public Vector3 mean
		{
			get
			{
				if (count == 0)
				{
					return Vector3.zero;
				}
				float num = count;
				return aggregatedFigures / num;
			}
		}

		public void addFigure(Vector3 v)
		{
			aggregatedFigures += v;
			count++;
		}
	}
}
