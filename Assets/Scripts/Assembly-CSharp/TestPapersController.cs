using UnityEngine;

public class TestPapersController : MonoBehaviour
{
	[SerializeField]
	private GameObject[] graphics;

	public void Setup(int graphicIndex)
	{
		for (int i = 0; i < graphics.Length; i++)
		{
			graphics[i].SetActive(i == graphicIndex);
		}
	}
}
