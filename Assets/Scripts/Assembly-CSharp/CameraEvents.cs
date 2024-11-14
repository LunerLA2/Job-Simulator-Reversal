using System;
using UnityEngine;

public class CameraEvents : MonoBehaviour
{
	public Action<CameraEvents> OnPreRenderEvent;

	public Action<CameraEvents> OnPostRenderEvent;

	public Action<CameraEvents> OnPreCullEvent;

	private void OnPreCull()
	{
		if (OnPreCullEvent != null)
		{
			OnPreCullEvent(this);
		}
	}

	private void OnPreRender()
	{
		if (OnPreRenderEvent != null)
		{
			OnPreRenderEvent(this);
		}
	}

	private void OnPostRender()
	{
		if (OnPostRenderEvent != null)
		{
			OnPostRenderEvent(this);
		}
	}
}
