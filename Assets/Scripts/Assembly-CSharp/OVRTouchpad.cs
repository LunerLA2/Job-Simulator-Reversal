using System;
using UnityEngine;

public static class OVRTouchpad
{
	public enum TouchEvent
	{
		SingleTap = 0,
		Left = 1,
		Right = 2,
		Up = 3,
		Down = 4
	}

	public class TouchArgs : EventArgs
	{
		public TouchEvent TouchType;
	}

	private enum TouchState
	{
		Init = 0,
		Down = 1,
		Stationary = 2,
		Move = 3,
		Up = 4
	}

	private static TouchState touchState = TouchState.Init;

	private static Vector2 moveAmount;

	private static float minMovMagnitude = 100f;

	private static Vector3 moveAmountMouse;

	private static float minMovMagnitudeMouse = 25f;

	private static OVRTouchpadHelper touchpadHelper = new GameObject("OVRTouchpadHelper").AddComponent<OVRTouchpadHelper>();

	public static event EventHandler TouchHandler;

	public static void Create()
	{
	}

	public static void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			moveAmountMouse = Input.mousePosition;
			touchState = TouchState.Down;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			moveAmountMouse -= Input.mousePosition;
			HandleInputMouse(ref moveAmountMouse);
			touchState = TouchState.Init;
		}
	}

	public static void OnDisable()
	{
	}

	private static void HandleInput(TouchState state, ref Vector2 move)
	{
		if (move.magnitude < minMovMagnitude || touchState == TouchState.Stationary || touchState != TouchState.Move)
		{
			return;
		}
		move.Normalize();
		if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
		{
			if (!(move.x > 0f))
			{
			}
		}
		else if (!(move.y > 0f))
		{
		}
	}

	private static void HandleInputMouse(ref Vector3 move)
	{
		if (move.magnitude < minMovMagnitudeMouse)
		{
			if (OVRTouchpad.TouchHandler != null)
			{
				OVRTouchpad.TouchHandler(null, new TouchArgs
				{
					TouchType = TouchEvent.SingleTap
				});
			}
			return;
		}
		move.Normalize();
		if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
		{
			if (move.x > 0f)
			{
				if (OVRTouchpad.TouchHandler != null)
				{
					OVRTouchpad.TouchHandler(null, new TouchArgs
					{
						TouchType = TouchEvent.Left
					});
				}
			}
			else if (OVRTouchpad.TouchHandler != null)
			{
				OVRTouchpad.TouchHandler(null, new TouchArgs
				{
					TouchType = TouchEvent.Right
				});
			}
		}
		else if (move.y > 0f)
		{
			if (OVRTouchpad.TouchHandler != null)
			{
				OVRTouchpad.TouchHandler(null, new TouchArgs
				{
					TouchType = TouchEvent.Down
				});
			}
		}
		else if (OVRTouchpad.TouchHandler != null)
		{
			OVRTouchpad.TouchHandler(null, new TouchArgs
			{
				TouchType = TouchEvent.Up
			});
		}
	}
}
