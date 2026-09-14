using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] audioClipList;

    [Header("Audio")]
    [SerializeField] private SoundManager musicManager;
    // Start is called before the first frame update
    void Start()
    {
        if (musicManager != null)
        {
            DontDestroyOnLoad(musicManager.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayPickUpSound()
    {
        if (musicManager != null && musicManager.TryGetComponent(out AudioSource source))
        {
            source.enabled = true;
        }
    }
}
