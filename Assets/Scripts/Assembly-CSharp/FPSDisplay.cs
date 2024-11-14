using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
	public TextMesh fpsText;

	private float deltaTime;

	private string tempString = string.Empty;

	public bool activeRBMode;

	private int frameCounter;

	private void Update()
	{
		if (!activeRBMode)
		{
			deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
			fpsText.text = string.Format("{0:0.0} ms | {1:0.} fps", deltaTime * 1000f, 1f / deltaTime);
			return;
		}
		frameCounter++;
		if (frameCounter <= 4)
		{
			return;
		}
		tempString = string.Empty;
		Rigidbody[] array = Object.FindObjectsOfType<Rigidbody>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].IsSleeping())
			{
				tempString = tempString + array[i].name + "\n";
			}
		}
		fpsText.text = tempString;
		frameCounter = -1;
	}
}
