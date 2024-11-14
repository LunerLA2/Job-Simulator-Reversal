using System;
using UnityEngine;
using Valve.VR;

public class SteamVR_TrackedObject : MonoBehaviour
{
	public enum EIndex
	{
		None = -1,
		Hmd = 0,
		Device1 = 1,
		Device2 = 2,
		Device3 = 3,
		Device4 = 4,
		Device5 = 5,
		Device6 = 6,
		Device7 = 7,
		Device8 = 8,
		Device9 = 9,
		Device10 = 10,
		Device11 = 11,
		Device12 = 12,
		Device13 = 13,
		Device14 = 14,
		Device15 = 15
	}

	public EIndex index;

	public Transform origin;

	public bool isValid;

	private void OnNewPoses(params object[] args)
	{
		if (index == EIndex.None)
		{
			return;
		}
		int num = (int)index;
		isValid = false;
		TrackedDevicePose_t[] array = (TrackedDevicePose_t[])args[0];
		if (array.Length > num && array[num].bDeviceIsConnected && array[num].bPoseIsValid)
		{
			isValid = true;
			SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(array[num].mDeviceToAbsoluteTracking);
			if (origin != null)
			{
				rigidTransform = new SteamVR_Utils.RigidTransform(origin) * rigidTransform;
				rigidTransform.pos.x *= origin.localScale.x;
				rigidTransform.pos.y *= origin.localScale.y;
				rigidTransform.pos.z *= origin.localScale.z;
				base.transform.position = rigidTransform.pos;
				base.transform.rotation = rigidTransform.rot;
			}
			else
			{
				base.transform.localPosition = rigidTransform.pos;
				base.transform.localRotation = rigidTransform.rot;
			}
		}
	}

	private void OnEnable()
	{
		SteamVR_Render instance = SteamVR_Render.instance;
		if (instance == null)
		{
			base.enabled = false;
		}
		else
		{
			SteamVR_Utils.Event.Listen("new_poses", OnNewPoses);
		}
	}

	private void OnDisable()
	{
		SteamVR_Utils.Event.Remove("new_poses", OnNewPoses);
	}

	public void SetDeviceIndex(int index)
	{
		if (Enum.IsDefined(typeof(EIndex), index))
		{
			this.index = (EIndex)index;
		}
	}
}
