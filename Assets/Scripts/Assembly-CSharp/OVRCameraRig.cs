using System;
using UnityEngine;
using UnityEngine.VR;

[ExecuteInEditMode]
public class OVRCameraRig : MonoBehaviour
{
	public bool usePerEyeCameras;

	private readonly string trackingSpaceName = "TrackingSpace";

	private readonly string trackerAnchorName = "TrackerAnchor";

	private readonly string eyeAnchorName = "EyeAnchor";

	private readonly string handAnchorName = "HandAnchor";

	private readonly string legacyEyeAnchorName = "Camera";

	private Camera _centerEyeCamera;

	private Camera _leftEyeCamera;

	private Camera _rightEyeCamera;

	public Camera leftEyeCamera
	{
		get
		{
			return (!usePerEyeCameras) ? _centerEyeCamera : _leftEyeCamera;
		}
	}

	public Camera rightEyeCamera
	{
		get
		{
			return (!usePerEyeCameras) ? _centerEyeCamera : _rightEyeCamera;
		}
	}

	public Transform trackingSpace { get; private set; }

	public Transform leftEyeAnchor { get; private set; }

	public Transform centerEyeAnchor { get; private set; }

	public Transform rightEyeAnchor { get; private set; }

	public Transform leftHandAnchor { get; private set; }

	public Transform rightHandAnchor { get; private set; }

	public Transform trackerAnchor { get; private set; }

	public event Action<OVRCameraRig> UpdatedAnchors;

	private void Awake()
	{
		EnsureGameObjectIntegrity();
	}

	private void Start()
	{
		EnsureGameObjectIntegrity();
		if (Application.isPlaying)
		{
			UpdateAnchors();
		}
	}

	private void Update()
	{
		EnsureGameObjectIntegrity();
		if (Application.isPlaying)
		{
			UpdateAnchors();
		}
	}

	private void UpdateAnchors()
	{
		bool monoscopic = OVRManager.instance.monoscopic;
		OVRPose pose = OVRManager.tracker.GetPose();
		centerEyeAnchor.localRotation = UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.CenterEye);
		leftEyeAnchor.localRotation = ((!monoscopic) ? UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.LeftEye) : centerEyeAnchor.localRotation);
		rightEyeAnchor.localRotation = ((!monoscopic) ? UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.RightEye) : centerEyeAnchor.localRotation);
		leftHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
		rightHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
		centerEyeAnchor.localPosition = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.CenterEye);
		leftEyeAnchor.localPosition = ((!monoscopic) ? UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftEye) : centerEyeAnchor.localPosition);
		rightEyeAnchor.localPosition = ((!monoscopic) ? UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.RightEye) : centerEyeAnchor.localPosition);
		leftHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
		rightHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
		if (this.UpdatedAnchors != null)
		{
			this.UpdatedAnchors(this);
		}
	}

	public void EnsureGameObjectIntegrity()
	{
		if (trackingSpace == null)
		{
			trackingSpace = ConfigureRootAnchor(trackingSpaceName);
		}
		if (leftEyeAnchor == null)
		{
			leftEyeAnchor = ConfigureEyeAnchor(trackingSpace, UnityEngine.XR.XRNode.LeftEye);
		}
		if (centerEyeAnchor == null)
		{
			centerEyeAnchor = ConfigureEyeAnchor(trackingSpace, UnityEngine.XR.XRNode.CenterEye);
		}
		if (rightEyeAnchor == null)
		{
			rightEyeAnchor = ConfigureEyeAnchor(trackingSpace, UnityEngine.XR.XRNode.RightEye);
		}
		if (leftHandAnchor == null)
		{
			leftHandAnchor = ConfigureHandAnchor(trackingSpace, OVRPlugin.Node.HandLeft);
		}
		if (rightHandAnchor == null)
		{
			rightHandAnchor = ConfigureHandAnchor(trackingSpace, OVRPlugin.Node.HandRight);
		}
		if (trackerAnchor == null)
		{
			trackerAnchor = ConfigureTrackerAnchor(trackingSpace);
		}
		if (_centerEyeCamera == null || _leftEyeCamera == null || _rightEyeCamera == null)
		{
			_centerEyeCamera = centerEyeAnchor.GetComponent<Camera>();
			_leftEyeCamera = leftEyeAnchor.GetComponent<Camera>();
			_rightEyeCamera = rightEyeAnchor.GetComponent<Camera>();
			if (_centerEyeCamera == null)
			{
				_centerEyeCamera = centerEyeAnchor.gameObject.AddComponent<Camera>();
				_centerEyeCamera.tag = "MainCamera";
			}
			if (_leftEyeCamera == null)
			{
				_leftEyeCamera = leftEyeAnchor.gameObject.AddComponent<Camera>();
				_leftEyeCamera.tag = "MainCamera";
			}
			if (_rightEyeCamera == null)
			{
				_rightEyeCamera = rightEyeAnchor.gameObject.AddComponent<Camera>();
				_rightEyeCamera.tag = "MainCamera";
			}
			_centerEyeCamera.stereoTargetEye = StereoTargetEyeMask.Both;
			_leftEyeCamera.stereoTargetEye = StereoTargetEyeMask.Left;
			_rightEyeCamera.stereoTargetEye = StereoTargetEyeMask.Right;
		}
		_centerEyeCamera.enabled = !usePerEyeCameras;
		_leftEyeCamera.enabled = usePerEyeCameras;
		_rightEyeCamera.enabled = usePerEyeCameras;
	}

	private Transform ConfigureRootAnchor(string name)
	{
		Transform transform = base.transform.Find(name);
		if (transform == null)
		{
			transform = new GameObject(name).transform;
		}
		transform.parent = base.transform;
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return transform;
	}

	private Transform ConfigureEyeAnchor(Transform root, UnityEngine.XR.XRNode eye)
	{
		object obj;
		switch (eye)
		{
		case UnityEngine.XR.XRNode.CenterEye:
			obj = "Center";
			break;
		case UnityEngine.XR.XRNode.LeftEye:
			obj = "Left";
			break;
		default:
			obj = "Right";
			break;
		}
		string text = (string)obj;
		string text2 = text + eyeAnchorName;
		Transform transform = base.transform.Find(root.name + "/" + text2);
		if (transform == null)
		{
			transform = base.transform.Find(text2);
		}
		if (transform == null)
		{
			string text3 = legacyEyeAnchorName + eye;
			transform = base.transform.Find(text3);
		}
		if (transform == null)
		{
			transform = new GameObject(text2).transform;
		}
		transform.name = text2;
		transform.parent = root;
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return transform;
	}

	private Transform ConfigureHandAnchor(Transform root, OVRPlugin.Node hand)
	{
		string text = ((hand != OVRPlugin.Node.HandLeft) ? "Right" : "Left");
		string text2 = text + handAnchorName;
		Transform transform = base.transform.Find(root.name + "/" + text2);
		if (transform == null)
		{
			transform = base.transform.Find(text2);
		}
		if (transform == null)
		{
			transform = new GameObject(text2).transform;
		}
		transform.name = text2;
		transform.parent = root;
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return transform;
	}

	private Transform ConfigureTrackerAnchor(Transform root)
	{
		string text = trackerAnchorName;
		Transform transform = base.transform.Find(root.name + "/" + text);
		if (transform == null)
		{
			transform = new GameObject(text).transform;
		}
		transform.parent = root;
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return transform;
	}
}
