using UnityEngine;

public class CarDriveByController : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer[] meshToChangeColorOf;

	[SerializeField]
	private AudioClip[] driveBySounds;

	private int lastRandomNumberGenerated;

	public void ChangeColor()
	{
		Color color = Random.ColorHSV(0f, 1f, 0.5f, 0.75f, 1f, 1f);
		for (int i = 0; i < meshToChangeColorOf.Length; i++)
		{
			meshToChangeColorOf[i].material.SetColor("_DiffColor", color);
		}
	}

	public void DriveBySound()
	{
		int num;
		do
		{
			num = Random.Range(0, driveBySounds.Length);
		}
		while (lastRandomNumberGenerated == num);
		AudioManager.Instance.Play(base.transform.position, driveBySounds[num], 1f, 1f);
		lastRandomNumberGenerated = num;
	}
}
