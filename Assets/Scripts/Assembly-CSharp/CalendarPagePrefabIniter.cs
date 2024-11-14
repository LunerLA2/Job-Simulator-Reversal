using UnityEngine;

public class CalendarPagePrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private CalendarPage prefab;

	[SerializeField]
	private Sprite pageSprite;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as CalendarPage).SetupCalendarPage(pageSprite);
	}
}
