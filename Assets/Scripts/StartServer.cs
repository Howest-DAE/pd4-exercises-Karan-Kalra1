using System.Collections;
using Unity.Netcode;
using UnityEngine;


public class StartServer : MonoBehaviour
{
    private void Start()
    {
#if UNITY_SERVER
        NetworkManager.Singleton.StartServer
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
#endif

    }





    private void OnDestroy()
    {
#if UNITY_SERVER
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
#endif
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected: {clientId}");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client disconnected: {clientId}");
    }
}