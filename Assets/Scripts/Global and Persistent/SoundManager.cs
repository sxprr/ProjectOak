using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] audioClipList;

    [Header("Audio")]
    [SerializeField] private SoundManager instance;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            DontDestroyOnLoad(instance.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayPickUpSound()
    {
        if (instance != null && instance.TryGetComponent(out AudioSource source))
        {
            source.enabled = true;
        }
    }
}
