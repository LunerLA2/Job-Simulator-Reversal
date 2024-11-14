using System;
using System.Linq;
using UnityEngine;

public class FluidInContainerController : MonoBehaviour
{
	private const float FADE_DISTANCE = 0.01f;

	[SerializeField]
	private MeshFilter meshFilter;

	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private ParticleSystem steamParticle;

	[SerializeField]
	private ParticleSystem bubblesParticle;

	[SerializeField]
	private Transform surfaceTransform;

	private int numSides;

	private float radius;

	private float height;

	private Mesh mesh;

	private Vector3[] vertices;

	private float[] sines;

	private float[] cosines;

	private Vector3[] normVerts;

	private Quaternion surfaceRot;

	private ContainerFluidSystem parentContainer;

	private bool fluidHitRim;

	private int fluidSurfaceCenterPropId;

	private int fluidSurfaceNormalPropId;

	public Transform SurfaceTransform
	{
		get
		{
			return surfaceTransform;
		}
	}

	public bool FluidHitRim
	{
		get
		{
			return fluidHitRim;
		}
	}

	private void Awake()
	{
		fluidSurfaceCenterPropId = Shader.PropertyToID("_SurfaceCenter");
		fluidSurfaceNormalPropId = Shader.PropertyToID("_SurfaceNormal");
	}

	public void BuildMesh(int numSides, bool includeSides, bool includeBottom)
	{
		mesh = new Mesh();
		mesh.MarkDynamic();
		meshFilter.sharedMesh = mesh;
		meshRenderer.material = meshRenderer.material;
		if (includeSides || includeBottom)
		{
			meshRenderer.material.SetFloat("_FadeDistance", 0f);
		}
		else
		{
			meshRenderer.material.SetFloat("_FadeDistance", 0.01f);
		}
		surfaceTransform.localPosition = Vector3.zero;
		this.numSides = numSides;
		vertices = new Vector3[(numSides + 1) * 2];
		int num = numSides + 1;
		sines = new float[numSides];
		cosines = new float[numSides];
		normVerts = new Vector3[numSides];
		int[] array = null;
		int[] array2 = new int[numSides * 3];
		float num2 = 360 / numSides / 2;
		for (int i = 0; i < numSides; i++)
		{
			array2[i * 3] = (i + 1) % numSides;
			array2[i * 3 + 1] = i;
			array2[i * 3 + 2] = numSides;
			float f = ((float)i / (float)numSides * 360f + num2) * ((float)Math.PI / 180f);
			sines[i] = Mathf.Sin(f);
			cosines[i] = Mathf.Cos(f);
			normVerts[i] = new Vector3(cosines[i], 0f, sines[i]);
			vertices[num + i] = normVerts[i] * radius;
		}
		array = array2;
		if (includeSides)
		{
			int[] array3 = new int[numSides * 6];
			for (int j = 0; j < numSides; j++)
			{
				int num3 = j;
				int num4 = num + j;
				int num5 = (j + 1) % numSides;
				int num6 = num + (j + 1) % numSides;
				array3[j * 6] = num3;
				array3[j * 6 + 1] = num5;
				array3[j * 6 + 2] = num4;
				array3[j * 6 + 3] = num4;
				array3[j * 6 + 4] = num5;
				array3[j * 6 + 5] = num6;
			}
			array = array.Concat(array3).ToArray();
		}
		if (includeBottom)
		{
			int[] array4 = new int[numSides * 3];
			for (int k = 0; k < numSides; k++)
			{
				array4[k * 3] = num + k;
				array4[k * 3 + 1] = num + (k + 1) % numSides;
				array4[k * 3 + 2] = num + numSides;
			}
			array = array.Concat(array4).ToArray();
		}
		Vector3[] array5 = new Vector3[vertices.Length];
		for (int l = 0; l < array5.Length; l++)
		{
			array5[l] = Vector3.up;
		}
		mesh.vertices = vertices;
		mesh.SetIndices(array, MeshTopology.Triangles, 0);
		mesh.normals = array5;
		mesh.bounds = new Bounds(new Vector3(0f, height / 2f, 0f), new Vector3(radius * 2f, height, radius * 2f));
	}

	public void SetFluidColor(Color c)
	{
		meshRenderer.material.color = c;
	}

	public void SetParticleEffectsBasedOnTemperature(float temperatureCelsius)
	{
		ParticleSystem.EmissionModule emission = bubblesParticle.emission;
		emission.enabled = temperatureCelsius >= 100f;
		emission = steamParticle.emission;
		emission.enabled = temperatureCelsius >= 65f;
	}

	public void SetParent(ContainerFluidSystem container)
	{
		parentContainer = container;
		if (parentContainer != null)
		{
			radius = parentContainer.ContainerRadius;
			height = parentContainer.ContainerHeight;
			surfaceTransform.localScale = Vector3.one * radius;
		}
	}

	private void Update()
	{
		UpdateEffects();
	}

	public void UpdateMesh(Vector3 worldSurfaceNormal)
	{
		if (parentContainer == null)
		{
			return;
		}
		if (parentContainer.FluidFullPercent <= 0f)
		{
			if (meshRenderer.enabled)
			{
				meshRenderer.enabled = false;
			}
			return;
		}
		if (!meshRenderer.enabled)
		{
			meshRenderer.enabled = true;
		}
		float num = Vector3.Angle(base.transform.up, worldSurfaceNormal);
		if (num >= 90f)
		{
			fluidHitRim = true;
			return;
		}
		float num2 = Mathf.Sin(num * ((float)Math.PI / 180f));
		float num3 = Mathf.Cos(num * ((float)Math.PI / 180f));
		float num4 = num2 / num3;
		Vector3 vector = base.transform.InverseTransformDirection(worldSurfaceNormal);
		float num5 = parentContainer.FluidFullPercent * height;
		float num6 = 0f;
		if (num5 / num4 < radius)
		{
			float num7 = num5 * radius * 2f;
			float num8 = Mathf.Sqrt(2f * num7 / num4);
			num6 = num4 * (num8 - radius);
		}
		else
		{
			num6 = num5;
		}
		float num9 = num6 + radius * num4;
		float num10 = num9 - height;
		if (num10 > 0f)
		{
			num6 -= num10;
			num9 -= num10;
			fluidHitRim = true;
		}
		else
		{
			fluidHitRim = false;
		}
		Plane plane = new Plane(vector, 0f);
		for (int i = 0; i < numSides; i++)
		{
			Vector3 vector2 = normVerts[i] * radius;
			float enter;
			plane.Raycast(new Ray(vector2, Vector3.up), out enter);
			vector2.y = enter + num6;
			vertices[i] = vector2;
		}
		surfaceTransform.localPosition = new Vector3(0f, num5, 0f);
		surfaceTransform.rotation = Quaternion.FromToRotation(Vector3.up, worldSurfaceNormal);
		vertices[numSides] = new Vector3(0f, num6, 0f);
		mesh.vertices = vertices;
		meshRenderer.material.SetVector(fluidSurfaceCenterPropId, vertices[numSides]);
		meshRenderer.material.SetVector(fluidSurfaceNormalPropId, vector);
	}

	private void UpdateEffects()
	{
		if (!(parentContainer != null))
		{
		}
	}
}
