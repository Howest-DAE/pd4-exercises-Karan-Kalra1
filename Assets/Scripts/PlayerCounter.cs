using TMPro;
using Unity.Netcode;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class PlayerCounter : NetworkBehaviour
{
    
    private NetworkVariable<int> _counter;

    [SerializeField]
    TextMeshProUGUI _countText;
    [SerializeField]
    InputActionReference _inputAction;
    private void Awake()
    {
        _counter = new NetworkVariable<int>( default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

        _inputAction.action.performed += Action_performed;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        UpdateUItext(0, 0);

        _counter.OnValueChanged += UpdateUItext;

    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        if (IsOwner)
        {
            _counter.Value++;
        }
        

    }
    void UpdateUItext(int previous, int newValue)
    {

        _countText.text = _counter.Value.ToString();
    }
}
