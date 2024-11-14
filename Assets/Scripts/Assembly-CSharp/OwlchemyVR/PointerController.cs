using UnityEngine;

namespace OwlchemyVR
{
	public class PointerController : MonoBehaviour
	{
		[SerializeField]
		private LineRenderer lineRenderer;

		private float maxLaserDistance = 50f;

		private int layerMask;

		private void Awake()
		{
			layerMask = LayerMaskHelper.EverythingBut(10, 12);
		}

		public Collider LineUpdate()
		{
			Collider result = null;
			float distance = maxLaserDistance;
			RaycastHit hitInfo;
			if (Physics.Raycast(lineRenderer.transform.position, lineRenderer.transform.forward, out hitInfo, maxLaserDistance, layerMask))
			{
				result = hitInfo.collider;
				distance = hitInfo.distance;
			}
			lineRenderer.SetPosition(1, new Vector3(0f, 0f, distance));
			return result;
		}
	}
}
