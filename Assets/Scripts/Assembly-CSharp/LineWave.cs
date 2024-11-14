using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class LineWave : MonoBehaviour
{
	[HideInInspector]
	public enum Origins
	{
		Start = 0,
		Middle = 1
	}

	private float ampT;

	private float prevAmpT = -1f;

	public Material lineMaterial;

	public float lineThickness = 0.3f;

	[HideInInspector]
	public GameObject targetOptional;

	[HideInInspector]
	public float altRotation;

	[HideInInspector]
	public Origins origin;

	public int numEdgesOnLine = 150;

	public float width = 13f;

	public float frequency = 3f;

	public float amplitude = 1f;

	[HideInInspector]
	public bool ampByFreq;

	public bool centerLine = true;

	public bool centerLineViaCrest = true;

	public bool warp = true;

	public bool warpInvert;

	public float warpRandom;

	public float slideManually;

	public float slideAutomatically;

	public float mouthLineEdgesPercent = 0.15f;

	public float mouthLineEdgesTransitionAmount = 0.1f;

	[HideInInspector]
	public bool spiral;

	private float start;

	private float warpT;

	private float angle;

	private float sinAngle;

	private float sinAngleZ;

	private double walkShift;

	private Vector3 posVtx2;

	private Vector3 posVtxSizeMinusOne;

	private LineRenderer lrComp;

	private bool isVisible;

	public void SetIsVisible(bool v)
	{
		isVisible = v;
	}

	private void Awake()
	{
		lrComp = GetComponent<LineRenderer>();
		lrComp.useWorldSpace = false;
		lrComp.material = lineMaterial;
	}

	private void Update()
	{
		if (!isVisible || (prevAmpT == 0f && amplitude == 0f))
		{
			return;
		}
		lrComp.SetWidth(lineThickness, lineThickness);
		if (targetOptional != null)
		{
			origin = Origins.Start;
			width = (base.transform.position - targetOptional.transform.position).magnitude;
			base.transform.LookAt(targetOptional.transform.position);
			base.transform.Rotate(altRotation, -90f, 0f);
		}
		if (warpRandom <= 0f)
		{
			warpRandom = 0f;
		}
		if (numEdgesOnLine <= 2)
		{
			numEdgesOnLine = 2;
		}
		lrComp.SetVertexCount(numEdgesOnLine);
		if (ampByFreq)
		{
			ampT = Mathf.Sin(frequency * (float)Math.PI);
		}
		else
		{
			ampT = 1f;
		}
		ampT *= amplitude;
		prevAmpT = ampT;
		if (warp && warpInvert)
		{
			ampT /= 2f;
		}
		for (int i = 0; i < numEdgesOnLine; i++)
		{
			angle = (float)Math.PI * 2f / (float)numEdgesOnLine * (float)i * frequency;
			if (centerLine)
			{
				angle -= frequency * (float)Math.PI;
				if (centerLineViaCrest)
				{
					angle -= (float)Math.PI / 2f;
				}
			}
			else
			{
				centerLineViaCrest = false;
			}
			walkShift -= slideAutomatically / (float)numEdgesOnLine * Time.deltaTime;
			angle += (float)walkShift - slideManually;
			sinAngle = Mathf.Sin(angle);
			if (spiral)
			{
				sinAngleZ = Mathf.Cos(angle);
			}
			else
			{
				sinAngleZ = 0f;
			}
			if (origin == Origins.Start)
			{
				start = 0f;
			}
			else
			{
				start = width / 2f;
			}
			float num = 1f;
			float num2 = (float)numEdgesOnLine * mouthLineEdgesPercent;
			float num3 = (float)numEdgesOnLine * (1f - mouthLineEdgesPercent);
			if ((float)i < num2 || (float)i > num3)
			{
				num = 0f;
			}
			else if (Mathf.Abs((float)i - num2) < mouthLineEdgesTransitionAmount)
			{
				num = Mathf.Abs((float)i - num2) / mouthLineEdgesTransitionAmount;
			}
			else if (Mathf.Abs((float)i - num3) < mouthLineEdgesTransitionAmount)
			{
				num = Mathf.Abs((float)i - num3) / mouthLineEdgesTransitionAmount;
			}
			if (warp)
			{
				warpT = numEdgesOnLine - i;
				warpT /= numEdgesOnLine;
				warpT = Mathf.Sin((float)Math.PI * warpT * (warpRandom + 1f));
				if (warpInvert)
				{
					warpT = 1f / warpT;
				}
				lrComp.SetPosition(i, new Vector3(width / (float)numEdgesOnLine * (float)i - start, sinAngle * ampT * warpT * num, sinAngleZ * ampT * warpT * num));
			}
			else
			{
				lrComp.SetPosition(i, new Vector3(width / (float)numEdgesOnLine * (float)i - start, sinAngle * ampT * num, sinAngleZ * ampT * num));
				warpInvert = false;
			}
			if (i == 1)
			{
				posVtx2 = new Vector3(width / (float)numEdgesOnLine * (float)i - start, sinAngle * ampT * warpT, sinAngleZ * ampT * warpT);
			}
			if (i == numEdgesOnLine - 1)
			{
				posVtxSizeMinusOne = new Vector3(width / (float)numEdgesOnLine * (float)i - start, sinAngle * ampT * warpT, sinAngleZ * ampT * warpT);
			}
		}
		if (warpInvert)
		{
			lrComp.SetPosition(0, posVtx2);
			lrComp.SetPosition(numEdgesOnLine - 1, posVtxSizeMinusOne);
		}
	}
}
