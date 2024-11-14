using UnityEngine;

[RequireComponent(typeof(GUIText))]
public class SteamVR_StatusText : SteamVR_Status
{
	private GUIText text;

	private void Awake()
	{
		text = GetComponent<GUIText>();
		if (mode == Mode.WhileTrue || mode == Mode.WhileFalse)
		{
			timer = fade * text.color.a;
		}
	}

	protected override void SetAlpha(float a)
	{
		if (a > 0f)
		{
			text.enabled = true;
			text.color = new Color(text.color.r, text.color.g, text.color.b, a);
		}
		else
		{
			text.enabled = false;
		}
	}
}
