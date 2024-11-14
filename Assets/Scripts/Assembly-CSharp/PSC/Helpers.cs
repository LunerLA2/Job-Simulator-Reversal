using UnityEngine;

namespace PSC
{
	public static class Helpers
	{
		public static bool ApproximatelyEqual(Transform x, Transform y)
		{
			if (x.localPosition != y.localPosition)
			{
				return false;
			}
			if (x.localRotation != y.localRotation)
			{
				return false;
			}
			if (x.localScale != y.localScale)
			{
				return false;
			}
			if (x.gameObject.activeSelf != y.gameObject.activeSelf)
			{
				return false;
			}
			return true;
		}

		public static bool ApproximatelyEqual(Placement x, Transform y)
		{
			if (x.localPosition != y.localPosition)
			{
				return false;
			}
			if (x.localRotation != y.localRotation)
			{
				return false;
			}
			if (x.localScale != y.localScale)
			{
				return false;
			}
			if (x.activeSelf != y.gameObject.activeSelf)
			{
				return false;
			}
			return true;
		}

		public static bool ApproximatelyEqual(Placement x, Placement y)
		{
			if (x.localPosition != y.localPosition)
			{
				return false;
			}
			if (x.localRotation != y.localRotation)
			{
				return false;
			}
			if (x.localScale != y.localScale)
			{
				return false;
			}
			if (x.activeSelf != y.activeSelf)
			{
				return false;
			}
			return true;
		}

		public static void CopyPlacements(Transform to, Transform from)
		{
			to.localPosition = from.localPosition;
			to.localScale = from.localScale;
			to.localRotation = from.localRotation;
			to.gameObject.SetActive(from.gameObject.activeSelf);
		}
	}
}
