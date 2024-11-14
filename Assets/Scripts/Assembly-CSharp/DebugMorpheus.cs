using UnityEngine;

public class DebugMorpheus : MonoBehaviour
{
	public static DebugMorpheus singleton;

	public TextMesh text;

	private Transform head;

	private void Awake()
	{
		if (singleton == null)
		{
			singleton = this;
		}
	}

	private void Start()
	{
		head = GlobalStorage.Instance.MasterHMDAndInputController.Head.transform;
	}

	private void Update()
	{
		base.transform.position = head.transform.position;
		base.transform.rotation = head.transform.rotation;
	}

	public void Print(string content)
	{
		text.text = content;
	}
}
