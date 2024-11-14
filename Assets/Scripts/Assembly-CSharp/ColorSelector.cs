using UnityEngine;

public class ColorSelector : MonoBehaviour
{
	public Camera refCamera;

	public GameObject selectorImage;

	public GameObject outerCursor;

	public GameObject innerCursor;

	public SpriteRenderer finalColorSprite;

	private Color finalColor;

	private Color selectedColor;

	private float selectorAngle;

	private Vector2 innerDelta = Vector2.zero;

	private static ColorSelector myslf;

	private void Awake()
	{
		myslf = this;
	}

	private void Start()
	{
		if (refCamera == null)
		{
			refCamera = Camera.main;
		}
		selectedColor = Color.red;
		SelectInnerColor(Vector2.zero);
		finalColorSprite.color = finalColor;
	}

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			UserInputUpdate();
		}
	}

	private void UserInputUpdate()
	{
		Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, base.transform.position.z - refCamera.transform.position.z);
		Ray ray = refCamera.ScreenPointToRay(position);
		RaycastHit hitInfo = default(RaycastHit);
		if (Physics.Raycast(ray, out hitInfo))
		{
			Vector3 vector = base.transform.InverseTransformPoint(hitInfo.point);
			float num = Vector2.Distance(Vector2.zero, vector);
			if ((double)num > 0.22)
			{
				SelectOuterColor(vector);
			}
			else
			{
				SelectInnerColor(vector);
			}
		}
	}

	private void SelectInnerColor(Vector2 delta)
	{
		float u = 0f;
		float v = 0f;
		float w = 0f;
		Barycentric(delta, ref u, ref v, ref w);
		if (u >= 0.15f && v >= -0.15f && w >= -0.15f)
		{
			Vector3 vector = new Vector3(selectedColor.r, selectedColor.g, selectedColor.b);
			Vector3 vector2 = u * vector + w * new Vector3(0f, 0f, 0f) + v * new Vector3(1f, 1f, 1f);
			finalColor = new Color(vector2.x, vector2.y, vector2.z);
			finalColorSprite.color = finalColor;
			innerCursor.transform.localPosition = delta;
			innerDelta = delta;
		}
	}

	private Vector3 ClampPosToCircle(Vector3 pos)
	{
		Vector3 zero = Vector3.zero;
		float num = 0.225f;
		float f = Mathf.Atan2(pos.x, pos.y);
		zero.x = num * Mathf.Sin(f);
		zero.y = num * Mathf.Cos(f);
		zero.z = pos.z;
		return zero;
	}

	private void Barycentric(Vector2 point, ref float u, ref float v, ref float w)
	{
		Vector2 vector = new Vector2(0f, 0.125f);
		Vector2 vector2 = new Vector2(-0.145f, -0.145f);
		Vector2 vector3 = new Vector2(0.145f, -0.145f);
		Vector2 vector4 = vector2 - vector;
		Vector2 vector5 = vector3 - vector;
		Vector2 lhs = point - vector;
		float num = Vector2.Dot(vector4, vector4);
		float num2 = Vector2.Dot(vector4, vector5);
		float num3 = Vector2.Dot(vector5, vector5);
		float num4 = Vector2.Dot(lhs, vector4);
		float num5 = Vector2.Dot(lhs, vector5);
		float num6 = num * num3 - num2 * num2;
		v = (num3 * num4 - num2 * num5) / num6;
		w = (num * num5 - num2 * num4) / num6;
		u = 1f - v - w;
	}

	private void SelectOuterColor(Vector2 delta)
	{
		float num = Mathf.Atan2(delta.x, delta.y);
		float num2 = num * 57.29578f;
		if (num2 < 0f)
		{
			num2 = 360f + num2;
		}
		selectorAngle = num2 / 360f;
		selectedColor = HSVToRGB(selectorAngle, 1f, 1f);
		selectorImage.GetComponent<Renderer>().material.SetColor("_Color", selectedColor);
		outerCursor.transform.localPosition = ClampPosToCircle(delta);
		SelectInnerColor(innerDelta);
	}

	public static Color HSVToRGB(float H, float S, float V)
	{
		if (S == 0f)
		{
			return new Color(V, V, V);
		}
		if (V == 0f)
		{
			return Color.black;
		}
		Color black = Color.black;
		float num = H * 6f;
		int num2 = Mathf.FloorToInt(num);
		float num3 = num - (float)num2;
		float num4 = V * (1f - S);
		float num5 = V * (1f - S * num3);
		float num6 = V * (1f - S * (1f - num3));
		switch (num2)
		{
		case -1:
			black.r = V;
			black.g = num4;
			black.b = num5;
			break;
		case 0:
			black.r = V;
			black.g = num6;
			black.b = num4;
			break;
		case 1:
			black.r = num5;
			black.g = V;
			black.b = num4;
			break;
		case 2:
			black.r = num4;
			black.g = V;
			black.b = num6;
			break;
		case 3:
			black.r = num4;
			black.g = num5;
			black.b = V;
			break;
		case 4:
			black.r = num6;
			black.g = num4;
			black.b = V;
			break;
		case 5:
			black.r = V;
			black.g = num4;
			black.b = num5;
			break;
		case 6:
			black.r = V;
			black.g = num6;
			black.b = num4;
			break;
		}
		black.r = Mathf.Clamp(black.r, 0f, 1f);
		black.g = Mathf.Clamp(black.g, 0f, 1f);
		black.b = Mathf.Clamp(black.b, 0f, 1f);
		return black;
	}

	public static Color GetColor()
	{
		return myslf.finalColor;
	}
}
