using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
	[SerializeField]
	private Text text;

	private int counter;

	private void Awake()
	{
		text.text = counter.ToString();
	}

	public void AddOne()
	{
		counter++;
		text.text = counter.ToString();
	}
}
