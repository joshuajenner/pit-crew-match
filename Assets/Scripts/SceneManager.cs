using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject cameraPointStart;
    public GameObject cameraPointGame;

    private float timeCount = 0.0f;

    private Vector3 cameraTargetPosition;
    private Vector3 cameraTargetRotation;

    private bool isGameActive = false;

    void Start()
    {
        
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


    public void SwitchToGame()
    {
        isGameActive = true;
    }
}
