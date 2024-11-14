using UnityEngine;

public class UnparentKeepWorldPos : MonoBehaviour
{
	private void Awake()
	{
		base.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
	}
}
