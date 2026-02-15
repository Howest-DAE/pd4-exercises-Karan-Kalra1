using Unity.Netcode;
using UnityEngine;

public class JoinAs : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    public void StartAsClient()
    {
        
        NetworkManager.Singleton.StartClient();

    }

    public void StartAsHost()
    {
        NetworkManager.Singleton.StartHost();

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
