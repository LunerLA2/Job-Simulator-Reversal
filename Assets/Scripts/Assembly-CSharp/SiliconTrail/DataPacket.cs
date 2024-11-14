using UnityEngine;
using UnityEngine.UI;

namespace SiliconTrail
{
	public class DataPacket : MonoBehaviour
	{
		private const float WORLD_LIMIT = 360f;

		[SerializeField]
		private float corruptionChance = 0.5f;

		[SerializeField]
		private float minSpeed = 5f;

		[SerializeField]
		private float maxSpeed = 10f;

		[SerializeField]
		private float doubleSizeChanceThreshold = 0.3f;

		[SerializeField]
		private float quadrupleSizeChanceThreshold;

		[SerializeField]
		private Text text;

		[SerializeField]
		private Color healthyColor;

		[SerializeField]
		private Color corruptedColor;

		[SerializeField]
		private BoxCollider2D boxCollider;

		private int data;

		private float speed;

		public int Data
		{
			get
			{
				return data;
			}
		}

		private void Awake()
		{
			data = 1;
			float num = Random.Range(0f, 1f);
			if (num < quadrupleSizeChanceThreshold)
			{
				data *= 4;
			}
			else if (num < doubleSizeChanceThreshold)
			{
				data *= 2;
			}
			if (Random.Range(0f, 1f) < corruptionChance)
			{
				data = -data;
			}
			speed = Random.Range(minSpeed, maxSpeed);
			text.color = ((data <= 0) ? corruptedColor : healthyColor);
			RectTransform component = text.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(component.sizeDelta.x * (float)Mathf.Abs(data), component.sizeDelta.y);
			boxCollider.size = new Vector2(boxCollider.size.x * (float)Mathf.Abs(data), boxCollider.size.y);
		}

		private void Update()
		{
			base.transform.Translate(speed * Time.deltaTime, 0f, 0f);
			if (base.transform.position.y > 360f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
