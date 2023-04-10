using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LastExit : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        Debug.Log("GO FALSE");

        if (other.CompareTag("Monster"))
        {
            other.gameObject.SetActive(false);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}
