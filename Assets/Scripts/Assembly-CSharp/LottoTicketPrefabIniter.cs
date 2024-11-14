using UnityEngine;

public class LottoTicketPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private LottoTicket prefab;

	[SerializeField]
	private Mesh labelMesh;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as LottoTicket).Init(labelMesh);
	}
}
