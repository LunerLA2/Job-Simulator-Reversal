using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class OvertimeTaskShuffleData : ScriptableObject
{
	[SerializeField]
	public int NumCardsInDeck { get; set; }
}
