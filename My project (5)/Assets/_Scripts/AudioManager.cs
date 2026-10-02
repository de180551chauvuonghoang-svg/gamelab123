using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("--- NGUỒN PHÁT ÂM THANH (AUDIO SOURCES) ---")]
    [Tooltip("AudioSource dành riêng cho nhạc nền (phát lặp liên tục)")]
    public AudioSource musicSource;

    [Tooltip("AudioSource dành cho hiệu ứng âm thanh (phát 1 lần bằng PlayOneShot)")]
    public AudioSource sfxSource;

    [Header("--- BẢN NHẠC NỀN (BACKGROUND MUSIC) ---")]
    [Tooltip("Nhạc nền chính của game")]
    public AudioClip bgmClip;

    [Header("--- HIỆU ỨNG ÂM THANH (SOUND EFFECTS) ---")]
    [Tooltip("Âm thanh khi nhân vật nhảy")]
    public AudioClip jumpClip;

    [Tooltip("Âm thanh khi nhặt đồng xu")]
    public AudioClip coinClip;

    [Tooltip("Âm thanh khi Game Over (chạm bẫy/quái)")]
    public AudioClip gameOverClip;

    [Tooltip("Âm thanh khi chiến thắng (nhặt chìa khóa)")]
    public AudioClip winClip;

    [Tooltip("Âm thanh khi click nút giao diện UI")]
    public AudioClip clickClip;

    private void Awake()
    {
        // Áp dụng mô hình Singleton để AudioManager duy nhất tồn tại xuyên suốt các Scene
        if (instance == null)
        {
            instance = this;
            // Giữ AudioManager không bị hủy khi đổi Scene
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Tự động kiểm tra và khởi tạo các AudioSource nếu chưa được gán
        SetupAudioSources();
    }

    private void Start()
    {
        // Tự động phát nhạc nền khi game khởi chạy nếu có bgmClip
        if (bgmClip != null && musicSource != null && !musicSource.isPlaying)
        {
            PlayBGM(bgmClip);
        }
    }

    private void SetupAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = 0.5f;
            musicSource.spatialBlend = 0f; // 2D Sound
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = 0.8f;
            sfxSource.spatialBlend = 0f; // 2D Sound
        }
    }

    // 1. Phát nhạc nền
    public void PlayBGM(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;

        // Nếu đang phát đúng bài này thì không phát lại từ đầu
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // Dừng nhạc nền
    public void StopBGM()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    // 2. Phát hiệu ứng âm thanh tùy biến bằng PlayOneShot
    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, volumeScale);
        }
    }

    // --- CÁC HÀM TIỆN ÍCH PHÁT ÂM THANH CỤ THỂ THEO ĐỀ BÀI LAB 3 ---

    // Phát âm thanh khi Player nhảy
    public void PlayJumpSFX()
    {
        PlaySFX(jumpClip);
    }

    // Phát âm thanh khi Player thu thập Coin
    public void PlayCoinSFX()
    {
        PlaySFX(coinClip);
    }

    // Phát âm thanh khi Game Over
    public void PlayGameOverSFX()
    {
        PlaySFX(gameOverClip);
    }

    // Phát âm thanh khi Thắng game
    public void PlayWinSFX()
    {
        PlaySFX(winClip);
    }

    // Phát âm thanh khi click nút UI
    public void PlayButtonClickSFX()
    {
        PlaySFX(clickClip, 0.7f);
    }
}
