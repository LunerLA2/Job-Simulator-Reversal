using PSC;
using UnityEngine;

public class OculusWorldMover : MonoBehaviour
{
	private const float MIN_DISTANCE_FROM_CAMERAS = 0.5f;

	public GameObject debugObj;

	public bool drawDebugBounds;

	private Vector3[] topPoints;

	private Vector3[] bottomPoints;

	private bool hasGuardianSetupBeenVerifiedYet;

	private bool isGuardianSetup;

	public bool IsGuardianSetup
	{
		get
		{
			return isGuardianSetup;
		}
	}

	public bool HasGuardianSetupBeenVerifiedYet
	{
		get
		{
			return hasGuardianSetupBeenVerifiedYet;
		}
	}

	private void Start()
	{
		LayoutConfiguration defaultLayoutToLoad = Room.defaultLayoutToLoad;
		OVRBoundary boundary = OVRManager.boundary;
		if (boundary == null || !boundary.GetConfigured())
		{
			Debug.Log("Skipping world recentering because guardian system isn't set up.");
			SetGuardianStatus(false);
			return;
		}
		topPoints = GetTopGuardianPoints(boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea));
		bottomPoints = GetBottomGuardianPoints(boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea));
		Vector3 centerOfBoundary = GetCenterOfBoundary(boundary);
		Vector3 vector = GlobalStorage.Instance.MasterHMDAndInputController.transform.position - centerOfBoundary;
		vector.y = 0f;
		GlobalStorage.Instance.MasterHMDAndInputController.transform.position += vector;
		SetGuardianStatus(true);
	}

	private void SetGuardianStatus(bool on)
	{
		if (on)
		{
			isGuardianSetup = true;
		}
		else
		{
			isGuardianSetup = false;
		}
		hasGuardianSetupBeenVerifiedYet = true;
		OVRManager.IsRecenteringSupported = !on;
	}

	public Vector3 GetCenterOfBoundary(OVRBoundary boundary)
	{
		Vector3[] geometry = boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
		boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
		if (geometry.Length > 4)
		{
			Debug.LogError("Expected play area boundary to only have four points!");
			return Vector3.zero;
		}
		return new Vector3((topPoints[0].x + topPoints[1].x) / 2f, 0f, (topPoints[0].z + bottomPoints[1].z) / 2f);
	}

	private Vector3[] GetTopGuardianPoints(Vector3[] boundaryPoints)
	{
		Vector3 vector = new Vector3(-9999f, -9999f, -9999f);
		Vector3 vector2 = vector;
		Vector3 vector3 = vector;
		for (int i = 0; i < boundaryPoints.Length; i++)
		{
			if (vector2.z < boundaryPoints[i].z)
			{
				vector3 = vector2;
				vector2 = boundaryPoints[i];
			}
			else if (vector3.z < boundaryPoints[i].z)
			{
				vector3 = boundaryPoints[i];
			}
		}
		return new Vector3[2] { vector2, vector3 };
	}

	private Vector3[] GetBottomGuardianPoints(Vector3[] boundaryPoints)
	{
		Vector3 vector = new Vector3(9999f, 9999f, 9999f);
		Vector3 vector2 = vector;
		Vector3 vector3 = vector;
		for (int i = 0; i < boundaryPoints.Length; i++)
		{
			if (vector2.z > boundaryPoints[i].z)
			{
				vector3 = vector2;
				vector2 = boundaryPoints[i];
			}
			else if (vector3.z > boundaryPoints[i].z)
			{
				vector3 = boundaryPoints[i];
			}
		}
		return new Vector3[2] { vector2, vector3 };
	}

	private float CameraToGuardianPlaySpaceDistance(Vector3 cameraLocation)
	{
		Debug.Log("top point 1: " + topPoints[0].ToStringPrecise() + "\ntop point 2:" + topPoints[1].ToStringPrecise());
		Debug.Log("bottom point 1: " + bottomPoints[0].ToStringPrecise() + "\nbottom point 2:" + bottomPoints[1].ToStringPrecise());
		float x = topPoints[1].x;
		float z = topPoints[1].z;
		float x2 = topPoints[0].x;
		float z2 = topPoints[0].z;
		float x3 = cameraLocation.x;
		float z3 = cameraLocation.z;
		Debug.Log("camera location: " + cameraLocation.ToStringPrecise());
		float num = (z2 - z) / (x2 - x);
		float num2 = -1f;
		float num3 = z + x * ((z2 - z) / (x2 - x));
		float num4 = Mathf.Pow(num2, 2f);
		float num5 = Mathf.Pow(num, 2f);
		float num6 = Mathf.Abs(num * x3 + num2 * z3 + num3);
		float num7 = Mathf.Sqrt(num5 + num4);
		return num6 / num7;
	}
}
