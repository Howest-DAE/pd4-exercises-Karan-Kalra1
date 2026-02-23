using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SendMessagePanel : MonoBehaviour
{
	public class MessageEventArgs : EventArgs
	{
		public MessageEventArgs(string message, ulong targetPlayerId, ulong sourcePlayerId)
		{
			Message = message;
			TargetPlayerId = targetPlayerId;
			SourcePlayerId = sourcePlayerId;
		}

		public string Message { get; }
		public ulong TargetPlayerId { get; }
		public ulong SourcePlayerId { get; }
	}

	public event EventHandler<MessageEventArgs> MessageSent;

	[SerializeField]
	private Button _sendButton, _cancelButton;

	[SerializeField]
	private TextMeshProUGUI _playerIdTxt;

	[SerializeField]
	private TMP_InputField _textInput;

	private ulong _targetPlayerId, _sourcePlayerId;
	private void Awake()
	{
		_sendButton.onClick.AddListener(Send);
		_cancelButton.onClick.AddListener(Cancel);
		_textInput.onValueChanged.AddListener(TextChanged);
	}

	public void Show(ulong targetPlayerId, ulong sourcePlayerId)
	{
		_textInput.text = "";
		_targetPlayerId = targetPlayerId;
		_sourcePlayerId = sourcePlayerId;
		_playerIdTxt.text = _targetPlayerId.ToString();
		gameObject.SetActive(true);
	}

	void Send()
	{
		string message = _textInput.text;
		OnSendMessage(message, _targetPlayerId, _sourcePlayerId);
		gameObject.SetActive(false);
	}

	void Cancel()
	{
		gameObject.SetActive(false);
	}

	void TextChanged(string value)
	{
		_sendButton.enabled = !string.IsNullOrWhiteSpace(value);
	}

	protected virtual void OnSendMessage(string message, ulong targetPlayerId, ulong sourcePlayerId)
	{
		MessageSent?.Invoke(this, new MessageEventArgs(message, targetPlayerId, sourcePlayerId));
	}
}
