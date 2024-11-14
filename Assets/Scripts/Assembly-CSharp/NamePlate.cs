using TMPro;
using UnityEngine;

public class NamePlate : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer mainMeshRenderer;

	[SerializeField]
	private GameObject[] titles;

	[SerializeField]
	private TextMeshPro customizableTMP;

	public void Setup(int titleIndex, Material mat)
	{
		for (int i = 0; i < titles.Length; i++)
		{
			titles[i].SetActive(i == titleIndex);
		}
		mainMeshRenderer.material = mat;
		if (JobBoardManager.instance != null && JobBoardManager.instance.EndlessModeStatusController != null)
		{
			customizableTMP.text = PromotionRankNameGenerator.GetRankName(GlobalStorage.Instance.GameStateData.NumberOfCompletedEndlessTasks());
		}
	}
}
