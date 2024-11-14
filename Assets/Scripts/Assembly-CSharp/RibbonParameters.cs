using System;
using UnityEngine;

public class RibbonParameters : ScriptableObject
{
	public int boneCount = 2;

	public int numSides = 4;

	public int resolution = 4;

	public float baseWidth = 0.045f;

	public float baseHeight = 0.002f;

	public float length = 0.5f;

	public bool capped = true;

	public AnimationCurve widthCurve = new AnimationCurve(new Keyframe(0f, 1f, 0f, 0f), new Keyframe(1f, 1f, 0f, 0f));

	public AnimationCurve heightCurve = new AnimationCurve(new Keyframe(0f, 1f, 0f, 0f), new Keyframe(1f, 1f, 0f, 0f));

	private Mesh mesh;

	public Mesh RebuildMesh()
	{
		if (mesh == null)
		{
			mesh = new Mesh();
			mesh.hideFlags = HideFlags.HideAndDontSave;
		}
		int num = (boneCount - 1) * resolution;
		int num2 = num + 1;
		float num3 = baseHeight / 2f;
		float num4 = baseWidth / 2f;
		float num5 = length / (float)num;
		float[] array = new float[numSides];
		float[] array2 = new float[numSides];
		float num6 = 0.0001f;
		float num7 = 0.0001f;
		for (int i = 0; i < numSides; i++)
		{
			float f = (float)Math.PI * 2f * (0f - (0.25f + ((float)i - 0.5f) / (float)numSides));
			array[i] = Mathf.Sin(f);
			array2[i] = Mathf.Cos(f);
			num6 = Mathf.Max(num6, Mathf.Abs(array[i]));
			num7 = Mathf.Max(num6, Mathf.Abs(array2[i]));
		}
		for (int j = 0; j < numSides; j++)
		{
			array[j] = Mathf.Clamp(array[j] / num6, -1f, 1f);
			array2[j] = Mathf.Clamp(array2[j] / num7, -1f, 1f);
		}
		Vector3[] array3 = new Vector3[num2 * numSides + (capped ? 2 : 0)];
		for (int k = 0; k < num2; k++)
		{
			float time = (float)k / (float)num;
			float num8 = num4 * widthCurve.Evaluate(time);
			float num9 = num3 * heightCurve.Evaluate(time);
			float z = (float)k * num5;
			int num10 = k * numSides;
			for (int l = 0; l < numSides; l++)
			{
				array3[num10 + l] = new Vector3(array2[l] * num8, array[l] * num9, z);
			}
		}
		Vector3[] array4 = new Vector3[array3.Length];
		for (int m = 0; m < num2 * numSides; m++)
		{
			if (numSides == 2 || numSides == 4)
			{
				array4[m] = new Vector3(0f, Mathf.Sign(array3[m].y), 0f);
				continue;
			}
			int num11 = m % numSides;
			array4[m] = new Vector3(array2[num11], array[num11], 0f);
		}
		if (capped)
		{
			array3[num2 * numSides] = Vector3.zero;
			array3[num2 * numSides + 1] = new Vector3(0f, 0f, (float)num * num5);
			array4[num2 * numSides] = Vector3.back;
			array4[num2 * numSides + 1] = Vector3.forward;
		}
		int num12 = num * numSides * 2 + (capped ? (numSides * 2) : 0);
		int[] array5 = new int[num12 * 3];
		int num13 = numSides * 6;
		for (int n = 0; n < num; n++)
		{
			for (int num14 = 0; num14 < numSides; num14++)
			{
				int num15 = n * num13 + num14 * 6;
				int num16 = n * numSides + num14;
				int num17 = (n + 1) * numSides + num14;
				array5[num15] = num16;
				array5[num15 + 1] = ((num14 <= 0) ? (num16 + numSides - 1) : (num16 - 1));
				array5[num15 + 2] = ((num14 <= 0) ? (num17 + numSides - 1) : (num17 - 1));
				array5[num15 + 3] = ((num14 <= 0) ? (num17 + numSides - 1) : (num17 - 1));
				array5[num15 + 4] = num17;
				array5[num15 + 5] = num16;
			}
		}
		if (capped)
		{
			int num18 = num * num13;
			int num19 = num2 * numSides;
			for (int num20 = 0; num20 < numSides; num20++)
			{
				int num21 = num18 + num20 * 3;
				array5[num21] = num20;
				array5[num21 + 1] = (num20 + 1) % numSides;
				array5[num21 + 2] = num19;
			}
			int num22 = num18 + numSides * 3;
			int num23 = num * numSides;
			int num24 = num2 * numSides + 1;
			for (int num25 = 0; num25 < numSides; num25++)
			{
				int num26 = num22 + num25 * 3;
				array5[num26] = num23 + (num25 + 1) % numSides;
				array5[num26 + 1] = num23 + num25;
				array5[num26 + 2] = num24;
			}
		}
		AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 1f, 0f, 0f), new Keyframe(1f, 0f, 0f, 0f));
		BoneWeight[] array6 = new BoneWeight[array3.Length];
		for (int num27 = 0; num27 < num2; num27++)
		{
			int num28 = num27 / resolution;
			int num29 = num27 * numSides;
			float num30 = animationCurve.Evaluate((float)(num27 % resolution) / (float)resolution);
			BoneWeight boneWeight = default(BoneWeight);
			boneWeight.boneIndex0 = num28;
			boneWeight.weight0 = num30;
			BoneWeight boneWeight2 = boneWeight;
			if (num27 < num)
			{
				boneWeight2.boneIndex1 = num28 + 1;
				boneWeight2.weight1 = 1f - num30;
			}
			for (int num31 = 0; num31 < numSides; num31++)
			{
				array6[num29 + num31] = boneWeight2;
			}
		}
		if (capped)
		{
			array6[num2 * numSides] = new BoneWeight
			{
				boneIndex0 = 0,
				weight0 = 1f
			};
			array6[num2 * numSides + 1] = new BoneWeight
			{
				boneIndex0 = boneCount - 1,
				weight0 = 1f
			};
		}
		Matrix4x4[] array7 = new Matrix4x4[boneCount];
		for (int num32 = 0; num32 < array7.Length; num32++)
		{
			array7[num32] = Matrix4x4.TRS(new Vector3(0f, 0f, 0f - (float)num32 / (float)(array7.Length - 1) * length), Quaternion.identity, Vector3.one);
		}
		mesh.Clear();
		mesh.vertices = array3;
		mesh.normals = array4;
		mesh.SetIndices(array5, MeshTopology.Triangles, 0);
		mesh.boneWeights = array6;
		mesh.bindposes = array7;
		return mesh;
	}
}
