using UnityEngine;

public class NumberDial : MonoBehaviour
{
	private int currentNumber;

	private Quaternion targetRotation;

	private float rotationSpeed = 0.1f;

	private bool isRotating;

	private void Start()
	{
		targetRotation = base.transform.rotation;
		currentNumber = 0;
	}

	private void Update()
	{
		if (base.transform.rotation != targetRotation)
		{
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, targetRotation, rotationSpeed);
		}
		else if (isRotating)
		{
			isRotating = false;
		}
	}

	public void IncreaseNumber()
	{
		if (!isRotating)
		{
			isRotating = true;
			currentNumber++;
			if (currentNumber > 9)
			{
				currentNumber = 0;
			}
			base.transform.Rotate(-36f, 0f, 0f);
			targetRotation = base.transform.rotation;
			base.transform.Rotate(36f, 0f, 0f);
		}
	}

	public void DecreaseNumber()
	{
		if (!isRotating)
		{
			isRotating = true;
			currentNumber--;
			if (currentNumber < 0)
			{
				currentNumber = 9;
			}
			base.transform.Rotate(36f, 0f, 0f);
			targetRotation = base.transform.rotation;
			base.transform.Rotate(-36f, 0f, 0f);
		}
	}

	public int GetCurrentNumber()
	{
		return currentNumber;
	}
}
