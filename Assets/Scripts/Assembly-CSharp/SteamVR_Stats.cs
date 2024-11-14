using System.Runtime.InteropServices;
using UnityEngine;
using Valve.VR;

public class SteamVR_Stats : MonoBehaviour
{
	public GUIText text;

	public Color fadeColor = Color.black;

	public float fadeDuration = 1f;

	private double lastUpdate;

	private void Awake()
	{
		if (text == null)
		{
			text = GetComponent<GUIText>();
			text.enabled = false;
		}
		if (fadeDuration > 0f)
		{
			SteamVR_Fade.Start(fadeColor, 0f);
			SteamVR_Fade.Start(Color.clear, fadeDuration);
		}
	}

	private void Update()
	{
		if (!(text != null))
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.I))
		{
			text.enabled = !text.enabled;
		}
		if (!text.enabled)
		{
			return;
		}
		CVRCompositor compositor = OpenVR.Compositor;
		if (compositor != null)
		{
			Compositor_FrameTiming pTiming = default(Compositor_FrameTiming);
			pTiming.m_nSize = (uint)Marshal.SizeOf(typeof(Compositor_FrameTiming));
			compositor.GetFrameTiming(ref pTiming, 0u);
			double flSystemTimeInSeconds = pTiming.m_flSystemTimeInSeconds;
			if (flSystemTimeInSeconds > lastUpdate)
			{
				double num = ((!(lastUpdate > 0.0)) ? 0.0 : (1.0 / (flSystemTimeInSeconds - lastUpdate)));
				lastUpdate = flSystemTimeInSeconds;
				text.text = string.Format("framerate: {0:N0}\ndropped frames: {1}", num, (int)pTiming.m_nNumDroppedFrames);
			}
			else
			{
				lastUpdate = flSystemTimeInSeconds;
			}
		}
	}
}
