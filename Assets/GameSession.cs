using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public int playerHealth;
    public int bossHealth;
      private Transform player;

    private bool isSwordEquipped = false;
    
    private bool isShieldEquipped = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public bool IsEquippedSword 
    { 
        
        get => isSwordEquipped;
        set => isSwordEquipped = value;
        
    }
        public bool IsEquippedShield 
    { 
        
        get => isShieldEquipped;
        set => isShieldEquipped = value;
        
    }
    
    // private Transform FindSwordHolder(Transform parentTransform, string swordHolderName)
    // {
    //     foreach (Transform child in parentTransform)
    //     {
    //         if (child.name == swordHolderName)
    //         {
    //             return child;
    //         }
    //         else
    //         {
    //             Transform foundChild = FindSwordHolder(child, swordHolderName);
    //             if (foundChild != null)
    //             {
    //                 return foundChild;
    //             }
    //         }
    //     }
    //     return null;
    // }
private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    if (isSwordEquipped)
    {
        Debug.Log("Sword equipped");
        GameObject playerCheck = GameObject.FindGameObjectWithTag("Player");
        if (playerCheck != null)
        {
            Debug.Log("Player not null");
            GameObject swordHolder = GameObject.FindWithTag("SwordHolder");
            if (swordHolder != null)
            {
                Debug.Log("Sword holder not null");

                SwordObject swordObject = FindObjectOfType<SwordObject>();
                if (swordObject != null)
                {
                    Debug.Log("Sword object not null");
                    swordObject.transform.SetParent(swordHolder.transform);
                    swordObject.transform.localPosition = Vector3.zero;
                    swordObject.transform.localRotation = Quaternion.identity;
                    swordObject.GetComponent<Collider>().enabled = true;
                    swordObject.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
        }
    }

    if (isShieldEquipped)
    {
        Debug.Log("Shield equipped");
        GameObject playerCheck = GameObject.FindGameObjectWithTag("Player");
        if (playerCheck != null)
        {
            Debug.Log("Player not null");
            GameObject shieldHolder = GameObject.FindWithTag("ShieldHolder");
            if (shieldHolder != null)
            {
                Debug.Log("Shield holder not null");

                ShieldObject shieldObject = FindObjectOfType<ShieldObject>();
                if (shieldObject != null)
                {
                    Debug.Log("Shield object not null");
                    shieldObject.transform.SetParent(shieldHolder.transform);
                    shieldObject.transform.localPosition = Vector3.zero;
                    shieldObject.transform.localRotation = Quaternion.identity;
                    shieldObject.GetComponent<Collider>().enabled = true;
                    shieldObject.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
        }
    }
}



}
