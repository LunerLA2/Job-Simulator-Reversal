using UnityEngine;
using Valve.VR;

public class SteamVR_Fade : MonoBehaviour
{
	private Color currentColor = new Color(0f, 0f, 0f, 0f);

	private Color targetColor = new Color(0f, 0f, 0f, 0f);

	private Color deltaColor = new Color(0f, 0f, 0f, 0f);

	private bool fadeOverlay;

	private static Material fadeMaterial;

	public static void Start(Color newColor, float duration, bool fadeOverlay = false)
	{
		SteamVR_Utils.Event.Send("fade", newColor, duration, fadeOverlay);
	}

	public static void View(Color newColor, float duration)
	{
		CVRCompositor compositor = OpenVR.Compositor;
		if (compositor != null)
		{
			compositor.FadeToColor(duration, newColor.r, newColor.g, newColor.b, newColor.a, false);
		}
	}

	public void OnStartFade(params object[] args)
	{
		Color color = (Color)args[0];
		float num = (float)args[1];
		fadeOverlay = args.Length > 2 && (bool)args[2];
		if (num > 0f)
		{
			targetColor = color;
			deltaColor = (targetColor - currentColor) / num;
		}
		else
		{
			currentColor = color;
		}
	}

	private void OnEnable()
	{
		if (fadeMaterial == null)
		{
			fadeMaterial = new Material(Shader.Find("Custom/SteamVR_Fade"));
		}
		SteamVR_Utils.Event.Listen("fade", OnStartFade);
		SteamVR_Utils.Event.Send("fade_ready");
	}

	private void OnDisable()
	{
		SteamVR_Utils.Event.Remove("fade", OnStartFade);
	}

	private void OnPostRender()
	{
		if (currentColor != targetColor)
		{
			if (Mathf.Abs(currentColor.a - targetColor.a) < Mathf.Abs(deltaColor.a) * Time.deltaTime)
			{
				currentColor = targetColor;
				deltaColor = new Color(0f, 0f, 0f, 0f);
			}
			else
			{
				currentColor += deltaColor * Time.deltaTime;
			}
			if (fadeOverlay)
			{
				SteamVR_Overlay instance = SteamVR_Overlay.instance;
				if (instance != null)
				{
					instance.alpha = 1f - currentColor.a;
				}
			}
		}
		if (currentColor.a > 0f && (bool)fadeMaterial)
		{
			GL.PushMatrix();
			GL.LoadOrtho();
			fadeMaterial.SetPass(0);
			GL.Begin(7);
			GL.Color(currentColor);
			GL.Vertex3(0f, 0f, 0f);
			GL.Vertex3(1f, 0f, 0f);
			GL.Vertex3(1f, 1f, 0f);
			GL.Vertex3(0f, 1f, 0f);
			GL.End();
			GL.PopMatrix();
		}
	}
}
