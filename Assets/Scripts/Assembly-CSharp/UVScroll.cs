using UnityEngine;

public class UVScroll : MonoBehaviour
{
	public Vector2 offsetAmount;

	private Vector2 offset;

	public float manualX;

	public float manualY;

	public bool x;

	public bool y;

	private Material mat;

	private void Awake()
	{
		mat = GetComponent<Renderer>().material;
	}

	private void Update()
	{
		if (x)
		{
			offset.x += offsetAmount.x * Time.deltaTime;
			offset.x %= 1f;
		}
		if (y)
		{
			offset.y += offsetAmount.y * Time.deltaTime;
			offset.y %= 1f;
		}
		mat.mainTextureOffset = offset + Vector2.right * manualX + Vector2.up * manualY;
	}
}
