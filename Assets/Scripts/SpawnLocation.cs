using System;
using UnityEngine;
using UnityEngine.UI;

public class SpawnLocation : MonoBehaviour
{
    public event EventHandler Clicked;

    [SerializeField]
    private Button _spawnHereButton;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spawnHereButton.onClick.AddListener(OnClicked);
    }

    // Update is called once per frame
    protected virtual void OnClicked()
    {
        Clicked?.Invoke(this, EventArgs.Empty);
    }
}
