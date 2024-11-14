using System;
using UnityEngine;

public class SimpleClickableButton : MonoBehaviour
{
	[SerializeField]
	private Vector2 hitboxSize;

	public Action<SimpleClickableButton> OnClicked;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePosition = Input.mousePosition;
			Vector2 vector = new Vector2(base.transform.position.x - hitboxSize.x / 2f, base.transform.position.y - hitboxSize.y / 2f);
			Vector2 vector2 = new Vector2(base.transform.position.x + hitboxSize.x / 2f, base.transform.position.y + hitboxSize.y / 2f);
			if (mousePosition.x > vector.x && mousePosition.x < vector2.x && mousePosition.y > vector.y && mousePosition.y < vector2.y && OnClicked != null)
			{
				OnClicked(this);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(base.transform.position, new Vector3(hitboxSize.x, hitboxSize.y, 0f));
	}
}
