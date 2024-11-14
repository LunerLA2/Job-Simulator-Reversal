using OwlchemyVR;
using UnityEngine;

public class CompetencyEvaluationRandomizer : MonoBehaviour
{
	[SerializeField]
	private WorldItemData hiredWID;

	[SerializeField]
	private WorldItemData notHiredWID;

	[SerializeField]
	private Sprite hiredSprite;

	[SerializeField]
	private Sprite notHiredSprite;

	[SerializeField]
	private SpriteRenderer sprite;

	[SerializeField]
	private WorldItem worldItem;

	private void Awake()
	{
		int num = Random.Range(0, 2);
		Debug.Log(num.ToString());
		switch (num)
		{
		case 0:
			sprite.sprite = hiredSprite;
			worldItem.ManualSetData(hiredWID);
			break;
		case 1:
			sprite.sprite = notHiredSprite;
			worldItem.ManualSetData(notHiredWID);
			break;
		}
	}
}
