using UnityEngine;

public class SteamVR_GazeTracker : MonoBehaviour
{
	public bool isInGaze;

	public float gazeInCutoff = 0.15f;

	public float gazeOutCutoff = 0.4f;

	private Transform hmdTrackedObject;

	public event GazeEventHandler GazeOn;

	public event GazeEventHandler GazeOff;

	private void Start()
	{
	}

	public virtual void OnGazeOn(GazeEventArgs e)
	{
		if (this.GazeOn != null)
		{
			this.GazeOn(this, e);
		}
	}

	public virtual void OnGazeOff(GazeEventArgs e)
	{
		if (this.GazeOff != null)
		{
			this.GazeOff(this, e);
		}
	}

	private void Update()
	{
		if (hmdTrackedObject == null)
		{
			SteamVR_TrackedObject[] array = Object.FindObjectsOfType<SteamVR_TrackedObject>();
			SteamVR_TrackedObject[] array2 = array;
			foreach (SteamVR_TrackedObject steamVR_TrackedObject in array2)
			{
				if (steamVR_TrackedObject.index == SteamVR_TrackedObject.EIndex.Hmd)
				{
					hmdTrackedObject = steamVR_TrackedObject.transform;
					break;
				}
			}
		}
		if (!hmdTrackedObject)
		{
			return;
		}
		Ray ray = new Ray(hmdTrackedObject.position, hmdTrackedObject.forward);
		Plane plane = new Plane(hmdTrackedObject.forward, base.transform.position);
		float enter = 0f;
		if (plane.Raycast(ray, out enter))
		{
			Vector3 a = hmdTrackedObject.position + hmdTrackedObject.forward * enter;
			float num = Vector3.Distance(a, base.transform.position);
			if (num < gazeInCutoff && !isInGaze)
			{
				isInGaze = true;
				GazeEventArgs e = default(GazeEventArgs);
				e.distance = num;
				OnGazeOn(e);
			}
			else if (num >= gazeOutCutoff && isInGaze)
			{
				isInGaze = false;
				GazeEventArgs e2 = default(GazeEventArgs);
				e2.distance = num;
				OnGazeOff(e2);
			}
		}
	}
}
