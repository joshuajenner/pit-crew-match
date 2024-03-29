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
    public GameObject settingsScreen;

    [SerializeField] MusicManager musicManager;

    private float timeCount = 0.0f;

    private Vector3 cameraCurrentPosition;
    private Quaternion cameraCurrentRotation;

    private Vector3 cameraTargetPosition;
    private Quaternion cameraTargetRotation;


    void Start()
    {
        InitScene();
    }

    private void InitScene()
    {
        SwitchToStart();
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(false);
    }

    void Update()
    {
        float distanceToTarget = (mainCamera.transform.position - cameraTargetPosition).magnitude;

        if (distanceToTarget > 0.1f)
        {
            mainCamera.transform.position = Vector3.Slerp(cameraCurrentPosition, cameraTargetPosition, timeCount);
            mainCamera.transform.rotation = Quaternion.Slerp(cameraCurrentRotation, cameraTargetRotation, timeCount);
            timeCount += Time.deltaTime * 1.5f;
        }
        else
        {
            cameraCurrentPosition = cameraTargetPosition;
            cameraCurrentRotation = cameraTargetRotation;
            mainCamera.transform.position = cameraCurrentPosition;
            mainCamera.transform.rotation = cameraCurrentRotation;
        }
    }
    
    public void SwitchToStart()
    {
        musicManager.PlayMenuMusic();
        startPanel.SetActive(true);
        pauseButton.SetActive(false);
        cameraTargetPosition = cameraPointStart.transform.position;
        cameraTargetRotation = cameraPointStart.transform.rotation;

        timeCount = 0;
    }

    public void SwitchToGame()
    {
        musicManager.PlayGameMusic();
        startPanel.SetActive(false);
        pauseButton.SetActive(true);
        cameraTargetPosition = cameraPointGame.transform.position;
        cameraTargetRotation = cameraPointGame.transform.rotation;
        timeCount = 0;
    }

    public void PauseGame()
    {
        pauseScreen.SetActive(true);
        musicManager.Pause();
    }

    public void UnPauseGame()
    {
        pauseScreen.SetActive(false);
        musicManager.Play();
    }

}
