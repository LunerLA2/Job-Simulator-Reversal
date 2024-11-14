using UnityEngine;

namespace SiliconTrail
{
	public class TrailManager : MonoBehaviour
	{
		[SerializeField]
		private float placeSpacing = 300f;

		[SerializeField]
		private Transform placesRoot;

		[SerializeField]
		private Transform scenery;

		[SerializeField]
		private float maxSceneryOffset;

		[SerializeField]
		private TrailPlaceNode placeNodePrefab;

		[SerializeField]
		private Animation travelAnimation;

		[SerializeField]
		private AnimationClip travelAnimClip;

		[SerializeField]
		private AnimationClip idleAnimClip;

		private TrailPlaceNode[] placeNodes;

		private void OnEnable()
		{
			travelAnimation.CrossFade(idleAnimClip.name, 0.75f);
		}

		public void Reset()
		{
			SetHorizontalOffset(placesRoot, 0f);
			SetHorizontalOffset(scenery, 0f);
			if (placeNodes != null)
			{
				for (int i = 0; i < placeNodes.Length; i++)
				{
					Object.Destroy(placeNodes[i].gameObject);
				}
				placeNodes = null;
			}
		}

		public void PopulatePlaces(Place[] places)
		{
			placeNodes = new TrailPlaceNode[places.Length];
			for (int i = 0; i < places.Length; i++)
			{
				TrailPlaceNode trailPlaceNode = Object.Instantiate(placeNodePrefab);
				trailPlaceNode.name = placeNodePrefab.name;
				trailPlaceNode.transform.SetParent(placesRoot, false);
				trailPlaceNode.transform.localPosition = new Vector3((float)(-i) * placeSpacing, 0f, 0f);
				trailPlaceNode.SetPlace(places[i].Name);
				placeNodes[i] = trailPlaceNode;
			}
		}

		public void SetProgress(float progress)
		{
			float num = progress / (float)(placeNodes.Length - 1);
			SetHorizontalOffset(placesRoot, progress * placeSpacing);
			SetHorizontalOffset(scenery, num * maxSceneryOffset);
			travelAnimation.CrossFade(travelAnimClip.name, 0.75f);
		}

		private void SetHorizontalOffset(Transform t, float offset)
		{
			Vector3 localPosition = t.localPosition;
			localPosition.x = offset;
			t.localPosition = localPosition;
		}
	}
}
