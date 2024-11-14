using System.Collections.Generic;
using TwitchChatter;
using UnityEngine;
using UnityEngine.UI;

public class TwitchChatRaffle : MonoBehaviour
{
	public string _raffleChannelName;

	public Text _keywordLabel;

	public Text _buttonLabel;

	public Text _output;

	private List<string> _raffleEntrants;

	private bool _isRaffleStarted;

	private void Awake()
	{
		_raffleEntrants = new List<string>();
	}

	private void Start()
	{
		if (TwitchChatClient.singleton != null)
		{
			TwitchChatClient.singleton.AddChatListener(OnChatMessage);
		}
		if (!string.IsNullOrEmpty(_raffleChannelName))
		{
			TwitchChatClient.singleton.JoinChannel(_raffleChannelName);
		}
		else
		{
			Debug.LogWarning("No channel name entered for raffle! Enter a channel name and restart the scene.", this);
		}
	}

	private void OnDestroy()
	{
		if (TwitchChatClient.singleton != null)
		{
			TwitchChatClient.singleton.RemoveChatListener(OnChatMessage);
		}
	}

	public void OnButtonPress()
	{
		if (_isRaffleStarted)
		{
			_isRaffleStarted = false;
			if (_raffleEntrants.Count > 0)
			{
				_output.text = _raffleEntrants[Random.Range(0, _raffleEntrants.Count)] + " wins!";
			}
			else
			{
				_output.text = "No winner!";
			}
			_buttonLabel.text = "Start raffle!";
		}
		else
		{
			_isRaffleStarted = true;
			_raffleEntrants.Clear();
			_buttonLabel.text = "Pick winner!";
		}
	}

	private void Update()
	{
		if (_isRaffleStarted)
		{
			_output.text = string.Empty + _raffleEntrants.Count + " entrants!";
		}
	}

	private void OnChatMessage(ref TwitchChatMessage msg)
	{
		if (_isRaffleStarted && msg.chatMessagePlainText.ToLower().Equals(_keywordLabel.text.ToLower()) && !_raffleEntrants.Contains(msg.userName))
		{
			_raffleEntrants.Add(msg.userName);
		}
	}
}
