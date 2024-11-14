using TMPro;
using UnityEngine;

public class RandomResmueGenerator : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro text;

	[SerializeField]
	private TextMeshPro text2;

	private int botId;

	[SerializeField]
	private string[] workHistory;

	[SerializeField]
	private string[] education;

	[SerializeField]
	private string[] hobbies;

	[SerializeField]
	private string[] skills;

	private void OnEnable()
	{
		GenerateText();
	}

	private void GenerateText()
	{
		string text = string.Format("{0} \n \n \n {1}  \n \n \n {2}  \n \n \n {3}", GetRandomResults(workHistory), GetRandomResults(education), GetRandomResults(skills), GetRandomResults(hobbies));
		this.text.text = text;
		string text2 = string.Format(" Bot # {0} ", Random.Range(0, 10000).ToString("D5"));
		this.text2.text = text2;
	}

	private string GetRandomResults(string[] category)
	{
		int num = Random.Range(0, category.Length);
		return category[num];
	}
}
