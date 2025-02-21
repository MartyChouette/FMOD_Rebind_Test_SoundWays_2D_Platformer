using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        // Synchronously load the master bank so events are ready to be triggered immediately.
        RuntimeManager.LoadBank("SFX", true);
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
