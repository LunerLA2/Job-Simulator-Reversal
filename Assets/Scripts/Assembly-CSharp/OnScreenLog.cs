using System;
using System.Collections.Generic;
using UnityEngine;

public class OnScreenLog : MonoBehaviour
{
	private static int msgCount = 0;

	private static List<string> log = new List<string>();

	private static int maxLines = 16;

	private static int fontSize = 24;

	private int frameCount;

	private void Start()
	{
		if (Application.platform == RuntimePlatform.PS4)
		{
			maxLines = 38;
		}
	}

	private void Update()
	{
		frameCount++;
	}

	private void OnGUI()
	{
		GUIStyle style = GUI.skin.GetStyle("Label");
		style.fontSize = fontSize;
		style.alignment = TextAnchor.UpperLeft;
		style.wordWrap = false;
		float num = 0f;
		string text = string.Empty;
		foreach (string item in log)
		{
			text = text + " " + item;
			text += "\n";
			num += style.lineHeight;
		}
		num += 6f;
		GUI.Label(new Rect(0f, 0f, Screen.width - 1, num), text, style);
		num = style.lineHeight + 4f;
		GUI.Label(new Rect(Screen.width - 100, Screen.height - 100, Screen.width - 1, num), frameCount.ToString());
	}

	public static void Add(string msg)
	{
		string text = msg.Replace("\r", " ");
		text = text.Replace("\n", " ");
		Console.WriteLine("[APP] " + text);
		log.Add(text);
		msgCount++;
		if (msgCount > maxLines)
		{
			log.RemoveAt(0);
		}
	}
}
