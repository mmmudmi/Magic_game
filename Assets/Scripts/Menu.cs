using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] AudioClip clickAudio;
    
    public void PlayAudio()
    {
        AudioSource.PlayClipAtPoint(clickAudio, Camera.main.transform.position,1f); 

    }
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
    public void QuitGame()
    {
        Debug.Log("Quit the game ...");
        Application.Quit();
    }
    public void goMainMenu()
    {
        SceneManager.LoadScene(0);

    }


}
