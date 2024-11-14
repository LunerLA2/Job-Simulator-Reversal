using UnityEngine;

public abstract class SteamVR_Status : MonoBehaviour
{
	public enum Mode
	{
		OnTrue = 0,
		OnFalse = 1,
		WhileTrue = 2,
		WhileFalse = 3
	}

	public string message;

	public float duration;

	public float fade;

	protected float timer;

	protected bool status;

	public Mode mode;

	protected abstract void SetAlpha(float a);

	private void OnEnable()
	{
		SteamVR_Utils.Event.Listen(message, OnEvent);
	}

	private void OnDisable()
	{
		SteamVR_Utils.Event.Remove(message, OnEvent);
	}

	private void OnEvent(params object[] args)
	{
		status = (bool)args[0];
		if (status)
		{
			if (mode == Mode.OnTrue)
			{
				timer = duration;
			}
		}
		else if (mode == Mode.OnFalse)
		{
			timer = duration;
		}
	}

	private void Update()
	{
		if (mode == Mode.OnTrue || mode == Mode.OnFalse)
		{
			timer -= Time.deltaTime;
			if (timer < 0f)
			{
				SetAlpha(0f);
				return;
			}
			float alpha = 1f;
			if (timer < fade)
			{
				alpha = timer / fade;
			}
			if (timer > duration - fade)
			{
				alpha = Mathf.InverseLerp(duration, duration - fade, timer);
			}
			SetAlpha(alpha);
		}
		else
		{
			bool flag = (mode == Mode.WhileTrue && status) || (mode == Mode.WhileFalse && !status);
			timer = ((!flag) ? Mathf.Max(0f, timer - Time.deltaTime) : Mathf.Min(fade, timer + Time.deltaTime));
			SetAlpha(timer / fade);
		}
	}
}
