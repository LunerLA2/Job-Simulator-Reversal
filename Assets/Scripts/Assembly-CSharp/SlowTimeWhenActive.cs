using UnityEngine;

public class SlowTimeWhenActive : MonoBehaviour
{
	private float staySlowForRealWorldSeconds = 15f;

	private float realWorldStartTime;

	private void OnEnable()
	{
		realWorldStartTime = Time.realtimeSinceStartup;
		Time.timeScale = 0.15f;
	}

	private void Update()
	{
		if (Time.realtimeSinceStartup > realWorldStartTime + staySlowForRealWorldSeconds * 1f)
		{
			Time.timeScale = 1f;
			base.gameObject.SetActive(false);
		}
	}
}
