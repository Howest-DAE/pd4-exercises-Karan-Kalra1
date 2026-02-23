using Unity.Netcode;
using UnityEngine;

public class SpawnSystem : NetworkBehaviour
{
    [SerializeField]
    private SpawnLocation[] _spawnPositions;

    [SerializeField]
    private NetworkObject _playerPrefab;

    private void Awake()
    {
        // Register to click events
        for (int i = 0; i < _spawnPositions.Length; i++)
        {
            int index = i; // Important: capture index
            _spawnPositions[i].Clicked += (sender, e) => OnSpawnLocationClicked(index);

            // Deactivate initially
            _spawnPositions[i].gameObject.SetActive(false);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Activate spawn locations when network starts
        foreach (var spawn in _spawnPositions)
        {
            spawn.gameObject.SetActive(true);
        }
    }

    private void OnSpawnLocationClicked(int spawnIndex)
    {
        // Only clients should request spawn
        if (!IsClient) return;

        ulong clientId = NetworkManager.LocalClientId;

        SpawnPlayerAvatarRpc(spawnIndex, clientId);
    }

    [Rpc(SendTo.Server)]
    private void SpawnPlayerAvatarRpc(int spawnIndex, ulong clientId)
    {
        // Server logic

        Transform spawnTransform = _spawnPositions[spawnIndex].transform;

        NetworkObject player =
            Instantiate(_playerPrefab,
                        spawnTransform.position,
                        spawnTransform.rotation);

        player.SpawnWithOwnership(clientId);
    }
}
