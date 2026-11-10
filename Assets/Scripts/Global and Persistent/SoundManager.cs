using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSourcePrimary;
    [SerializeField] private AudioSource musicSourceSecondary; // For concurrent game ambient tracks
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource jingleSource;          // Dedicated source for UI/Win/Loss jingles

    [Header("Audio Clips")]
    [SerializeField] private AudioClip menuAmbientMusic;       // 1. Main Menu ambient
    [SerializeField] private AudioClip gameAmbientTrack1;      // 2. Game Scene ambient track A
    [SerializeField] private AudioClip gameAmbientTrack2;      // 3. Game Scene ambient track B (concurrent)
    [SerializeField] private AudioClip playerWalkingSFX;       // 4. Player walking loop/shot
    [SerializeField] private AudioClip pickupJingle;           // 5. Pick up jingle
    [SerializeField] private AudioClip finishLineJingle;       // 6. Finish Line victory jingle
    [SerializeField] private AudioClip gameOverJingle;         // 7. Game Over death jingle
    [SerializeField] private AudioClip enemyDetectJingle;      // 8. Enemy detection jingle
    [SerializeField] private AudioClip senseAbilityJingle;     // 9. Sense Ability
    [SerializeField] private AudioClip enemyPatrolJingle;      // 10. Enemy Patrol Jingle
    [SerializeField] private AudioClip sprintStartJingle;      // 11. Sprint start Jingle


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            LogHandler.Log("Awake nullcheck ");

            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- Music Controls ---

    public void PlayMenuMusic()
    {
        // Stop secondary source if active from gameplay
        if (musicSourceSecondary.isPlaying) musicSourceSecondary.Stop();

        LogHandler.Log($"'{musicSourceSecondary.name}' has been stopped due to Menu transition. Currently playing " +
            $"'{menuAmbientMusic.name}', assigned to '{musicSourcePrimary.name}'");

        PlayMusic(musicSourcePrimary, menuAmbientMusic, loop: true);
    }

    //there's a silent error?
    public void PlayGameMusicConcurrently()
    {
        Debug.Log("PlayGameMusicConcurrently entered");


        // Plays both game ambient tracks together concurrently
        PlayMusic(musicSourcePrimary, gameAmbientTrack1, loop: true);
        PlayMusic(musicSourceSecondary, gameAmbientTrack2, loop: true);

        //doesn't appear in the console.
        Debug.Log($"'{musicSourceSecondary.name}' has started due to MainGame.. Currently playing " +
            $"'{gameAmbientTrack1}', assigned to '{musicSourcePrimary.name}'");


    }

    private void PlayMusic(AudioSource source, AudioClip clip, bool loop)
    {
        if (source == null)
        {
            Debug.LogWarning("PlayMusic: AudioSource is missing or destroyed", this);
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning($"PlayMusic: clip not assigned for source '{source.name}'", this);
            return;
        }

        if (source.clip == clip && source.isPlaying) return;

        source.clip = clip;
        source.loop = loop;
        source.Play();
    }

    // --- SFX & Jingle Controls ---

    public void PlayWalkingSFX()
    {
        if (sfxSource != null && playerWalkingSFX != null)
        {
            // Use PlayOneShot or toggle looping depending on your walking implementation
            if (!sfxSource.isPlaying)
            {
                sfxSource.clip = playerWalkingSFX;
                sfxSource.loop = true;
                sfxSource.Play();
            }
        }
    }

    public void StopWalkingSFX()
    {
        if (sfxSource != null && sfxSource.clip == playerWalkingSFX)
        {
            sfxSource.Stop();
            sfxSource.loop = false;
        }
    }

    public void PlayPickupJingle() => PlayJingle(pickupJingle);
    public void PlayFinishLineJingle() => PlayJingle(finishLineJingle);
    public void PlayGameOverJingle() => PlayJingle(gameOverJingle);
    public void PlayEnemyDetectionJingle() => PlayJingle(enemyDetectJingle);

    public void PlayerSenseAbilityJingle() => PlayJingle(senseAbilityJingle);

    private void PlayJingle(AudioClip clip)
    {
        if (clip == null) { Debug.LogWarning("PlayJingle: clip is null", this); return; }
        if (jingleSource == null) { Debug.LogWarning("PlayJingle: jingleSource is missing/destroyed", this); return; }
        if (!jingleSource.isActiveAndEnabled) { Debug.LogWarning("PlayJingle: jingleSource is disabled", this); return; }

        jingleSource.PlayOneShot(clip);
        LogHandler.Log($"Played jingle '{clip.name}'");
    }
}

