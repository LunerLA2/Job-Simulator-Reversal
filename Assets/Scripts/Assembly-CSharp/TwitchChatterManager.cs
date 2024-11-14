using UnityEngine;
using UnityEngine.UI;

public class TwitchChatterManager : MonoBehaviour
{
	private string channelName;

	[SerializeField]
	private Text statusText;

	[SerializeField]
	private InputField channelNameInputField;

	private Text channelNameText;

	[SerializeField]
	private Button loadChannelButton;

	[SerializeField]
	private Button clearChannelButton;

	[SerializeField]
	private GameObject ChatObjectPrefab;

	private GameObject chatObject;

	private bool shouldChatObjectBeActive;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.identity;
		channelNameInputField.textComponent.text = string.Empty;
		channelNameText = channelNameInputField.textComponent;
		loadChannelButton.interactable = false;
		clearChannelButton.gameObject.SetActive(false);
		statusText.text = "Enter Your Channel Name";
	}

	private void Update()
	{
		if (chatObject == null)
		{
			chatObject = Object.Instantiate(ChatObjectPrefab, Vector3.zero + Vector3.up, Quaternion.identity) as GameObject;
			chatObject.transform.parent = base.transform;
		}
		if (chatObject.activeInHierarchy != shouldChatObjectBeActive)
		{
			chatObject.SetActive(shouldChatObjectBeActive);
		}
		if (loadChannelButton.gameObject.activeInHierarchy && loadChannelButton.interactable != (channelNameText.text != string.Empty))
		{
			loadChannelButton.interactable = channelNameText.text != string.Empty;
		}
	}

	public void LoadChat()
	{
		if (channelName != null)
		{
			TwitchChatClient.singleton.LeaveChannel(channelName);
			channelName = null;
		}
		channelName = channelNameInputField.textComponent.text;
		if (channelName == null)
		{
			Debug.LogError("This Should Never Happen Since Button Is Not Interactable If Text is blank");
			return;
		}
		TwitchChatClient.singleton.JoinChannel(channelName);
		shouldChatObjectBeActive = true;
		channelNameInputField.readOnly = true;
		channelNameInputField.interactable = false;
		loadChannelButton.gameObject.SetActive(false);
		clearChannelButton.gameObject.SetActive(true);
		statusText.text = "Twitch Chat Created";
	}

	public void DestroyChat()
	{
		TwitchChatClient.singleton.LeaveChannel(channelName);
		shouldChatObjectBeActive = false;
		Object.Destroy(chatObject);
		channelName = null;
		channelNameInputField.readOnly = false;
		channelNameInputField.interactable = true;
		loadChannelButton.gameObject.SetActive(true);
		clearChannelButton.gameObject.SetActive(false);
		statusText.text = "Enter Your Channel Name";
	}
}
