using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Exit : MonoBehaviour
{

    [SerializeField] AudioClip warpSFX;
    public GameObject LoadingScene;

    void Awake(){
        LoadingScene.SetActive(false);
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("GO FALSE");

        if (other.CompareTag("Boss"))
        {
            other.gameObject.SetActive(false);
        }
        else
        {
            AudioSource.PlayClipAtPoint(warpSFX, transform.position,2f); 

            StartCoroutine(LoadNextLevel());
        }
    }

    IEnumerator LoadNextLevel()
    {
        // aka: come back to run the following line after the delay
        yield return new WaitForSecondsRealtime(1);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if(nextSceneIndex == SceneManager.sceneCountInBuildSettings) {
            nextSceneIndex = 0;
        }
        // Before loading the next level, have to destroy the ScenePersist object so that
        // the new one of the new level will be there to do the work       

        //SceneManager.LoadScene(nextSceneIndex);
        LoadingScene.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneIndex);
    }

}
