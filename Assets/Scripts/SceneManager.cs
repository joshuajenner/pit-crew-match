using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject cameraPointStart;
    public GameObject cameraPointGame;
    public GameObject startPanel;
    public GameObject pauseButton;
    public GameObject pauseScreen;

    private float timeCount = 0.0f;

    private Vector3 cameraTargetPosition;
    private Vector3 cameraTargetRotation;

    private bool isGameActive = false;

    void Start()
    {
        InitScene();
    }

    private void InitScene()
    {
        startPanel.SetActive(true);
        pauseButton.SetActive(false);
        pauseScreen.SetActive(false);
    }

    void Update()
    {
        if (isGameActive)
        {
            mainCamera.transform.rotation = Quaternion.Slerp(cameraPointStart.transform.rotation, cameraPointGame.transform.rotation, timeCount);
            mainCamera.transform.position = Vector3.Slerp(cameraPointStart.transform.position, cameraPointGame.transform.position, timeCount);
            timeCount = timeCount + Time.deltaTime;
        }
    }
    
    public void SwitchToStart()
    {
        startPanel.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void SwitchToGame()
    {
        isGameActive = true;
        startPanel.SetActive(false);
        pauseButton.SetActive(true);
    }

    public void PauseGame()
    {
        
    }

    public void UnPauseGame()
    {
        
    }

}
