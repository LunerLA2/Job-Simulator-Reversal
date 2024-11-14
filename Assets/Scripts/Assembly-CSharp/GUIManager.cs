using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
	public Text guiTextMode;

	public Slider sizeSlider;

	public TexturePainter painter;

	public void SetBrushMode(int newMode)
	{
		Painter_BrushMode painter_BrushMode = ((newMode == 0) ? Painter_BrushMode.DECAL : Painter_BrushMode.PAINT);
		string text = ((painter_BrushMode != 0) ? "purple" : "orange");
		guiTextMode.text = "<b>Mode:</b><color=" + text + ">" + painter_BrushMode.ToString() + "</color>";
		painter.SetBrushMode(painter_BrushMode);
	}

	public void UpdateSizeSlider()
	{
		painter.SetBrushSize(sizeSlider.value);
	}
}
