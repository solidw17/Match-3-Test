using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        float orthoSize = 7f * Screen.height / Screen.width * 0.5f;

        Camera.main.orthographicSize = orthoSize;
    }
}
