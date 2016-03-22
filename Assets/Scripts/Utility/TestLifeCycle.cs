using UnityEngine;
using System.Collections;

public class TestLifeCycle : MonoBehaviour
{
    void Awake()
    {
        logFrame("Awake");
    }
    void Main()
    {
        logFrame("Main");
    }
    void Start()
    {
        logFrame("Start");
    }

    void OnLevelWasLoaded()
    {
        logFrame("OnLevelWasLoaded");
    }
    void OnNetworkLoadedLevel()
    {
        logFrame("OnNetworkLoadedLevel");
    }

    void OnEnable()
    {
        logFrame("OnEnable");
    }
    void OnDisable()
    {
        logFrame("OnDisable");
    }
    void OnApplicationQuit()
    {
        logFrame("OnApplicationQuit");
    }


    void Update()
    {
        logFrame("Update");
    }
    void LateUpdate()
    {
        logFrame("LateUpdate");
    }
    void FixedUpdate()
    {
        logFrame("FixedUpdate");
    }
    void LateFixedUpdate()
    {
        logFrame("LateFixedUpdate");
    }

    void OnDrawGizmos()
    {
        logFrame("OnDrawGizmos");
    }
    void OnDrawGizmosSelected()
    {
        logFrame("OnDrawGizmosSelected");
    }


    static void logFrame(string message)
    {
        Debug.Log(Time.frameCount + " - " + message);
    }
}