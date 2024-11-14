using UnityEngine;

public class BotCostumePieceGroup : ScriptableObject
{
	[SerializeField]
	private CostumePiece[] costumePiecePrefabs;

	public CostumePiece[] CostumePiecePrefabs
	{
		get
		{
			return costumePiecePrefabs;
		}
	}
}
