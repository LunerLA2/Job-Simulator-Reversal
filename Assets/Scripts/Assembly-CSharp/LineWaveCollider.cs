using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineWaveCollider : MonoBehaviour
{
	public enum Origins
	{
		Start = 0,
		Middle = 1
	}

	public enum WaveColliders
	{
		Box = 0,
		Sphere = 1
	}

	private float ampT;

	public Material traceMaterial;

	public float traceWidth = 0.3f;

	public GameObject targetOptional;

	public float altRotation;

	public Origins origin;

	public int size = 300;

	public float lengh = 10f;

	public float freq = 2.5f;

	public float amp = 1f;

	public bool ampByFreq;

	public bool centered = true;

	public bool centCrest = true;

	public bool warp = true;

	public bool warpInvert;

	public float warpRandom;

	public float walkManual;

	public float walkAuto;

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

	private List<Vector3> linePositions;

	public WaveColliders waveCollider;

	private List<GameObject> colliders;

	private GameObject Colliders;

	private GameObject colliderGO;

	private Type colType;

	public float colliderSize = 0.2f;

	public int collidersGap = 5;

	private void Awake()
	{
		lrComp = GetComponent<LineRenderer>();
		lrComp.useWorldSpace = false;
		lrComp.material = traceMaterial;
		linePositions = new List<Vector3>();
		colliders = new List<GameObject>();
		Colliders = new GameObject("Colliders");
		Colliders.transform.parent = base.transform;
		colliderGO = new GameObject("col");
		colliderGO.transform.position = base.transform.position;
		colliderGO.transform.parent = Colliders.transform;
	}

	private void Update()
	{
		lrComp.SetWidth(traceWidth, traceWidth);
		base.gameObject.transform.localScale = Vector3.one;
		if (targetOptional != null)
		{
			origin = Origins.Start;
			lengh = (base.transform.position - targetOptional.transform.position).magnitude;
			base.transform.LookAt(targetOptional.transform.position);
			base.transform.Rotate(altRotation, -90f, 0f);
		}
		if (warpRandom <= 0f)
		{
			warpRandom = 0f;
		}
		if (size <= 2)
		{
			size = 2;
		}
		lrComp.SetVertexCount(size);
		if (ampByFreq)
		{
			ampT = Mathf.Sin(freq * (float)Math.PI);
		}
		else
		{
			ampT = 1f;
		}
		ampT *= amp;
		if (warp && warpInvert)
		{
			ampT /= 2f;
		}
		foreach (GameObject collider in colliders)
		{
			UnityEngine.Object.Destroy(collider);
		}
		linePositions.Clear();
		colliders.Clear();
		for (int i = 0; i < size; i++)
		{
			angle = (float)Math.PI * 2f / (float)size * (float)i * freq;
			if (centered)
			{
				angle -= freq * (float)Math.PI;
				if (centCrest)
				{
					angle -= (float)Math.PI / 2f;
				}
			}
			else
			{
				centCrest = false;
			}
			walkShift -= walkAuto / (float)size * Time.deltaTime;
			angle += (float)walkShift - walkManual;
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
				start = lengh / 2f;
			}
			if (warp)
			{
				warpT = size - i;
				warpT /= size;
				warpT = Mathf.Sin((float)Math.PI * warpT * (warpRandom + 1f));
				if (warpInvert)
				{
					warpT = 1f / warpT;
				}
				linePositions.Add(new Vector3(lengh / (float)size * (float)i - start, sinAngle * ampT * warpT, sinAngleZ * ampT * warpT));
			}
			else
			{
				linePositions.Add(new Vector3(lengh / (float)size * (float)i - start, sinAngle * ampT, sinAngleZ * ampT));
				warpInvert = false;
			}
			if (i == 1)
			{
				posVtx2 = new Vector3(lengh / (float)size * (float)i - start, sinAngle * ampT * warpT, sinAngleZ * ampT * warpT);
			}
			if (i == size - 1)
			{
				posVtxSizeMinusOne = new Vector3(lengh / (float)size * (float)i - start, sinAngle * ampT * warpT, sinAngleZ * ampT * warpT);
			}
		}
		switch (waveCollider)
		{
		case WaveColliders.Box:
			colType = typeof(BoxCollider);
			break;
		case WaveColliders.Sphere:
			colType = typeof(SphereCollider);
			break;
		}
		if (colliderGO.GetComponent(colType) == null)
		{
			UnityEngine.Object.Destroy(colliderGO.GetComponent(typeof(Collider)));
			colliderGO.AddComponent(colType);
		}
		colliderGO.SetActive(true);
		colliderGO.transform.localScale = new Vector3(colliderSize, colliderSize, colliderSize);
		collidersGap = Mathf.Clamp(collidersGap, 1, 20);
		for (int j = 0; j < linePositions.Count; j++)
		{
			lrComp.SetPosition(j, linePositions[j]);
			if (j % collidersGap == 0)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(colliderGO);
				gameObject.transform.parent = Colliders.transform;
				if ((bool)targetOptional)
				{
					gameObject.transform.position = Vector3.Lerp(base.gameObject.transform.position, targetOptional.transform.position, (float)j / (float)linePositions.Count);
					gameObject.transform.Translate(0f, linePositions[j].y, linePositions[j].z, base.gameObject.transform);
				}
				else
				{
					gameObject.transform.position = base.gameObject.transform.position;
					gameObject.transform.Translate(linePositions[j].x, linePositions[j].y, linePositions[j].z, base.gameObject.transform);
				}
				gameObject.transform.rotation = base.gameObject.transform.rotation;
				colliders.Add(gameObject);
			}
		}
		colliderGO.SetActive(false);
		if (warpInvert)
		{
			lrComp.SetPosition(0, posVtx2);
			lrComp.SetPosition(size - 1, posVtxSizeMinusOne);
		}
	}
}
