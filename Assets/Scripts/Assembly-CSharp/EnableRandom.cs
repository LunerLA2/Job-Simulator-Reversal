using UnityEngine;

public class EnableRandom : MonoBehaviour
{
	[SerializeField]
	private GameObject[] Objects;

	private void Start()
	{
		Randomize();
	}

	private void Randomize()
	{
		if (Objects != null)
		{
			for (int i = 0; i < Objects.Length; i++)
			{
				Objects[i].SetActive(false);
			}
		}
		int num = Mathf.FloorToInt(Random.Range(0f, (float)Objects.Length - 0.01f));
		Objects[num].SetActive(true);
		base.enabled = false;
	}
}
