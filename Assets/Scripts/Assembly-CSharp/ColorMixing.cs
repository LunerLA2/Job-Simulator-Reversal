using UnityEngine;

public class ColorMixing : MonoBehaviour
{
	[SerializeField]
	private Renderer r;

	[SerializeField]
	private Color c1;

	[Range(0f, 1f)]
	[SerializeField]
	private float c1Percentage;

	[SerializeField]
	private Color c2;

	private float c2Percentage;

	[SerializeField]
	private bool rgbMode = true;

	private void Start()
	{
	}

	private void Update()
	{
		MixColors();
	}

	private void MixColors()
	{
		c2Percentage = 1f - c1Percentage;
		Color color = default(Color);
		if (rgbMode)
		{
			color = c1 * c1Percentage + c2 * c2Percentage;
		}
		else
		{
			CMYK cMYK = RGBtoCMYK((int)(c1.r * 255f), (int)(c1.g * 255f), (int)(c1.b * 255f));
			CMYK cMYK2 = RGBtoCMYK((int)(c2.r * 255f), (int)(c2.g * 255f), (int)(c2.b * 255f));
			float num = cMYK.Cyan * c1Percentage + cMYK2.Cyan * c2Percentage;
			float num2 = cMYK.Magenta * c1Percentage + cMYK2.Magenta * c2Percentage;
			float num3 = cMYK.Yellow * c1Percentage + cMYK2.Yellow * c2Percentage;
			float num4 = cMYK.Black * c1Percentage + cMYK2.Black * c2Percentage;
			color = CMYKtoRGB(num, num2, num3, num4);
		}
		r.material.color = color;
	}

	public static Color CMYKtoRGB(CMYK cmyk)
	{
		return CMYKtoRGB(cmyk.Cyan, cmyk.Magenta, cmyk.Yellow, cmyk.Black);
	}

	public static Color CMYKtoRGB(double c, double m, double y, double k)
	{
		int num = (int)((1.0 - c) * (1.0 - k) * 255.0);
		int num2 = (int)((1.0 - m) * (1.0 - k) * 255.0);
		int num3 = (int)((1.0 - y) * (1.0 - k) * 255.0);
		return new Color((float)num / 255f, (float)num2 / 255f, (float)num3 / 255f);
	}

	public static CMYK RGBtoCMYK(int red, int green, int blue)
	{
		float num = (255f - (float)red) / 255f;
		float num2 = (255f - (float)green) / 255f;
		float num3 = (255f - (float)blue) / 255f;
		float num4 = Mathf.Min(num, Mathf.Min(num2, num3));
		if ((double)num4 == 1.0)
		{
			return new CMYK(0f, 0f, 0f, 1f);
		}
		return new CMYK((num - num4) / (1f - num4), (num2 - num4) / (1f - num4), (num3 - num4) / (1f - num4), num4);
	}

	public static CIEXYZ RGBtoXYZ(int red, int green, int blue)
	{
		float num = (float)red / 255f;
		float num2 = (float)green / 255f;
		float num3 = (float)blue / 255f;
		float num4 = ((!(num > 0.04045f)) ? (num / 12.92f) : Mathf.Pow((num + 0.055f) / 1.055f, 2.2f));
		float num5 = ((!(num2 > 0.04045f)) ? (num2 / 12.92f) : Mathf.Pow((num2 + 0.055f) / 1.055f, 2.2f));
		float num6 = ((!(num3 > 0.04045f)) ? (num3 / 12.92f) : Mathf.Pow((num3 + 0.055f) / 1.055f, 2.2f));
		return new CIEXYZ(num4 * 0.4124f + num5 * 0.3576f + num6 * 0.1805f, num4 * 0.2126f + num5 * 0.7152f + num6 * 0.0722f, num4 * 0.0193f + num5 * 0.1192f + num6 * 0.9505f);
	}

	public static CIELab RGBtoLab(int red, int green, int blue)
	{
		return XYZtoLab(RGBtoXYZ(red, green, blue));
	}

	private static float Fxyz(float t)
	{
		return (!(t > 0.008856f)) ? (7.787f * t + 0.13793103f) : Mathf.Pow(t, 1f / 3f);
	}

	public static CIELab XYZtoLab(CIEXYZ xyz)
	{
		return XYZtoLab(xyz.X, xyz.Y, xyz.Z);
	}

	public static CIELab XYZtoLab(float x, float y, float z)
	{
		CIELab empty = CIELab.Empty;
		empty.L = 116f * Fxyz(y / CIEXYZ.D65.Y) - 16f;
		empty.A = 500f * (Fxyz(x / CIEXYZ.D65.X) - Fxyz(y / CIEXYZ.D65.Y));
		empty.B = 200f * (Fxyz(y / CIEXYZ.D65.Y) - Fxyz(z / CIEXYZ.D65.Z));
		return empty;
	}

	public static CIEXYZ LabtoXYZ(float l, float a, float b)
	{
		float num = 0.20689656f;
		float num2 = (l + 16f) / 116f;
		float num3 = num2 + a / 500f;
		float num4 = num2 - b / 200f;
		return new CIEXYZ((!(num3 > num)) ? ((num3 - 0.13793103f) * 3f * (num * num) * CIEXYZ.D65.X) : (CIEXYZ.D65.X * (num3 * num3 * num3)), (!(num2 > num)) ? ((num2 - 0.13793103f) * 3f * (num * num) * CIEXYZ.D65.Y) : (CIEXYZ.D65.Y * (num2 * num2 * num2)), (!(num4 > num)) ? ((num4 - 0.13793103f) * 3f * (num * num) * CIEXYZ.D65.Z) : (CIEXYZ.D65.Z * (num4 * num4 * num4)));
	}

	public static Color LabtoRGB(CIELab lab)
	{
		return LabtoRGB(lab.L, lab.A, lab.B);
	}

	public static Color LabtoRGB(float l, float a, float b)
	{
		return XYZtoRGB(LabtoXYZ(l, a, b));
	}

	public static Color XYZtoRGB(CIEXYZ cieXYZ)
	{
		return XYZtoRGB(cieXYZ.X, cieXYZ.Y, cieXYZ.Z);
	}

	public static Color XYZtoRGB(float x, float y, float z)
	{
		float[] array = new float[3]
		{
			x * 3.241f - y * 1.5374f - z * 0.4986f,
			(0f - x) * 0.9692f + y * 1.876f - z * 0.0416f,
			x * 0.0556f - y * 0.204f + z * 1.057f
		};
		for (int i = 0; i < 3; i++)
		{
			array[i] = ((!(array[i] <= 0.0031308f)) ? (1.055f * Mathf.Pow(array[i], 5f / 12f) - 0.055f) : (12.92f * array[i]));
		}
		return new Color(array[0], array[1], array[2]);
	}
}
