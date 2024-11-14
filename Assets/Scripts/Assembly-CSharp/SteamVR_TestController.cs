using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SteamVR_TestController : MonoBehaviour
{
	private List<int> controllerIndices = new List<int>();

	private EVRButtonId[] buttonIds = new EVRButtonId[4]
	{
		EVRButtonId.k_EButton_ApplicationMenu,
		EVRButtonId.k_EButton_Grip,
		EVRButtonId.k_EButton_Axis0,
		EVRButtonId.k_EButton_Axis1
	};

	private EVRButtonId[] axisIds = new EVRButtonId[2]
	{
		EVRButtonId.k_EButton_Axis0,
		EVRButtonId.k_EButton_Axis1
	};

	public Transform point;

	public Transform pointer;

	private void OnDeviceConnected(params object[] args)
	{
		int num = (int)args[0];
		CVRSystem system = OpenVR.System;
		if (system != null && system.GetTrackedDeviceClass((uint)num) == ETrackedDeviceClass.Controller)
		{
			if ((bool)args[1])
			{
				Debug.Log(string.Format("Controller {0} connected.", num));
				PrintControllerStatus(num);
				controllerIndices.Add(num);
			}
			else
			{
				Debug.Log(string.Format("Controller {0} disconnected.", num));
				PrintControllerStatus(num);
				controllerIndices.Remove(num);
			}
		}
	}

	private void OnEnable()
	{
		SteamVR_Utils.Event.Listen("device_connected", OnDeviceConnected);
	}

	private void OnDisable()
	{
		SteamVR_Utils.Event.Remove("device_connected", OnDeviceConnected);
	}

	private void PrintControllerStatus(int index)
	{
		SteamVR_Controller.Device device = SteamVR_Controller.Input(index);
		Debug.Log("index: " + device.index);
		Debug.Log("connected: " + device.connected);
		Debug.Log("hasTracking: " + device.hasTracking);
		Debug.Log("outOfRange: " + device.outOfRange);
		Debug.Log("calibrating: " + device.calibrating);
		Debug.Log("uninitialized: " + device.uninitialized);
		Debug.Log("pos: " + device.transform.pos);
		Debug.Log("rot: " + device.transform.rot.eulerAngles);
		Debug.Log("velocity: " + device.velocity);
		Debug.Log("angularVelocity: " + device.angularVelocity);
		int deviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
		int deviceIndex2 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
		Debug.Log((deviceIndex == deviceIndex2) ? "first" : ((deviceIndex != index) ? "right" : "left"));
	}

	private void Update()
	{
		foreach (int controllerIndex in controllerIndices)
		{
			SteamVR_Overlay instance = SteamVR_Overlay.instance;
			if ((bool)instance && (bool)point && (bool)pointer)
			{
				SteamVR_Utils.RigidTransform rigidTransform = SteamVR_Controller.Input(controllerIndex).transform;
				pointer.transform.localPosition = rigidTransform.pos;
				pointer.transform.localRotation = rigidTransform.rot;
				SteamVR_Overlay.IntersectionResults results = default(SteamVR_Overlay.IntersectionResults);
				if (instance.ComputeIntersection(rigidTransform.pos, rigidTransform.rot * Vector3.forward, ref results))
				{
					point.transform.localPosition = results.point;
					point.transform.localRotation = Quaternion.LookRotation(results.normal);
				}
				continue;
			}
			EVRButtonId[] array = buttonIds;
			foreach (EVRButtonId eVRButtonId in array)
			{
				if (SteamVR_Controller.Input(controllerIndex).GetPressDown(eVRButtonId))
				{
					Debug.Log(string.Concat(eVRButtonId, " press down"));
				}
				if (SteamVR_Controller.Input(controllerIndex).GetPressUp(eVRButtonId))
				{
					Debug.Log(string.Concat(eVRButtonId, " press up"));
					if (eVRButtonId == EVRButtonId.k_EButton_Axis1)
					{
						SteamVR_Controller.Input(controllerIndex).TriggerHapticPulse();
						PrintControllerStatus(controllerIndex);
					}
				}
				if (SteamVR_Controller.Input(controllerIndex).GetPress(eVRButtonId))
				{
					Debug.Log(eVRButtonId);
				}
			}
			EVRButtonId[] array2 = axisIds;
			foreach (EVRButtonId eVRButtonId2 in array2)
			{
				if (SteamVR_Controller.Input(controllerIndex).GetTouchDown(eVRButtonId2))
				{
					Debug.Log(string.Concat(eVRButtonId2, " touch down"));
				}
				if (SteamVR_Controller.Input(controllerIndex).GetTouchUp(eVRButtonId2))
				{
					Debug.Log(string.Concat(eVRButtonId2, " touch up"));
				}
				if (SteamVR_Controller.Input(controllerIndex).GetTouch(eVRButtonId2))
				{
					Vector2 axis = SteamVR_Controller.Input(controllerIndex).GetAxis(eVRButtonId2);
					Debug.Log("axis: " + axis);
				}
			}
		}
	}
}
