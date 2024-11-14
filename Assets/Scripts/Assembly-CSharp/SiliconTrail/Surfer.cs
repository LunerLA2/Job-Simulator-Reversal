using UnityEngine;

namespace SiliconTrail
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Surfer : MonoBehaviour
	{
		[SerializeField]
		private float moveForce;

		private Rigidbody2D rb;

		public event SurferHitHandler Hit;

		private void Awake()
		{
			rb = GetComponent<Rigidbody2D>();
		}

		public void MoveUp()
		{
			rb.AddForce(Vector2.up * moveForce, ForceMode2D.Impulse);
		}

		public void MoveDown()
		{
			rb.AddForce(Vector2.down * moveForce, ForceMode2D.Impulse);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			DataPacket component = other.GetComponent<DataPacket>();
			if (component != null && this.Hit != null)
			{
				this.Hit(component);
			}
		}
	}
}
