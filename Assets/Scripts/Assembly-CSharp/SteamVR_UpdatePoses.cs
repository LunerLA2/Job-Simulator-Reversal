using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(Camera))]
public class SteamVR_UpdatePoses : MonoBehaviour
{
	private void Awake()
	{
		Camera component = GetComponent<Camera>();
		component.clearFlags = CameraClearFlags.Nothing;
		component.useOcclusionCulling = false;
		component.cullingMask = 0;
		component.depth = -9999f;
		component.eventMask = 0;
		GUILayer gUILayer = component.gameObject.AddComponent<GUILayer>();
		gUILayer.enabled = false;
	}

	private void OnPreCull()
	{
		CVRCompositor compositor = OpenVR.Compositor;
		if (compositor != null)
		{
			SteamVR_Render instance = SteamVR_Render.instance;
			compositor.GetLastPoses(instance.poses, instance.gamePoses);
			SteamVR_Utils.Event.Send("new_poses", instance.poses);
			SteamVR_Utils.Event.Send("new_poses_applied");
		}
	}
}
