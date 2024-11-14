using System;
using UnityEngine;

public class IntegratedTwitchCamera : FirstPersonControls
{
	public Camera gameCamera;

	public GameObject billboardQuad;

	public GameObject groundCoverQuad;

	protected Mesh groundCoverRenderMesh;

	private Vector3[] groundQuadVertices = new Vector3[4];

	protected float cameraStateHash;

	protected bool locked;

	private Vector3 twitchCameraPosition = new Vector3(0f, 1f, 0f);

	private Quaternion twitchCameraRotation = Quaternion.identity;

	private float twitchCameraFOV = 100f;

	private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

	private Plane webcamTexturePlane = default(Plane);

	private Vector3 lineVec = default(Vector3);

	private Vector3 linePoint = default(Vector3);

	private Vector3 topLeft = default(Vector3);

	private Vector3 topRight = default(Vector3);

	private Vector3 bottomLeft = default(Vector3);

	private Vector3 bottomRight = default(Vector3);

	private Ray ray = default(Ray);

	private Vector3 downVec = default(Vector3);

	private void Awake()
	{
		twitchCameraPosition = base.transform.position;
		twitchCameraRotation = base.transform.rotation;
	}

	public override void Start()
	{
		base.Start();
		base.transform.position = twitchCameraPosition;
		base.transform.rotation = twitchCameraRotation;
		gameCamera.fieldOfView = twitchCameraFOV;
		cameraStateHash = gameCamera.fieldOfView + base.transform.position.sqrMagnitude + base.transform.rotation.eulerAngles.sqrMagnitude;
		WebCamTexture webCamTexture = new WebCamTexture();
		Renderer component = groundCoverQuad.GetComponent<MeshRenderer>();
		webCamTexture = new WebCamTexture(720, 405, 30);
		webCamTexture.wrapMode = TextureWrapMode.Clamp;
		webCamTexture.mipMapBias = 0f;
		component.sharedMaterial.SetTexture("_MainTex", webCamTexture);
		component.sharedMaterial.SetVector("_MainTex_TexelSize", new Vector4(1f / (float)webCamTexture.width, 1f / (float)webCamTexture.height, webCamTexture.width, webCamTexture.height));
		webCamTexture.Play();
		if (groundCoverRenderMesh == null)
		{
			groundCoverRenderMesh = new Mesh();
			groundCoverQuad.GetComponent<MeshFilter>().sharedMesh = groundCoverRenderMesh;
			groundCoverRenderMesh.vertices = groundQuadVertices;
			groundCoverRenderMesh.triangles = new int[6] { 0, 1, 2, 2, 1, 3 };
		}
	}

	private void UpdateRenderQuad(float percentToDraw, Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight)
	{
		groundQuadVertices[0] = topLeft;
		groundQuadVertices[1] = topRight;
		groundQuadVertices[2] = bottomLeft;
		groundQuadVertices[3] = bottomRight;
		groundCoverRenderMesh.vertices = groundQuadVertices;
		groundCoverRenderMesh.RecalculateBounds();
	}

	protected override void Update()
	{
		if (!locked)
		{
			base.Update();
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			gameCamera.cullingMask ^= 1 << LayerMask.NameToLayer("Hands");
			Debug.Log("Toggle showing controllers in cameras");
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			locked = !locked;
			Debug.Log("Toggle move camera Lock to " + locked);
		}
		float num = gameCamera.fieldOfView + base.transform.position.sqrMagnitude + base.transform.rotation.eulerAngles.sqrMagnitude;
		if (num != cameraStateHash)
		{
			twitchCameraPosition = base.transform.position;
			twitchCameraRotation = base.transform.rotation;
			twitchCameraFOV = gameCamera.fieldOfView;
			cameraStateHash = num;
		}
	}

	private void LateUpdate()
	{
		gameCamera.transform.localRotation = Quaternion.identity;
		float num = Vector3.Distance(GlobalStorage.Instance.MasterHMDAndInputController.Head.transform.position, gameCamera.transform.position);
		billboardQuad.transform.localPosition = new Vector3(0f, 0f, num);
		float num2 = Mathf.Tan((float)Math.PI / 180f * (gameCamera.fieldOfView / 2f)) * num;
		float num3 = num2 * 2f;
		billboardQuad.transform.localScale = new Vector3(num3 * gameCamera.aspect, num3, 1f);
		webcamTexturePlane.SetNormalAndPosition(gameCamera.transform.forward, billboardQuad.transform.position);
		Plane[] array = GeometryUtility.CalculateFrustumPlanes(gameCamera);
		Plane plane = array[0];
		Plane plane2 = array[1];
		Plane plane3 = array[2];
		Math3d.PlanePlaneIntersection(out linePoint, out lineVec, groundPlane.normal, Vector3.zero, webcamTexturePlane.normal, billboardQuad.transform.position);
		ray.direction = lineVec;
		ray.origin = linePoint;
		float enter;
		if (plane2.Raycast(ray, out enter))
		{
			topRight = ray.GetPoint(enter);
		}
		ray.direction *= -1f;
		if (plane.Raycast(ray, out enter))
		{
			topLeft = ray.GetPoint(enter);
		}
		Math3d.PlanePlaneIntersection(out linePoint, out lineVec, groundPlane.normal, Vector3.zero, plane3.normal, gameCamera.transform.position);
		ray.direction = lineVec;
		ray.origin = linePoint;
		if (plane2.Raycast(ray, out enter))
		{
			bottomRight = ray.GetPoint(enter);
		}
		ray.direction *= -1f;
		if (plane.Raycast(ray, out enter))
		{
			bottomLeft = ray.GetPoint(enter);
		}
		topLeft.y += 0.1f;
		topRight.y += 0.1f;
		bottomLeft.y += 0.1f;
		bottomRight.y += 0.1f;
		topLeft = gameCamera.transform.InverseTransformPoint(topLeft);
		topRight = gameCamera.transform.InverseTransformPoint(topRight);
		bottomLeft = gameCamera.transform.InverseTransformPoint(bottomLeft);
		bottomRight = gameCamera.transform.InverseTransformPoint(bottomRight);
		float percentToDraw = 0f;
		downVec.Set(0f, -1f, 0f);
		downVec = billboardQuad.transform.TransformVector(downVec);
		downVec.Normalize();
		ray.direction = downVec;
		ray.origin = billboardQuad.transform.position;
		if (groundPlane.Raycast(ray, out enter))
		{
			percentToDraw = 0.5f - enter / billboardQuad.transform.localScale.y;
		}
		else
		{
			ray.direction *= -1f;
			if (groundPlane.Raycast(ray, out enter))
			{
				percentToDraw = 0.5f + enter / billboardQuad.transform.localScale.y;
			}
		}
		UpdateRenderQuad(percentToDraw, topLeft, topRight, bottomLeft, bottomRight);
	}
}
