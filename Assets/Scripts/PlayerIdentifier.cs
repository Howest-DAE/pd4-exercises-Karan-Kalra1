using UnityEngine;
using Unity.Netcode;

public class PlayerIdentifier : NetworkBehaviour
{
    [SerializeField]
    private Renderer _renderer;     // cube renderer

    [SerializeField]
    private Material _hostMaterial; // orange material

    private Material _clientMaterial;

    [SerializeField]
    private GameObject _localMarker; // green sphere

    public override void OnNetworkSpawn()
    {   
        //Save the default Color
        _clientMaterial = _renderer.material;

        // LOCAL PLAYER (this client owns this object)
        if (IsOwner)
        {
            if (_localMarker != null)
                _localMarker.SetActive(true);
        }
        else
        {
            if (_localMarker != null)
                _localMarker.SetActive(false);
        }

        // HOST PLAYER (server owner)
        if (OwnerClientId == NetworkManager.ServerClientId)
        {
            if (_renderer != null && _hostMaterial != null)
                _renderer.material = _hostMaterial;
        }
        if (IsClient)
        {
            if (_renderer != null && _renderer.material == _hostMaterial)
                _renderer.material = _clientMaterial;
        }
    }
}
