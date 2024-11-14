using UnityEngine;

public class OVRPlatformMenu : MonoBehaviour
{
	public enum eHandler
	{
		ResetCursor = 0,
		ShowGlobalMenu = 1,
		ShowConfirmQuit = 2
	}

	private enum eBackButtonAction
	{
		NONE = 0,
		DOUBLE_TAP = 1,
		SHORT_PRESS = 2,
		LONG_PRESS = 3
	}

	public GameObject cursorTimer;

	public Color cursorTimerColor = new Color(0f, 0.643f, 1f, 1f);

	public float fixedDepth = 3f;

	public KeyCode keyCode = KeyCode.Escape;

	public eHandler doubleTapHandler;

	public eHandler shortPressHandler = eHandler.ShowConfirmQuit;

	public eHandler longPressHandler = eHandler.ShowGlobalMenu;

	private GameObject instantiatedCursorTimer;

	private Material cursorTimerMaterial;

	private float doubleTapDelay = 0.25f;

	private float shortPressDelay = 0.25f;

	private float longPressDelay = 0.75f;

	private int downCount;

	private int upCount;

	private float initialDownTime = -1f;

	private bool waitForUp;

	private eBackButtonAction ResetAndSendAction(eBackButtonAction action)
	{
		MonoBehaviour.print(string.Concat("ResetAndSendAction( ", action, " );"));
		downCount = 0;
		upCount = 0;
		initialDownTime = -1f;
		waitForUp = false;
		ResetCursor();
		if (action == eBackButtonAction.LONG_PRESS)
		{
			waitForUp = true;
		}
		return action;
	}

	private eBackButtonAction HandleBackButtonState()
	{
		if (waitForUp)
		{
			if (Input.GetKeyDown(keyCode) || Input.GetKey(keyCode))
			{
				return eBackButtonAction.NONE;
			}
			waitForUp = false;
		}
		if (Input.GetKeyDown(keyCode))
		{
			downCount++;
			if (downCount == 1)
			{
				initialDownTime = Time.realtimeSinceStartup;
			}
		}
		else if (downCount > 0)
		{
			if (Input.GetKey(keyCode))
			{
				if (downCount <= upCount)
				{
					downCount++;
				}
				float num = Time.realtimeSinceStartup - initialDownTime;
				if (num > shortPressDelay)
				{
					float timerRotateRatio = (num - shortPressDelay) / (longPressDelay - shortPressDelay);
					UpdateCursor(timerRotateRatio);
				}
				if (num > longPressDelay)
				{
					return ResetAndSendAction(eBackButtonAction.LONG_PRESS);
				}
			}
			else if (initialDownTime >= 0f)
			{
				if (upCount < downCount)
				{
					upCount++;
				}
				float num2 = Time.realtimeSinceStartup - initialDownTime;
				if (num2 < doubleTapDelay)
				{
					if (downCount == 2 && upCount == 2)
					{
						return ResetAndSendAction(eBackButtonAction.DOUBLE_TAP);
					}
				}
				else if (num2 > shortPressDelay)
				{
					if (downCount == 1 && upCount == 1)
					{
						return ResetAndSendAction(eBackButtonAction.SHORT_PRESS);
					}
				}
				else if (num2 < longPressDelay)
				{
					return ResetAndSendAction(eBackButtonAction.NONE);
				}
			}
		}
		return eBackButtonAction.NONE;
	}

	private void Awake()
	{
		if (!OVRManager.isHmdPresent)
		{
			base.enabled = false;
		}
		else if (cursorTimer != null && instantiatedCursorTimer == null)
		{
			instantiatedCursorTimer = Object.Instantiate(cursorTimer);
			if (instantiatedCursorTimer != null)
			{
				cursorTimerMaterial = instantiatedCursorTimer.GetComponent<Renderer>().material;
				cursorTimerMaterial.SetColor("_Color", cursorTimerColor);
				instantiatedCursorTimer.GetComponent<Renderer>().enabled = false;
			}
		}
	}

	private void OnDestroy()
	{
		if (cursorTimerMaterial != null)
		{
			Object.Destroy(cursorTimerMaterial);
		}
	}

	private void OnApplicationFocus(bool focusState)
	{
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus)
		{
			Input.ResetInputAxes();
		}
	}

	private void ShowConfirmQuitMenu()
	{
		ResetCursor();
	}

	private void ShowGlobalMenu()
	{
	}

	private void DoHandler(eHandler handler)
	{
		if (handler == eHandler.ResetCursor)
		{
			ResetCursor();
		}
		if (handler == eHandler.ShowConfirmQuit)
		{
			ShowConfirmQuitMenu();
		}
		if (handler == eHandler.ShowGlobalMenu)
		{
			ShowGlobalMenu();
		}
	}

	private void Update()
	{
	}

	private void UpdateCursor(float timerRotateRatio)
	{
		timerRotateRatio = Mathf.Clamp(timerRotateRatio, 0f, 1f);
		if (instantiatedCursorTimer != null)
		{
			instantiatedCursorTimer.GetComponent<Renderer>().enabled = true;
			float value = Mathf.Clamp(1f - timerRotateRatio, 0f, 1f);
			cursorTimerMaterial.SetFloat("_ColorRampOffset", value);
			Vector3 forward = Camera.main.transform.forward;
			Vector3 position = Camera.main.transform.position;
			instantiatedCursorTimer.transform.position = position + forward * fixedDepth;
			instantiatedCursorTimer.transform.forward = forward;
		}
	}

	private void ResetCursor()
	{
		if (instantiatedCursorTimer != null)
		{
			cursorTimerMaterial.SetFloat("_ColorRampOffset", 1f);
			instantiatedCursorTimer.GetComponent<Renderer>().enabled = false;
		}
	}
}
