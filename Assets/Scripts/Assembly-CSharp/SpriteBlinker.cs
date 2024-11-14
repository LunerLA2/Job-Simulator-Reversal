using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteBlinker : MonoBehaviour
{
	[SerializeField]
	private float blinkPeriod = 0.5f;

	private Image image;

	private void Awake()
	{
		image = GetComponent<Image>();
	}

	private void Update()
	{
		Color color = image.color;
		color.a = (int)(Time.time / blinkPeriod) % 2;
		if (image.color != color)
		{
			image.color = color;
		}
	}
}
