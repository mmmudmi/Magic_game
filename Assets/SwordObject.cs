using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwordObject : MonoBehaviour
{
    [SerializeField] private float pickUpRange = 3f;

    private GameSession gameSession;
    private Transform player;

    private void Start()
    {
        gameSession = GameSession.Instance;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (!gameSession.IsEquippedSword && Vector3.Distance(player.position, transform.position) <= pickUpRange)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                PickUpSword();
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (gameSession.IsEquippedSword)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            Transform swordHolder = FindSwordHolder(player, "SwordHolder");
            AttachSwordToPlayer(swordHolder);
        }
    }

    private Transform FindSwordHolder(Transform parentTransform, string swordHolderName)
    {
        foreach (Transform child in parentTransform)
        {
            if (child.name == swordHolderName)
            {
                return child;
            }
            else
            {
                Transform foundChild = FindSwordHolder(child, swordHolderName);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }
        }
        return null;
    }

    private void PickUpSword()
    {
        Debug.Log("PICK!!!");
        Transform swordHolder = FindSwordHolder(player, "SwordHolder");

        if (swordHolder != null)
        {
            gameSession.IsEquippedSword = true;
            AttachSwordToPlayer(swordHolder);
        }
    }

    private void AttachSwordToPlayer(Transform swordHolder)
    {
        Debug.Log("SwordHolder found!");
        transform.SetParent(swordHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
