using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : MonoBehaviour
{

    public GameObject[] Characters;
    [SerializeField] private int characterIndex;
    private PlayerInputManager playerInputManager;
    [SerializeField] private Transform[] SpawnPoints;
    private int m_playerCount;

    void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            characterIndex++;
            if(characterIndex == Characters.Length) {characterIndex = 0;}
            playerInputManager.playerPrefab = Characters[characterIndex];
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInput.transform.position = SpawnPoints[m_playerCount].transform.position;
        // if(m_playerCount == 0)
        // {
        //     playerInput.GetComponent<Player>().SwitchOnCamera();
        // }
        m_playerCount++;
    }
}
