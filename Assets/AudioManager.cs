using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    #region Music
    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip[] ambientClips;
    #endregion

    #region SFX
    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip chickenPickupClip;
    public AudioClip chickenDieClip;
    public AudioClip enemyShootClip;
    public AudioClip playerHitClip;
    public AudioClip playerShootClip;
    public AudioClip upgradePickupClip;
    public AudioClip meteorDestroyClip;
    #endregion

    private AudioClip[] _playlist;
    private int _index;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (ambientClips == null || ambientClips.Length == 0) return;
        if (!musicSource) return;

        _playlist = new AudioClip[ambientClips.Length];
        for (var i = 0; i < ambientClips.Length; i++)
            _playlist[i] = ambientClips[i];

        ShufflePlaylist();
        _index = 0;
        PlayNextMusic();
    }

    private void Update()
    {
        if (!musicSource) return;
        if (_playlist == null || _playlist.Length == 0) return;
        if (musicSource.isPlaying) return;
        PlayNextMusic();
    }

    private void PlayNextMusic()
    {
        if (_playlist == null || _playlist.Length == 0) return;

        if (_index >= _playlist.Length)
        {
            ShufflePlaylist();
            _index = 0;
        }

        var clip = _playlist[_index];
        _index++;

        musicSource.clip = clip;
        musicSource.loop = false;
        musicSource.Play();
    }

    private void ShufflePlaylist()
    {
        if (_playlist is not { Length: > 1 }) return;

        for (var i = 0; i < _playlist.Length - 1; i++)
        {
            var j = Random.Range(i, _playlist.Length);
            (_playlist[i], _playlist[j]) = (_playlist[j], _playlist[i]);
        }
    }

    public void PlayChickenPickup()
    {
        PlaySfx(chickenPickupClip);
    }
    
    public void PlayChickenDie()
    {
        PlaySfx(chickenDieClip);
    }

    public void PlayEnemyShoot()
    {
        PlaySfx(enemyShootClip);
    }

    public void PlayPlayerShoot()
    {
        PlaySfx(playerShootClip);
    }

    public void PlayPlayerHit()
    {
        PlaySfx(playerHitClip);
    }

    public void PlayUpgradePickup()
    {
        PlaySfx(upgradePickupClip);
    }

    public void PlayMeteorDestroy()
    {
        PlaySfx(meteorDestroyClip);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (!clip) return;
        if (!sfxSource) return;
        sfxSource.PlayOneShot(clip);
    }
}
