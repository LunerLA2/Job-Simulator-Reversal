using System.Collections.Generic;
using TwitchChatter;
using UnityEngine;
using UnityEngine.UI;

public class TwitchChatPoll : MonoBehaviour
{
	public string _pollChannelName;

	public InputField _optionOneLabel;

	public InputField _optionTwoLabel;

	public InputField _optionThreeLabel;

	public Text _optionOneCountLabel;

	public Text _optionTwoCountLabel;

	public Text _optionThreeCountLabel;

	private int _optionOneCount;

	private int _optionTwoCount;

	private int _optionThreeCount;

	private List<string> _voterList;

	private void Awake()
	{
		_voterList = new List<string>();
	}

	private void Start()
	{
		if (TwitchChatClient.singleton != null)
		{
			TwitchChatClient.singleton.AddChatListener(OnChatMessage);
		}
		if (!string.IsNullOrEmpty(_pollChannelName))
		{
			TwitchChatClient.singleton.JoinChannel(_pollChannelName);
		}
		else
		{
			Debug.LogWarning("No channel name entered for poll! Enter a channel name and restart the scene.", this);
		}
	}

	private void OnDestroy()
	{
		if (TwitchChatClient.singleton != null)
		{
			TwitchChatClient.singleton.RemoveChatListener(OnChatMessage);
		}
	}

	private void OnChatMessage(ref TwitchChatMessage msg)
	{
		if (!_voterList.Contains(msg.userName))
		{
			bool flag = false;
			if (msg.chatMessagePlainText.Equals("#1"))
			{
				flag = true;
				_optionOneCount++;
				_optionOneCountLabel.text = string.Empty + _optionOneCount;
			}
			else if (msg.chatMessagePlainText.Equals("#2"))
			{
				flag = true;
				_optionTwoCount++;
				_optionTwoCountLabel.text = string.Empty + _optionTwoCount;
			}
			else if (msg.chatMessagePlainText.Equals("#3"))
			{
				flag = true;
				_optionThreeCount++;
				_optionThreeCountLabel.text = string.Empty + _optionThreeCount;
			}
			if (flag)
			{
				_voterList.Add(msg.userName);
			}
		}
	}

	public void OnResetButtonPressed()
	{
		_optionOneLabel.text = string.Empty;
		_optionTwoLabel.text = string.Empty;
		_optionThreeLabel.text = string.Empty;
		_optionOneCount = 0;
		_optionTwoCount = 0;
		_optionThreeCount = 0;
		_optionOneCountLabel.text = "0";
		_optionTwoCountLabel.text = "0";
		_optionThreeCountLabel.text = "0";
		_voterList.Clear();
	}
}
