using UnityEngine;

namespace NobleMuffins.MuffinSlicer
{
	internal class Triangulation
	{
		private const float triangulationEpsilon = 0f;

		private static float Area(Vector2[] contour)
		{
			int num = contour.Length;
			float num2 = 0f;
			int num3 = num - 1;
			int num4 = 0;
			while (num4 < num)
			{
				num2 += contour[num3].x * contour[num4].y - contour[num4].x * contour[num3].y;
				num3 = num4++;
			}
			return num2 * 0.5f;
		}

		private static bool InsideTriangle(float Ax, float Ay, float Bx, float By, float Cx, float Cy, float Px, float Py)
		{
			float num = Cx - Bx;
			float num2 = Cy - By;
			float num3 = Ax - Cx;
			float num4 = Ay - Cy;
			float num5 = Bx - Ax;
			float num6 = By - Ay;
			float num7 = Px - Ax;
			float num8 = Py - Ay;
			float num9 = Px - Bx;
			float num10 = Py - By;
			float num11 = Px - Cx;
			float num12 = Py - Cy;
			float num13 = num * num10 - num2 * num9;
			float num14 = num5 * num8 - num6 * num7;
			float num15 = num3 * num12 - num4 * num11;
			return num13 >= 0f && num15 >= 0f && num14 >= 0f;
		}

		private static bool Snip(Vector2[] contour, int u, int v, int w, int n, int[] V)
		{
			float x = contour[V[u]].x;
			float y = contour[V[u]].y;
			float x2 = contour[V[v]].x;
			float y2 = contour[V[v]].y;
			float x3 = contour[V[w]].x;
			float y3 = contour[V[w]].y;
			if (0f > (x2 - x) * (y3 - y) - (y2 - y) * (x3 - x))
			{
				return false;
			}
			for (int i = 0; i < n; i++)
			{
				if (i != u && i != v && i != w)
				{
					float x4 = contour[V[i]].x;
					float y4 = contour[V[i]].y;
					if (InsideTriangle(x, y, x2, y2, x3, y3, x4, y4))
					{
						return false;
					}
				}
			}
			return true;
		}

		public static bool triangulate(Vector2[] contour, out int[] result)
		{
			int num = Mathf.Max(0, (contour.Length - 2) * 3);
			result = new int[num];
			int num2 = contour.Length;
			if (num2 < 3)
			{
				return false;
			}
			int[] array = new int[num2];
			if (0f < Area(contour))
			{
				for (int i = 0; i < num2; i++)
				{
					array[i] = i;
				}
			}
			else
			{
				for (int j = 0; j < num2; j++)
				{
					array[j] = num2 - 1 - j;
				}
			}
			int num3 = num2;
			int num4 = 2 * num3;
			int num5 = 0;
			int num6 = 0;
			int num7 = num3 - 1;
			while (num3 > 2)
			{
				if (0 >= num4--)
				{
					return false;
				}
				int num8 = num7;
				if (num3 <= num8)
				{
					num8 = 0;
				}
				num7 = num8 + 1;
				if (num3 <= num7)
				{
					num7 = 0;
				}
				int num9 = num7 + 1;
				if (num3 <= num9)
				{
					num9 = 0;
				}
				if (Snip(contour, num8, num7, num9, num3, array))
				{
					int num10 = array[num8];
					int num11 = array[num7];
					int num12 = array[num9];
					result[num5++] = num10;
					result[num5++] = num11;
					result[num5++] = num12;
					num6++;
					int num13 = num7;
					for (int k = num7 + 1; k < num3; k++)
					{
						array[num13] = array[k];
						num13++;
					}
					num3--;
					num4 = 2 * num3;
				}
			}
			return true;
		}
	}
}
