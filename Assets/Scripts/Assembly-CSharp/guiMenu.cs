using UnityEngine;

public class guiMenu : MonoBehaviour
{
	public GameObject sphere1;

	public GameObject sphere2;

	public GameObject sphere3;

	public GameObject cam;

	public GUIStyle tgStyle;

	private GameObject activeSphere;

	private float guiWidth = 200f;

	private float guiHeight = 150f;

	private string[] tbContent = new string[3] { "Red", "Green", "Blue" };

	private int tgInt;
}
