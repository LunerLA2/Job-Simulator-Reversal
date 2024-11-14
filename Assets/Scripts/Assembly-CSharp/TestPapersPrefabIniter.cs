using UnityEngine;

public class TestPapersPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private TestPapersController prefab;

	[SerializeField]
	private int graphicIndex;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as TestPapersController).Setup(graphicIndex);
	}
}
