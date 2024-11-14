using UnityEngine;

namespace OwlchemyVR
{
	public class ColliderGrabbableItemPointer : MonoBehaviour
	{
		[SerializeField]
		public GrabbableItem grabbableItem;

		public GrabbableItem GrabbableItem
		{
			get
			{
				return grabbableItem;
			}
		}
	}
}
