using TMPro;
using UnityEngine;

public class MorpheusCalibrationText : MonoBehaviour
{
	public TextMeshPro tmp;

	public void DebugText(string inText)
	{
		tmp.text = inText;
	}
}
