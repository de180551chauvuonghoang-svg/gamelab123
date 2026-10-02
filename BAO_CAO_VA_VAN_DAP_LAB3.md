# 🎓 TÀI LIỆU BÁO CÁO, GIẢI THÍCH MÃ NGUỒN & BỘ CÂU HỎI VẤN ĐÁP LAB 3 (UNITY 2D PLATFORMER)

> **Dự án:** Game 2D Platformer (Lab 3)  
> **Phiên bản Engine:** Unity 6 (6000.3.23f1)  
> **Nội dung trọng tâm:** Hệ thống âm thanh (AudioManager - BGM & SFX), Sàn di chuyển (Moving Platform & SetParent), Màn hình Menu chính (Main Menu Scene & TextMeshPro), Đóng gói & Xuất bản game (.EXE Build Settings).

---

## 📑 MỤC LỤC
1. [Tổng quan dự án & Yêu cầu bài Lab 3](#1-tổng-quan-dự-án--yêu-cầu-bài-lab-3)
2. [Cấu trúc tài nguyên & Thư mục mới của Lab 3](#2-cấu-trúc-tài-nguyên--thư-mục-mới-của-lab-3)
3. [Giải thích chi tiết toàn bộ mã nguồn C#](#3-giải-thích-chi-tiết-toàn-bộ-mã-nguồn-c)
   - [3.1. AudioManager.cs (Mẫu Singleton & 2 AudioSource BGM/SFX)](#31-audiomanagercs-mẫu-singleton--2-audiosource-bgmsfx)
   - [3.2. MovingPlatform.cs (Sàn di chuyển tuần tra & Cơ chế SetParent)](#32-movingplatformcs-sàn-di-chuyển-tuần-tra--cơ-chế-setparent)
   - [3.3. MainMenuController.cs (Điều khiển Menu chính & Quản lý Scene)](#33-mainmenucontrollercs-điều-khiển-menu-chính--quản-lý-scene)
   - [3.4. Tích hợp âm thanh vào PlayerController, Coin, GameManager, UIButtonAction](#34-tích-hợp-âm-thanh-vào-playercontroller-coin-gamemanager-uibuttonaction)
   - [3.5. Lab3Automator.cs (Công cụ tự động hóa toàn diện 1-Click Setup & Build .EXE)](#35-lab3automatorcs-công-cụ-tự-động-hóa-toàn-diện-1-click-setup--build-exe)
4. [Các kiến thức cốt lõi về lập trình Game Unity đã áp dụng](#4-các-kiến-thức-cốt-lõi-về-lập-trình-game-unity-đã-áp-dụng)
5. [Hướng dẫn trải nghiệm & Test Lab 3](#5-hướng-dẫn-trải-nghiệm--test-lab-3)
6. [Kịch bản thuyết trình với Giảng viên](#6-kịch-bản-thuyết-trình-với-giảng-viên)
7. [Bộ 10 câu hỏi vấn đáp (Oral Exam) Lab 3 kèm đáp án chuẩn](#7-bộ-10-câu-hỏi-vấn-đáp-oral-exam-lab-3-kèm-đáp-án-chuẩn)

---

## 1. TỔNG QUAN DỰ ÁN & YÊU CẦU BÀI LAB 3

Theo tài liệu đặc tả môn học trong `Lab123.docx`, **Lab 3** là giai đoạn hoàn thiện tựa game 2D Platformer thành một sản phẩm thương mại hoàn chỉnh với trải nghiệm âm thanh - hình ảnh sống động, cơ chế chướng ngại vật cơ khí và khả năng xuất bản độc lập:

1. **Mục 6 - Hệ thống âm thanh (Audio Manager):**
   - Xây dựng script `AudioManager` theo mô hình Singleton quản lý âm thanh tập trung.
   - Sử dụng **hai AudioSource độc lập**:
     - *Background Music (BGM):* Phát nhạc nền liên tục, bật chế độ `loop = true`, duy trì xuyên suốt màn chơi.
     - *Sound Effects (SFX):* Phát hiệu ứng âm thanh một lần bằng phương thức `PlayOneShot()`.
   - Phát Sound Effect tương ứng với các tương tác của Player:
     - Player thực hiện cú nhảy (`jump.wav`).
     - Player thu thập đồng xu Coin (`coin.wav`).
     - Bổ sung hiệu ứng GameOver, Chiến thắng và Click Button UI.

2. **Mục 7 - Sàn di chuyển (Moving Platform) & Main Menu:**
   - **Moving Platform:**
     - Xây dựng script điều khiển sàn cơ khí tự động trượt qua lại giữa 2 điểm mốc A và B.
     - Khi Player nhảy lên đứng trên Moving Platform, sử dụng `SetParent()` gán Player làm con (`Child`) của Platform để Player di chuyển đồng tốc cùng sàn mà không bị trượt chân rơi xuống vực.
     - Khi Player nhảy ra khỏi sàn, hủy quan hệ cha-con (`SetParent(null)`).
   - **Font chữ & Main Menu:**
     - Sử dụng TextMeshPro tùy chỉnh hiển thị giao diện sắc nét.
     - Tạo riêng một Scene **`MainMenu`** độc lập gồm:
       - Nút **Play**: Chuyển sang màn chơi chính.
       - Nút **Quit**: Thoát khỏi trò chơi.

3. **Mục 8 - Đóng gói và xuất bản Game (Build):**
   - Cấu hình Build Settings: Thêm toàn bộ Scene cần thiết, sắp xếp `MainMenu.unity` ở vị trí đầu tiên (`Index 0`) và `SampleScene.unity` ở vị trí thứ hai (`Index 1`).
   - Cấu hình thiết lập Build cho nền tảng mục tiêu (Windows Standalone 64-bit).
   - Tiến hành xuất bản ra file thực thi `.exe` độc lập cùng thư mục tài nguyên cần thiết.

---

## 2. CẤU TRÚC TÀI NGUYÊN & THƯ MỤC MỚI CỦA LAB 3

```text
Assets/
├── Audio/                  ⭐ [MỚI] Thư mục âm thanh chuẩn 16-bit PCM WAV
│   ├── bgm.wav             # Nhạc nền Chiptune 8-bit lặp tuần hoàn mượt mà
│   ├── jump.wav            # Âm thanh nhảy vút lên cao retro
│   ├── coin.wav            # Hiệu ứng chuông leng keng nhặt xu 2 tông (B5 -> E6)
│   ├── gameover.wav        # Hiệu ứng rơi trúng bẫy/quái (trầm buồn)
│   ├── win.wav             # Âm thanh khải hoàn chiến thắng nhặt chìa khóa
│   └── click.wav           # Âm thanh click nút giao diện UI
├── Sprites/
│   └── MovingPlatform.png  ⭐ [MỚI] Sprite pixel art sàn cơ khí nổi (96x32px)
├── Scenes/
│   ├── MainMenu.unity      ⭐ [MỚI] Scene Menu chính với Title, nút Play & Quit
│   └── SampleScene.unity   # Scene màn chơi chính (bổ sung Moving Platform & AudioManager)
├── _Scripts/
│   ├── AudioManager.cs     ⭐ [MỚI] Singleton quản lý 2 kênh Music/SFX
│   ├── MovingPlatform.cs   ⭐ [MỚI] Logic sàn di chuyển A-B & gán SetParent
│   ├── MainMenuController.cs ⭐ [MỚI] Xử lý nút PlayGame và QuitGame
│   ├── PlayerController.cs # [CẬP NHẬT] Kích hoạt AudioManager.instance.PlayJumpSFX()
│   ├── Coin.cs             # [CẬP NHẬT] Kích hoạt AudioManager.instance.PlayCoinSFX()
│   ├── GameManager.cs      # [CẬP NHẬT] Kích hoạt GameOver/Win SFX & quay về MainMenu
│   └── UIButtonAction.cs   # [CẬP NHẬT] Kích hoạt Click SFX
└── Editor/
    └── Lab3Automator.cs    ⭐ [MỚI] Menu công cụ 1-Click tự động hóa & Build .EXE
```

---

## 3. GIẢI THÍCH CHI TIẾT TOÀN BỘ MÃ NGUỒN C#

---

### 3.1. `AudioManager.cs` (Mẫu Singleton & 2 AudioSource BGM/SFX)

File mã nguồn: [`Assets/_Scripts/AudioManager.cs`](file:///c:/Users/RinHeo/Desktop/AgenticWorkFlow/My%20project%20%285%29/Assets/_Scripts/AudioManager.cs)

```csharp
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("--- NGUỒN PHÁT ÂM THANH (AUDIO SOURCES) ---")]
    public AudioSource musicSource; // Dành cho nhạc nền BGM (loop = true)
    public AudioSource sfxSource;   // Dành cho hiệu ứng âm thanh (PlayOneShot)

    [Header("--- BẢN NHẠC NỀN ---")]
    public AudioClip bgmClip;

    [Header("--- HIỆU ỨNG ÂM THANH ---")]
    public AudioClip jumpClip;
    public AudioClip coinClip;
    public AudioClip gameOverClip;
    public AudioClip winClip;
    public AudioClip clickClip;

    private void Awake()
    {
        // 1. Áp dụng Design Pattern Singleton
        if (instance == null)
        {
            instance = this;
            // 2. Không hủy AudioManager khi chuyển đổi giữa MainMenu và SampleScene
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

        SetupAudioSources();
    }

    private void Start()
    {
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
            musicSource.spatialBlend = 0f; // 2D Sound (âm lượng đều khắp 2 tai)
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = 0.85f;
            sfxSource.spatialBlend = 0f; // 2D Sound
        }
    }

    // Phát BGM có kiểm tra tránh ngắt quãng bài đang phát
    public void PlayBGM(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // Phát SFX bằng PlayOneShot: Cho phép nhiều âm thanh chồng lên nhau mà không bị ngắt
    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, volumeScale);
        }
    }

    public void PlayJumpSFX()       => PlaySFX(jumpClip);
    public void PlayCoinSFX()       => PlaySFX(coinClip);
    public void PlayGameOverSFX()   => PlaySFX(gameOverClip);
    public void PlayWinSFX()        => PlaySFX(winClip);
    public void PlayButtonClickSFX()=> PlaySFX(clickClip, 0.7f);
}
```

#### 🔍 Điểm sáng kiến trúc:
1. **Tại sao cần 2 `AudioSource` riêng biệt?**  
   Nếu chỉ dùng 1 `AudioSource`, khi gọi lệnh phát âm thanh ăn coin hoặc nhảy, lệnh `Play()` mới sẽ **lập tức ghi đè và dập tắt tiếng nhạc nền BGM đang phát**. Bằng cách phân tách `musicSource` (chế độ Loop) và `sfxSource` (chế độ `PlayOneShot`), nhạc nền luôn được phát liền mạch trong khi các hiệu ứng âm thanh tự do cất lên.
2. **Ưu điểm vượt trội của `PlayOneShot()`:**  
   `PlayOneShot` tạo ra một instance phát âm ảo. Khi Player ăn liên tục 3 đồng xu trong nửa giây, cả 3 tiếng leng keng sẽ cùng vang lên hài hòa chồng lên nhau (polyphonic) thay vì tiếng sau cắt đứt đuôi của tiếng trước.
3. **`DontDestroyOnLoad(gameObject)`:**  
   Giúp đối tượng `AudioManager` sống sót khi người chơi chuyển từ Menu chính sang màn chơi và ngược lại, giữ cho âm nhạc không bị khởi động lại giật cục.

---

### 3.2. `MovingPlatform.cs` (Sàn di chuyển tuần tra & Cơ chế SetParent)

File mã nguồn: [`Assets/_Scripts/MovingPlatform.cs`](file:///c:/Users/RinHeo/Desktop/AgenticWorkFlow/My%20project%20%285%29/Assets/_Scripts/MovingPlatform.cs)

```csharp
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("--- ĐIỂM MỐC DI CHUYỂN ---")]
    public Transform pointA;
    public Transform pointB;

    [Header("--- THIẾT LẬP TỐC ĐỘ ---")]
    public float speed = 2.5f;
    public float waitTime = 0.5f;

    private Transform currentTarget;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    private void Start()
    {
        currentTarget = (pointB != null) ? pointB : pointA;
    }

    private void Update()
    {
        if (pointA == null || pointB == null) return;

        // Nếu đang ở điểm dừng chờ đổi chiều
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f) isWaiting = false;
            return;
        }

        // Tịnh tiến vị trí sàn về điểm mốc hiện tại
        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // Khi sàn đã chạm đến điểm mốc
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.05f)
        {
            isWaiting = true;
            waitTimer = waitTime;
            // Đảo chiều mục tiêu A <-> B
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }

    // Khi Player tiếp đất lên mặt sàn di chuyển
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Kiểm tra pháp tuyến va chạm: Chỉ gắn làm con khi Player đứng TRÊN bề mặt sàn
            // (contacts[0].normal.y < -0.3f nghĩa là lực phản hồi hướng từ sàn lên trên bàn chân)
            if (collision.contactCount > 0 && collision.contacts[0].normal.y < -0.3f)
            {
                collision.transform.SetParent(transform);
            }
        }
    }

    // Khi Player nhảy ra hoặc rời khỏi sàn di chuyển
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Tách Player ra khỏi sàn, trả về toạ độ World độc lập
            if (collision.transform.parent == transform)
            {
                collision.transform.SetParent(null);
            }
        }
    }
}
```

#### 🔍 Bản chất vật lý của cơ chế `SetParent()`:
- **Vấn đề thực tế trong game 2D:** Nếu không gán Player làm con của sàn, khi sàn chuyển động sang phải theo vận tốc $V_s$, Player đứng trên sàn chỉ có ma sát tĩnh bề mặt. Do ma sát vật lý 2D dễ bị trượt vi mô (micro-sliding) và trọng lực kéo xuống liên tục, nhân vật sẽ dần bị tụt lại phía sau và rơi tõm xuống vực.
- **Giải pháp `collision.transform.SetParent(transform)`:**  
  Trong hệ toạ độ của Unity: $\vec{P}_{\text{world}} = \vec{P}_{\text{parent}} + \vec{P}_{\text{local}}$. Khi Player trở thành con của Platform, toạ độ tương đối $\vec{P}_{\text{local}}$ của Player so với tâm sàn được giữ nguyên (ví dụ $(0, 0.5, 0)$). Mỗi khi sàn di chuyển một khoảng $\Delta \vec{X}$, Player tự động được cộng thêm chính xác $\Delta \vec{X}$ đó mà không cần can thiệp vận tốc vật lý `linearVelocity`.
- **Kiểm tra va chạm pháp tuyến (`contacts[0].normal.y < -0.3f`):**  
  Đây là điểm tinh chỉnh rất chuyên nghiệp: Nếu Player nhảy từ dưới lên và đụng đầu vào gầm dưới của sàn, Player sẽ không bị dính làm con của sàn, chỉ khi đáp chân lên trên mặt sàn thì mới kích hoạt gắn `SetParent`.

---

### 3.3. `MainMenuController.cs` (Điều khiển Menu chính & Quản lý Scene)

File mã nguồn: [`Assets/_Scripts/MainMenuController.cs`](file:///c:/Users/RinHeo/Desktop/AgenticWorkFlow/My%20project%20%285%29/Assets/_Scripts/MainMenuController.cs)

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("--- TÊN SCENE CHƠI GAME ---")]
    public string gameSceneName = "SampleScene";

    // Bắt đầu chơi game (Play)
    public void PlayGame()
    {
        Time.timeScale = 1f;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayButtonClickSFX();
        }

        if (Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            // Tải theo chỉ số Index 1 trong Build Settings
            SceneManager.LoadScene(1);
        }
    }

    // Thoát game (Quit)
    public void QuitGame()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayButtonClickSFX();
        }

        Debug.Log("<color=yellow><b>[Main Menu]</b> Đang thoát trò chơi...</color>");
        
        // Thoát ứng dụng khi chạy bản build .exe độc lập
        Application.Quit();

#if UNITY_EDITOR
        // Dừng chế độ chạy thử trong Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
```

---

### 3.4. Tích hợp âm thanh vào PlayerController, Coin, GameManager, UIButtonAction

1. **Trong [`PlayerController.cs`](file:///c:/Users/RinHeo/Desktop/AgenticWorkFlow/My%20project%20%285%29/Assets/_Scripts/PlayerController.cs#L118-L125):**
   ```csharp
   if (jumpPressed && isGrounded)
   {
       rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
       if (AudioManager.instance != null)
       {
           AudioManager.instance.PlayJumpSFX(); // Phát âm thanh nhảy
       }
   }
   ```
2. **Trong [`Coin.cs`](file:///c:/Users/RinHeo/Desktop/AgenticWorkFlow/My%20project%20%285%29/Assets/_Scripts/Coin.cs#L35-L45):**
   ```csharp
   if (collision.CompareTag("Player"))
   {
       if (AudioManager.instance != null)
       {
           AudioManager.instance.PlayCoinSFX(); // Phát âm thanh ăn coin
       }
       if (GameManager.instance != null)
       {
           GameManager.instance.AddScore(scoreValue);
       }
       Destroy(gameObject);
   }
   ```
3. **Trong [`GameManager.cs`](file:///c:/Users/RinHeo/Desktop/AgenticWorkFlow/My%20project%20%285%29/Assets/_Scripts/GameManager.cs#L72-L95):**
   - Khi Game Over: `AudioManager.instance.PlayGameOverSFX();`
   - Khi Win Game: `AudioManager.instance.PlayWinSFX();`
   - Nút Main Menu: `SceneManager.LoadScene("MainMenu");`
4. **Trong [`UIButtonAction.cs`](file:///c:/Users/RinHeo/Desktop/AgenticWorkFlow/My%20project%20%285%29/Assets/_Scripts/UIButtonAction.cs#L25-L35):**
   - Khi click nút: `AudioManager.instance.PlayButtonClickSFX();`

---

### 3.5. `Lab3Automator.cs` (Công cụ tự động hóa toàn diện 1-Click Setup & Build .EXE)

File mã nguồn: [`Assets/Editor/Lab3Automator.cs`](file:///c:/Users/RinHeo/Desktop/AgenticWorkFlow/My%20project%20%285%29/Assets/Editor/Lab3Automator.cs)

Menu Editor tích hợp sẵn trên thanh công cụ Unity:
- `Lab 3/Chạy Cài Đặt Tự Động Toàn Bộ Lab 3 (One-Click Setup)`: Tự động khởi tạo và liên kết toàn bộ tài nguyên chỉ với 1 cú click chuột.
- `Lab 3/5. Xuất Bản Game Độc Lập (.EXE Standalone Build)`: Tự động gọi `BuildPipeline.BuildPlayer` tạo file `Build/GameLab3.exe` sẵn sàng đem đi nộp bài hoặc trình chiếu cho Giảng viên.

---

## 4. CÁC KIẾN THỨC CỐT LÕI VỀ LẬP TRÌNH GAME UNITY ĐÃ ÁP DỤNG

### 1. Hệ thống Âm thanh trong Unity (Audio System Architecture)
- **`AudioListener`:** Đóng vai trò như "đôi tai" của người chơi trong thế giới ảo. Mỗi Scene chỉ được phép có duy nhất 1 AudioListener (thường gắn trên Main Camera).
- **`AudioSource`:** Đóng vai trò như "chiếc loa" phát ra âm thanh.
- **`AudioClip`:** File dữ liệu sóng âm thanh thực tế (.wav, .mp3, .ogg).
- **2D Sound (`spatialBlend = 0f`):** Âm thanh phát đều ra 2 kênh stereo trái/phải không phụ thuộc khoảng cách không gian (phù hợp với game 2D Platformer). Khác với 3D Sound (`spatialBlend = 1f`) bị suy giảm âm lượng theo khoảng cách.
- **Streaming vs Decompress on Load:**
  - Nhạc nền BGM thời lượng dài (vài phút): Cấu hình nạp luồng (`Load Type = Streaming`) để tiết kiệm dung lượng RAM.
  - Hiệu ứng SFX ngắn (dưới 2 giây): Cấu hình `Decompress On Load` để âm thanh được giải nén sẵn trong RAM, phát ra ngay lập tức với độ trễ bằng 0ms khi người chơi bấm nút.

### 2. Mô hình phân cấp Transform (Transform Hierarchy & Parenting)
- Trong Unity, mối quan hệ Cha - Con (`Parent - Child`) thiết lập một ma trận chuyển đổi không gian liên kết:
  $$\mathbf{M}_{\text{world}} = \mathbf{M}_{\text{parent}} \times \mathbf{M}_{\text{local}}$$
- Nhờ cơ chế này, toàn bộ chuyển động tịnh tiến, quay hay tỷ lệ của vật thể cha sẽ tự động áp dụng lên vật thể con một cách tự nhiên mà không cần viết thêm mã xử lý vị trí.

### 3. Vòng đời Scene & Build Pipeline
- **`SceneManager.LoadScene()`:** Dọn dẹp tài nguyên của Scene hiện tại khỏi RAM và nạp cấu trúc cây GameObject của Scene mới.
- **`EditorBuildSettingsScene`:** Đăng ký các Scene được đóng gói vào file nhị phân. Scene ở **Index 0** chính là Scene đầu tiên được nạp vào bộ nhớ khi mở file thực thi `.exe`.
- **`Application.Quit()`:** Gửi tín hiệu đóng tiến trình (Process Exit) về hệ điều hành Windows khi người chơi muốn thoát game.

---

## 5. HƯỚNG DẪN TRẢI NGHIỆM & TEST LAB 3

### Cách 1: Test trực tiếp trong Unity Editor
1. Mở Unity Editor, nhìn lên thanh Menu trên cùng:
   - Bấm vào menu **`Lab 3`** ➜ Chọn **`Chạy Cài Đặt Tự Động Toàn Bộ Lab 3 (One-Click Setup)`**.
2. Unity sẽ tự động mở màn hình **`MainMenu.unity`**.
3. Bấm nút **Play (▶)**:
   - Thưởng thức nhạc nền BGM chiptune 8-bit sống động.
   - Bấm nút **BẮT ĐẦU CHƠI (PLAY)** ➜ Chuyển cảnh vào màn chơi chính `SampleScene`.
   - Bấm **Space** để nghe tiếng nhảy vui nhộn.
   - Chạy qua ăn xu để nghe tiếng chuông leng keng `coin.wav`.
   - Nhảy lên tấm sàn nổi `MovingPlatform` ➜ Cảm nhận sàn trượt sang bờ bên kia mà nhân vật không hề bị trượt chân.
   - Nhảy sang bờ đất, nhặt chìa khóa vàng ➜ Màn hình **YOU WIN!** xuất hiện kèm âm thanh chiến thắng.
   - Bấm nút **Menu Chính** ➜ Quay trở lại màn hình Menu đầu game!

### Cách 2: Chạy trực tiếp từ bản Build độc lập (.EXE)
1. Trong Unity Editor, chọn menu **`Lab 3`** ➜ **`5. Xuất Bản Game Độc Lập (.EXE Standalone Build)`**.
2. Mở thư mục **`Build/`** trong dự án ➜ Nhấp đúp chuột vào file **`GameLab3.exe`**.
3. Game chạy mượt mà độc lập ở chế độ Full HD trên Windows!

---

## 6. KỊCH BẢN THUYẾT TRÌNH VỚI GIẢNG VIÊN

> **Thời lượng gợi ý:** 2 - 3 phút  
> **Phong thái:** Tự tin, làm chủ toàn bộ mã nguồn và giải thích rõ ràng các quyết định thiết kế kỹ thuật.

### 🎤 Lời thoại mẫu khi báo cáo:

> *"Kính thưa Thầy/Cô, hôm nay em xin phép trình bày kết quả thực hiện **Lab 3** môn Phát triển Game 2D trong Unity.*
>
> *Sau khi đã hoàn thiện cơ chế điều khiển vật lý ở Lab 1 và thiết kế bản đồ, camera, quái vật ở Lab 2, trong Lab 3 này em đã hoàn tất 4 trụ cột quan trọng để đưa trò chơi thành một sản phẩm hoàn chỉnh:*
>
> *1. **Thứ nhất - Hệ thống Âm thanh tập trung (AudioManager):**  
> Em xây dựng `AudioManager` theo mô hình Singleton, sử dụng 2 kênh `AudioSource` độc lập. Kênh BGM được đặt `loop = true` để phát nhạc nền chiptune xuyên suốt, còn kênh SFX sử dụng phương thức `PlayOneShot()` để phát các âm thanh nhảy và ăn tiền xu mà không bao giờ bị ngắt hay nuốt tiếng. Đồng thời em dùng `DontDestroyOnLoad` để âm thanh duy trì mượt mà giữa các Scene.*
>
> *2. **Thứ hai - Sàn di chuyển cơ khí (Moving Platform):**  
> Sàn tự động trượt tuần tra giữa 2 điểm mốc A và B bằng `Vector2.MoveTowards`. Để giải quyết bài toán chống trượt chân trong vật lý 2D, em sử dụng cơ chế `collision.transform.SetParent(transform)` khi nhân vật tiếp đất lên sàn, giúp Player trở thành đối tượng con và chuyển động đồng tốc tuyệt đối cùng sàn. Khi nhảy ra, em gọi `SetParent(null)` để hoàn trả trạng thái độc lập.*
>
> *3. **Thứ ba - Giao diện Menu chính (Main Menu Scene):**  
> Em thiết kế Scene `MainMenu` riêng biệt bằng TextMeshPro hiện đại với hiệu ứng nút bấm, hỗ trợ chuyển cảnh vào game bằng `SceneManager.LoadScene` và thoát game dứt khoát bằng `Application.Quit()`.*
>
> *4. **Cuối cùng - Đóng gói và Xuất bản (.EXE Build):**  
> Em đã cấu hình Build Settings với `MainMenu` ở Index 0 và `SampleScene` ở Index 1, đồng thời viết script tự động biên dịch toàn bộ game ra file thực thi `GameLab3.exe` độc lập chạy trực tiếp trên Windows.*
>
> *Sau đây em xin phép được chạy thử trực tiếp cả trong Editor và bản build .exe để Thầy/Cô cùng trải nghiệm ạ!"*

---

## 7. BỘ 10 CÂU HỎI VẤN ĐÁP (ORAL EXAM) LAB 3 KÈM ĐÁP ÁN CHUẨN

Dưới đây là 10 câu hỏi giảng viên hay hỏi nhất khi nghiệm thu Lab 3, cùng câu trả lời ngắn gọn, chuẩn xác và thuyết phục nhất:

---

### ❓ Câu 1: Tại sao trong AudioManager em phải chia làm 2 AudioSource riêng biệt mà không dùng 1 cái?
- **Đáp án chuẩn:**  
  Một Component `AudioSource` chỉ có thể phát một âm thanh tại một thời điểm nếu dùng hàm `Play()`. Nếu dùng chung 1 AudioSource, mỗi khi Player nhảy hoặc ăn coin, âm thanh hiệu ứng đó sẽ **lập tức ngắt ngang và tắt luôn nhạc nền BGM đang phát**. Vì vậy, em chia thành `musicSource` (chế độ Loop cho nhạc nền) và `sfxSource` (chế độ `PlayOneShot` cho hiệu ứng âm thanh) để hai luồng âm thanh hoạt động độc lập, không ảnh hưởng lẫn nhau.

---

### ❓ Câu 2: Phương thức `PlayOneShot()` khác gì so với phương thức `Play()` thông thường?
- **Đáp án chuẩn:**  
  - Hàm `Play()` sẽ phát AudioClip đang được gán trong biến `AudioSource.clip`. Nếu gọi `Play()` lần nữa khi âm thanh trước chưa dứt, âm thanh cũ sẽ bị ngắt và phát lại từ đầu.
  - Hàm `PlayOneShot(clip)` nhận trực tiếp một AudioClip làm tham số và cho phép **nhiều âm thanh cùng loại hoặc khác loại phát đè lên nhau cùng lúc** (polyphonic). Ví dụ khi Player ăn liên tiếp 5 đồng xu trong 1 giây, cả 5 tiếng chuông leng keng đều vang lên trọn vẹn mà không tiếng nào bị ngắt quãng.

---

### ❓ Câu 3: Làm thế nào để giải quyết tình trạng Player bị trượt chân hoặc rơi khỏi sàn khi sàn di chuyển ngang?
- **Đáp án chuẩn:**  
  Trong hàm `OnCollisionEnter2D`, khi phát hiện Player bước lên mặt trên của sàn, em gọi lệnh `collision.transform.SetParent(transform)`. Lệnh này gán Player làm đối tượng con (`Child`) của sàn di chuyển. Nhờ đó, ma trận biến đổi toạ độ của sàn sẽ tự động áp dụng lên toạ độ của Player, khiến Player di chuyển đồng tốc cùng sàn. Khi Player nhảy ra khỏi sàn, trong `OnCollisionExit2D` em gọi `collision.transform.SetParent(null)` để tách Player trở lại trạng thái độc lập.

---

### ❓ Câu 4: Nếu Player nhảy từ dưới lên và chạm đầu vào gầm của sàn di chuyển thì có bị dính vào sàn không? Em xử lý thế nào?
- **Đáp án chuẩn:**  
  Không bị dính ạ. Em đã bổ sung kiểm tra pháp tuyến va chạm: `collision.contacts[0].normal.y < -0.3f`. Khi va chạm xảy ra, vector pháp tuyến thể hiện hướng lực tác động. Điều kiện này đảm bảo rằng lực tác động phải hướng từ mặt trên của sàn lên chân nhân vật (tức là Player đang đứng trên mặt sàn) thì mới kích hoạt `SetParent`. Nếu Player cụng đầu vào đáy dưới sàn, điều kiện này sai nên không bị gán làm con.

---

### ❓ Câu 5: Lệnh `DontDestroyOnLoad(gameObject)` trong AudioManager có tác dụng gì?
- **Đáp án chuẩn:**  
  Thông thường khi Unity tải một Scene mới bằng `SceneManager.LoadScene()`, toàn bộ các GameObject ở Scene cũ sẽ bị xóa khỏi bộ nhớ RAM. Lệnh `DontDestroyOnLoad` yêu cầu Unity đưa GameObject `AudioManager` vào một phân vùng bộ nhớ đặc biệt không bị xóa khi đổi Scene. Nhờ đó, nhạc nền BGM và các thiết lập âm thanh không bị khởi động lại hay gián đoạn khi người chơi chuyển từ `MainMenu` sang `SampleScene`.

---

### ❓ Câu 6: Làm thế nào để đảm bảo khi game khởi động, màn hình Main Menu luôn được mở đầu tiên chứ không phải màn chơi chính?
- **Đáp án chuẩn:**  
  Trong cửa sổ **Build Settings** (`File -> Build Settings`), Unity quy định Scene nào nằm ở chỉ số **Index 0** trên danh sách `Scenes In Build` sẽ là Scene đầu tiên được tải khi game khởi chạy. Em đã cấu hình đưa `MainMenu.unity` lên vị trí Index 0 và `SampleScene.unity` ở vị trí Index 1.

---

### ❓ Câu 7: Khi ấn nút "Quit" trong Main Menu, tại sao phải dùng cả `Application.Quit()` và chỉ thị tiền xử lý `#if UNITY_EDITOR`?
- **Đáp án chuẩn:**  
  Lệnh `Application.Quit()` chỉ có tác dụng khi game đã được Build ra file thực thi độc lập (.exe trên Windows hoặc ứng dụng trên điện thoại). Lệnh này hoàn toàn vô tác dụng khi đang chạy thử trong Unity Editor. Vì vậy, em dùng cặp chỉ thị tiền xử lý `#if UNITY_EDITOR UnityEditor.EditorApplication.isPlaying = false; #endif` để khi test trong Editor, Unity tự động tắt chế độ Play mode, giúp kiểm thử hành vi thoát game một cách trực quan.

---

### ❓ Câu 8: Sự khác biệt giữa cấu hình âm thanh "Streaming" và "Decompress On Load" trong Unity là gì?
- **Đáp án chuẩn:**  
  - **Decompress On Load:** Âm thanh được giải nén sẵn toàn bộ vào bộ nhớ RAM ngay khi Scene nạp xong. Cách này tốn thêm một chút RAM nhưng thời gian phản hồi cực nhanh (0ms trễ), rất phù hợp với các hiệu ứng SFX ngắn như tiếng nhảy, ăn coin.
  - **Streaming:** File âm thanh được lưu ở dạng nén trên ổ cứng và được đọc nạp dần theo luồng khi phát. Cách này tiết kiệm tối đa dung lượng RAM, rất thích hợp cho các bài nhạc nền BGM dung lượng lớn kéo dài nhiều phút.

---

### ❓ Câu 9: Đối với âm thanh game 2D Platformer, thông số `Spatial Blend` trong AudioSource nên đặt bằng bao nhiêu? Tại sao?
- **Đáp án chuẩn:**  
  Thông số `Spatial Blend` nên được đặt bằng **`0`** (chế độ hoàn toàn **2D**). Ở chế độ 2D, âm thanh sẽ phát đều ra cả hai tai (stereo) với âm lượng ổn định không phụ thuộc vào khoảng cách hay góc quay giữa nguồn âm và AudioListener của Camera. Nếu đặt bằng 1 (chế độ 3D), âm thanh sẽ bị nhỏ đi hoặc lệch sang một bên tai khi nhân vật chạy ra xa vị trí gốc toạ độ của nguồn âm.

---

### ❓ Câu 10: Sau khi người chơi bị Game Over hoặc Win, nếu bấm nút "Main Menu" thì biến `Time.timeScale` cần xử lý như thế nào?
- **Đáp án chuẩn:**  
  Khi Game Over hoặc Win, game đã bị đóng băng bằng lệnh `Time.timeScale = 0f` để dừng mọi chuyển động vật lý. Vì vậy, trước khi gọi `SceneManager.LoadScene("MainMenu")`, ta bắt buộc phải đặt lại `Time.timeScale = 1f;`. Nếu quên bước này, khi chuyển về Menu hoặc bắt đầu ván chơi mới, thời gian toàn game vẫn bị đóng băng ở mức 0, dẫn tới nhân vật không thể di chuyển và các animation không thể chạy.
