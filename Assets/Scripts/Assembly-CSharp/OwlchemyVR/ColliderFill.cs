using UnityEngine;

namespace OwlchemyVR
{
	public class ColliderFill : MonoBehaviour
	{
		[SerializeField]
		private bool displayWhenNotSelected = true;

		[SerializeField]
		private bool renderTriggers;

		[SerializeField]
		private Color drawColor = new Color(0f, 1f, 0f, 0.71f);

		public void Setup(bool _displayWhenNotSelected, bool _renderTriggers, Color _color)
		{
			displayWhenNotSelected = _displayWhenNotSelected;
			renderTriggers = _renderTriggers;
			drawColor = _color;
		}

		private void OnDrawGizmos()
		{
			if (displayWhenNotSelected)
			{
				RenderGizmos();
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (!displayWhenNotSelected)
			{
				RenderGizmos();
			}
		}

		private void RenderGizmos()
		{
			Color color = Gizmos.color;
			Gizmos.color = drawColor;
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			Matrix4x4 matrix = Gizmos.matrix;
			Collider[] array = componentsInChildren;
			foreach (Collider collider in array)
			{
				if (!collider.isTrigger || renderTriggers)
				{
					Transform transform = collider.transform;
					Gizmos.matrix = transform.localToWorldMatrix;
					if (collider.GetType() == typeof(BoxCollider))
					{
						BoxCollider boxCollider = (BoxCollider)collider;
						Gizmos.DrawCube(boxCollider.center, boxCollider.size);
					}
					else if (collider.GetType() == typeof(SphereCollider))
					{
						SphereCollider sphereCollider = (SphereCollider)collider;
						Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
					}
					else if (collider.GetType() != typeof(CapsuleCollider))
					{
					}
				}
			}
			Gizmos.color = color;
			Gizmos.matrix = matrix;
		}
	}
}
