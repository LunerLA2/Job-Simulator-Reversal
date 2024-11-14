using System.Collections.Generic;
using UnityEngine;

public class StaggeredCameraRenderManager : MonoBehaviour
{
	private List<StaggeredCameraState> states = new List<StaggeredCameraState>();

	private static StaggeredCameraRenderManager _instance;

	public static StaggeredCameraRenderManager _instanceNoCreate
	{
		get
		{
			return _instance;
		}
	}

	public static StaggeredCameraRenderManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(StaggeredCameraRenderManager)) as StaggeredCameraRenderManager;
				if (_instance == null)
				{
					_instance = new GameObject("_StaggeredCameraRenderManager").AddComponent<StaggeredCameraRenderManager>();
				}
			}
			return _instance;
		}
	}

	public void RegisterCamera(Camera cam, bool renderOnOdd)
	{
		cam.enabled = false;
		StaggeredCameraState staggeredCameraState = new StaggeredCameraState();
		staggeredCameraState.cam = cam;
		staggeredCameraState.renderOnOdd = renderOnOdd;
		staggeredCameraState.cam.Render();
		states.Add(staggeredCameraState);
	}

	public void UnregisterCamera(Camera cam)
	{
		StaggeredCameraState stateForCamera = GetStateForCamera(cam);
		if (stateForCamera != null && states.Contains(stateForCamera))
		{
			states.Remove(stateForCamera);
		}
	}

	private StaggeredCameraState GetStateForCamera(Camera cam)
	{
		for (int i = 0; i < states.Count; i++)
		{
			if (states[i].cam == cam)
			{
				return states[i];
			}
		}
		return null;
	}

	private void OnLevelWasLoaded(int l)
	{
		states.Clear();
	}

	private void LateUpdate()
	{
		bool flag = Time.frameCount % 2 > 0;
		for (int i = 0; i < states.Count; i++)
		{
			if (flag && states[i].renderOnOdd)
			{
				if (states[i].cam.gameObject.activeInHierarchy)
				{
					states[i].cam.Render();
				}
			}
			else if (!flag && !states[i].renderOnOdd)
			{
				if (states[i].cam.gameObject.activeInHierarchy)
				{
					states[i].cam.Render();
				}
			}
			else if (states[i].cam.gameObject.activeInHierarchy != states[i].prevActiveState)
			{
				states[i].prevActiveState = states[i].cam.gameObject.activeInHierarchy;
				if (states[i].prevActiveState)
				{
					states[i].cam.Render();
				}
			}
		}
	}
}
