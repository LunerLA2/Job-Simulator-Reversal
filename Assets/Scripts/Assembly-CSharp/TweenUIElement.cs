using UnityEngine;

public class TweenUIElement : MonoBehaviour
{
	[SerializeField]
	private Vector3 bounceMagnitude;

	[SerializeField]
	private GoEaseType easeType;

	[SerializeField]
	private float time;

	[SerializeField]
	private bool tweenColor;

	[SerializeField]
	private MonoBehaviour imageComponent;

	[SerializeField]
	private Color toColor;

	private void Start()
	{
		Go.to(base.gameObject.transform, time, new GoTweenConfig().localPosition(bounceMagnitude, true).setIterations(-1, GoLoopType.PingPong).setEaseType(easeType));
		if (tweenColor)
		{
			Go.to(imageComponent, time, new GoTweenConfig().colorProp("color", toColor).setIterations(-1, GoLoopType.PingPong));
		}
	}
}
