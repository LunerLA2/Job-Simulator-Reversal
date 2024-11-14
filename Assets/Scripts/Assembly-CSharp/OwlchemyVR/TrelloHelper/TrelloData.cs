using System;
using System.Collections.Generic;
using UnityEngine;

namespace OwlchemyVR.TrelloHelper
{
	[Serializable]
	public class TrelloData : ScriptableObject
	{
		[HideInInspector]
		[SerializeField]
		private int currentBoardIndex;

		[SerializeField]
		[HideInInspector]
		private int currentListIndex;

		[SerializeField]
		[HideInInspector]
		private string token;

		[HideInInspector]
		[SerializeField]
		private List<BoardData> boards;

		[HideInInspector]
		[SerializeField]
		private string[] boardNames;

		[SerializeField]
		[HideInInspector]
		private List<ListData> lists;

		[SerializeField]
		[HideInInspector]
		private string[] listNames;

		[SerializeField]
		[HideInInspector]
		private float doubleClickDuration;

		[HideInInspector]
		[SerializeField]
		private bool screenshotMode;

		[HideInInspector]
		[SerializeField]
		private bool trackScreenshots;

		[HideInInspector]
		[SerializeField]
		private int screenshotNumber;

		public int CurrentBoardIndex
		{
			get
			{
				return currentBoardIndex;
			}
		}

		public int CurrentListIndex
		{
			get
			{
				return currentListIndex;
			}
		}

		public string Token
		{
			get
			{
				return token;
			}
		}

		public List<BoardData> Boards
		{
			get
			{
				return boards;
			}
		}

		public string[] BoardNames
		{
			get
			{
				return boardNames;
			}
		}

		public List<ListData> Lists
		{
			get
			{
				return lists;
			}
		}

		public string[] ListNames
		{
			get
			{
				return listNames;
			}
		}

		public float DoubleClickDuration
		{
			get
			{
				return doubleClickDuration;
			}
		}

		public bool ScreenshotMode
		{
			get
			{
				return screenshotMode;
			}
		}

		public bool TrackScreenshots
		{
			get
			{
				return trackScreenshots;
			}
		}

		public int ScreenshotNumber
		{
			get
			{
				return screenshotNumber;
			}
		}

		public void SetBoards(List<BoardData> b)
		{
			boards = b;
			if (boards != null)
			{
				boardNames = new string[boards.Count];
				for (int i = 0; i < boards.Count; i++)
				{
					boardNames[i] = boards[i].name;
					boardNames[i] = boardNames[i].Replace('/', '-');
				}
			}
		}

		public void SetLists(List<ListData> l)
		{
			lists = l;
			if (lists != null)
			{
				listNames = new string[lists.Count];
				for (int i = 0; i < lists.Count; i++)
				{
					listNames[i] = lists[i].name;
					listNames[i] = listNames[i].Replace('/', '-');
				}
			}
		}

		public void SetCurrentBoardIndex(int i)
		{
			currentBoardIndex = i;
		}

		public void SetCurrentListIndex(int i)
		{
			currentListIndex = i;
		}

		public void SetToken(string t)
		{
			token = t;
		}

		public void SetDoubleClickDuration(float f)
		{
			doubleClickDuration = f;
		}

		public void SetScreenshotMode(bool b)
		{
			screenshotMode = b;
		}

		public void SetTrackScreenshots(bool b)
		{
			trackScreenshots = b;
		}

		public void SetScreenshotNumber(int i)
		{
			screenshotNumber = i;
		}
	}
}
