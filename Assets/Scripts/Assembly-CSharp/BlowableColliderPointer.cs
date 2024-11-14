using UnityEngine;

public class BlowableColliderPointer : MonoBehaviour
{
	[SerializeField]
	private BlowableItem blowableItem;

	public BlowableItem BlowableItem
	{
		get
		{
			return blowableItem;
		}
	}
}
