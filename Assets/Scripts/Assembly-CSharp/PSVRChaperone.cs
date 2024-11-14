using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class PSVRChaperone : MonoBehaviour
{
	[Serializable]
	public struct PSVRBound
	{
		[SerializeField]
		private MeshRenderer boundRenderer;

		private Transform boundTransform;

		private Material boundMaterial;

		[SerializeField]
		private bool isLeftOrRightBound;

		private float trackedObj_Distance;

		private float cloestObj_Distance;

		private Ray chaperonOriginToBoundCenter;

		public void RenderBasedOnDistance(Transform[] trackedObjs, Vector3 chaperonPos)
		{
			if (boundTransform == null)
			{
				boundTransform = boundRenderer.transform;
			}
			if (boundMaterial == null)
			{
				boundMaterial = boundRenderer.material;
			}
			cloestObj_Distance = 10000f;
			chaperonOriginToBoundCenter.origin = chaperonPos;
			chaperonOriginToBoundCenter.direction = boundTransform.position - chaperonPos;
			for (int i = 0; i < trackedObjs.Length; i++)
			{
				trackedObj_Distance = Vector3.Cross(chaperonOriginToBoundCenter.direction, trackedObjs[i].position - chaperonOriginToBoundCenter.origin).magnitude;
				if (trackedObj_Distance < cloestObj_Distance)
				{
					cloestObj_Distance = trackedObj_Distance;
				}
			}
			if (cloestObj_Distance < minDistanceForBoundsToShow)
			{
				float t = cloestObj_Distance / minDistanceForBoundsToShow;
				boundMaterial.color = Color.Lerp(solid, clear, t);
			}
			else if (boundMaterial.color != Color.clear)
			{
				boundMaterial.color = Color.clear;
			}
		}

		public void SetBoundColor(Color targetColor)
		{
			if (boundMaterial == null)
			{
				boundMaterial = boundRenderer.material;
			}
			boundMaterial.color = targetColor;
		}
	}

	public static Color solid = new Color(0f, 1f, 1f, 0.75f);

	public static Color clear = new Color(0f, 1f, 1f, 0f);

	public static float minDistanceForBoundsToShow = 0.3f;

	private Coroutine pulseRoutine;

	private float pulseRoutineDuration = 3f;

	[SerializeField]
	private float captureTime = 1f;

	private float rotDataCaptureTimer;

	private Vector3 cameraAcceleration;

	private Vector3 targetCameraRot;

	private Morpheus_IndividualController move1Input;

	private Morpheus_IndividualController move2Input;

	[SerializeField]
	private Transform frustumVisualizer;

	[SerializeField]
	private PSVRBound[] bounds;

	private Transform[] trackedObjects;

	private IEnumerator Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		for (int i = 0; i < bounds.Length; i++)
		{
			bounds[i].SetBoundColor(Color.clear);
		}
		while (!GlobalStorage.Instance.MasterHMDAndInputController.IsHMDAndInputReady)
		{
			yield return null;
		}
		trackedObjects = new Transform[2]
		{
			GlobalStorage.Instance.MasterHMDAndInputController.LeftHand.transform,
			GlobalStorage.Instance.MasterHMDAndInputController.RightHand.transform
		};
		base.transform.parent = GlobalStorage.Instance.MasterHMDAndInputController.transform;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
		frustumVisualizer.localScale = new Vector3(1.05f, 0.7f, 1f);
		frustumVisualizer.localEulerAngles = new Vector3(1f, 0f, 0f);
		move1Input = GlobalStorage.Instance.MasterHMDAndInputController.LeftHand.GetComponent<Morpheus_IndividualController>();
		move2Input = GlobalStorage.Instance.MasterHMDAndInputController.RightHand.GetComponent<Morpheus_IndividualController>();
	}

	private void Update()
	{
		rotDataCaptureTimer -= Time.deltaTime;
		if (rotDataCaptureTimer <= 0f)
		{
		}
		if (pulseRoutine == null && trackedObjects != null)
		{
			for (int i = 0; i < bounds.Length; i++)
			{
				bounds[i].RenderBasedOnDistance(trackedObjects, base.transform.position);
			}
		}
		if (!(move1Input == null) && !(move2Input == null) && (move1Input.GetButtonDown(Morpheus_IndividualController.MoveControllerButton.Start) || move2Input.GetButtonDown(Morpheus_IndividualController.MoveControllerButton.Start)))
		{
			StartPulse();
		}
	}

	public void StartPulse()
	{
		if (pulseRoutine != null)
		{
			StopCoroutine(pulseRoutine);
		}
		pulseRoutine = StartCoroutine(PulseRoutine());
	}

	private IEnumerator PulseRoutine()
	{
		float startTime = Time.time;
		float t;
		do
		{
			t = (Time.time - startTime) / pulseRoutineDuration;
			Debug.Log(t);
			for (int i = 0; i < bounds.Length; i++)
			{
				bounds[i].SetBoundColor(Color.Lerp(solid, clear, t));
			}
			yield return null;
		}
		while (!(t >= 1f));
		pulseRoutine = null;
	}

	private void DebugTools()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			StartPulse();
		}
		if (move1Input == null || !move1Input.IsControllerReady)
		{
			return;
		}
		if (move1Input.GetButtonDown(Morpheus_IndividualController.MoveControllerButton.Circle))
		{
			frustumVisualizer.localScale = new Vector3(frustumVisualizer.localScale.x - 0.05f, frustumVisualizer.localScale.y, frustumVisualizer.localScale.z);
		}
		else if (move1Input.GetButtonDown(Morpheus_IndividualController.MoveControllerButton.Triangle))
		{
			frustumVisualizer.localScale = new Vector3(frustumVisualizer.localScale.x + 0.05f, frustumVisualizer.localScale.y, frustumVisualizer.localScale.z);
		}
		if (move1Input.GetButtonDown(Morpheus_IndividualController.MoveControllerButton.Square))
		{
			frustumVisualizer.localScale = new Vector3(frustumVisualizer.localScale.x, frustumVisualizer.localScale.y + 0.05f, frustumVisualizer.localScale.z);
		}
		else if (move1Input.GetButtonDown(Morpheus_IndividualController.MoveControllerButton.Cross))
		{
			frustumVisualizer.localScale = new Vector3(frustumVisualizer.localScale.x, frustumVisualizer.localScale.y - 0.05f, frustumVisualizer.localScale.z);
		}
		if (move1Input.GetButtonDown(Morpheus_IndividualController.MoveControllerButton.Center))
		{
			if (move1Input.GetButton(Morpheus_IndividualController.MoveControllerButton.Trigger))
			{
				frustumVisualizer.localEulerAngles = new Vector3(frustumVisualizer.localEulerAngles.x - 1f, frustumVisualizer.localEulerAngles.y, frustumVisualizer.localEulerAngles.z);
			}
			else
			{
				frustumVisualizer.localEulerAngles = new Vector3(frustumVisualizer.localEulerAngles.x + 1f, frustumVisualizer.localEulerAngles.y, frustumVisualizer.localEulerAngles.z);
			}
		}
	}
}
