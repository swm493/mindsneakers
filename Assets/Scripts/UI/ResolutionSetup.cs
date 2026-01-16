using UnityEngine;

public class ForceWindowMode : MonoBehaviour
{
    void Awake()
    {
        Screen.SetResolution(1920, 1080, false);
    }
}