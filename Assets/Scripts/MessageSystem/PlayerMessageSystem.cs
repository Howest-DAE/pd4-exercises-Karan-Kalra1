using System.Linq;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMessageSystem : NetworkBehaviour   
{
	[SerializeField]
	SendMessagePanel _messagePanel;

	[SerializeField]
	private InputActionReference _clickAction;

	void Start()
	{
		_clickAction.action.Enable();
		_clickAction.action.performed += Action_performed;
		_messagePanel.MessageSent += _messagePanel_MessageSent;
	}

	private void Action_performed(InputAction.CallbackContext obj)
	{
		Vector3 mousePos = Mouse.current.position.ReadValue();
		mousePos.z = Camera.main.farClipPlane;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePos), out RaycastHit hitInfo, Camera.main.farClipPlane))
		{
			if (hitInfo.collider.CompareTag("Player"))
			{
				PlayerMessageBubble targetMessageBubble = hitInfo.collider.GetComponentInParent<PlayerMessageBubble>();

                //Testing: Directly show message on the clicked object
                //targetMessageBubble.ShowMessageRpc("Ouch!");
                //TODO - ex. 1: show the message on the clicked client!

                //TODO: Show the MessagePanel with the correct playerIds
                if (targetMessageBubble != null)
                {
                    ulong targetClientId = targetMessageBubble.OwnerClientId;
                    ulong localClientId = NetworkManager.LocalClientId;

                    _messagePanel.Show(targetClientId, localClientId);
                }

            }
        }
	}

    [Rpc(SendTo.SpecifiedInParams)]
    private void ReceivedSecretMessageRpc(
    string message,
    ulong fromPlayerId,
    RpcParams rpcParams = default)
    {

        Debug.Log($"Received secret message: {message} from {fromPlayerId}");

        // Find the PlayerMessageBubble of the sender
        PlayerMessageBubble fromPlayerBubble =
            FindObjectsByType<PlayerMessageBubble>(FindObjectsSortMode.None)
            .FirstOrDefault(p => p.OwnerClientId == fromPlayerId);


        if (fromPlayerBubble != null)
        {
            // Show message locally (NO RPC)
            fromPlayerBubble.ShowMessage(message);
        }
    }

    /*private void Action_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Clicked!");

        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Camera.main.farClipPlane;

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Camera.main.farClipPlane))
        {
            Debug.Log("Hit: " + hitInfo.collider.name);

            if (hitInfo.collider.CompareTag("Player"))
            {
                Debug.Log("Player hit!");

                PlayerMessageBubble targetMessageBubble = hitInfo.collider.GetComponent<PlayerMessageBubble>();

                if (targetMessageBubble != null)
                {
                    targetMessageBubble.ShowMessage("Ouch!");
                }
                else
                {
                    Debug.LogError("PlayerMessageBubble not found!");
                }
            }
        }
    }
	*/

    private void _messagePanel_MessageSent(object sender, SendMessagePanel.MessageEventArgs e)
    {
        Debug.Log($"Sending secret message \"{e.Message}\" to player {e.TargetPlayerId}");


        ReceivedSecretMessageRpc(
            e.Message,
            e.SourcePlayerId,
            RpcTarget.Single(e.TargetPlayerId, RpcTargetUse.Temp)
        );
    }

}
