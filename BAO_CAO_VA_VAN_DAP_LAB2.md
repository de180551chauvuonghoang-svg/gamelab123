# 🎓 TÀI LIỆU BÁO CÁO, GIẢI THÍCH MÃ NGUỒN & BỘ CÂU HỎI VẤN ĐÁP LAB 2 (UNITY 2D PLATFORMER)

> **Dự án:** Game 2D Platformer (Lab 2)  
> **Phiên bản Engine:** Unity 6 (6000.3.23f1)  
> **Các tính năng trọng tâm:** Camera Follow (Cinemachine), Tilemap & Composite Collider 2D, Coin Prefab, Singleton GameManager, Enemy Patrol A-B, Trap, Key Win, UI TextMeshPro.  

---

## 📑 MỤC LỤC
1. [Tổng quan dự án & Yêu cầu bài Lab 2](#1-tổng-quan-dự-án--yêu-cầu-bài-lab-2)
2. [Cấu trúc tài nguyên & Thư mục mới của Lab 2](#2-cấu-trúc-tài-nguyên--thư-mục-mới-của-lab-2)
3. [Giải thích chi tiết toàn bộ mã nguồn C#](#3-giải-thích-chi-tiết-toàn-bộ-mã-nguồn-c)
   - [3.1. GameManager.cs (Mẫu Singleton & Quản lý màn chơi)](#31-gamemanagercs-mẫu-singleton--quản-lý-màn-chơi)
   - [3.2. Coin.cs (Hệ thống thu thập tiền xu)](#32-coincs-hệ-thống-thu-thập-tiền-xu)
   - [3.3. Hazard.cs (Bẫy gai) & EnemyPatrol.cs (Quái tuần tra A-B)](#33-hazardcs-bẫy-gai--enemypatrolcs-quái-tuần-tra-a-b)
   - [3.4. KeyItem.cs (Chìa khóa chiến thắng)](#34-keyitemcs-chìa-khóa-chiến-thắng)
   - [3.5. CameraFollow.cs (Camera theo dõi nhân vật mượt mà)](#35-camerafollowcs-camera-theo-dõi-nhân-vật-mượt-mà)
4. [Các kiến thức cốt lõi về lập trình Game 2D đã áp dụng](#4-các-kiến-thức-cốt-lõi-về-lập-trình-game-2d-đã-áp-dụng)
5. [Hướng dẫn trải nghiệm & Test Lab 2](#5-hướng-dẫn-trải-nghiệm--test-lab-2)
6. [Kịch bản thuyết trình với Giảng viên](#6-kịch-bản-thuyết-trình-với-giảng-viên)
7. [Bộ 10 câu hỏi vấn đáp (Oral Exam) Lab 2 kèm đáp án chuẩn](#7-bộ-10-câu-hỏi-vấn-đáp-oral-exam-lab-2-kèm-đáp-án-chuẩn)

---

## 1. TỔNG QUAN DỰ ÁN & YÊU CẦU BÀI LAB 2

Sau khi hoàn thành phần điều khiển vật lý cơ bản ở Lab 1, **Lab 2** nâng cấp tựa game thành một trò chơi hoàn chỉnh có vòng lặp gameplay (Gameplay Loop: Thử thách ➔ Phần thưởng ➔ Thắng/Thua):

1. **Camera điều khiển thông minh:** Camera tự động bám theo Player khi di chuyển khắp bản đồ rộng lớn.
2. **Thiết kế bản đồ Tilemap đa lớp:**
   - Phân chia các lớp: `Background`, `Ground`, `Foreground`.
   - Tối ưu hóa va chạm bằng `Tilemap Collider 2D` kết hợp `Composite Collider 2D` (tránh lỗi kẹt mép gạch).
3. **Logic Game, Điểm số & Giao diện (UI):**
   - Đóng gói đồng xu thành **Prefab** để tái sử dụng nhiều nơi.
   - Bắt sự kiện va chạm `OnTriggerEnter2D` để tính điểm, cập nhật lên TextMeshPro UI thời gian thực và hủy đối tượng bằng `Destroy()`.
4. **Bẫy (Trap), Quái (Enemy) & Game Over:**
   - Xây dựng quái vật tự động đi tuần tra qua lại giữa điểm A và điểm B, tự động quay mặt theo hướng đi.
   - Chạm vào bẫy hoặc quái vật sẽ kích hoạt `Game Over`, hiển thị bảng thông báo kèm 2 nút: `Play Again` (chơi lại) và `Main Menu`.
5. **Chìa khóa (Key) & Chiến thắng:**
   - Đặt vật phẩm chìa khóa ở cuối màn; khi Player thu thập sẽ kích hoạt màn hình `YOU WIN!`.

---

## 2. CẤU TRÚC TÀI NGUYÊN & THƯ MỤC MỚI CỦA LAB 2

```text
Assets/
├── Prefabs/                ⭐ [MỚI] Chứa các đối tượng mẫu tái sử dụng
│   └── Coin.prefab         # Mẫu đồng xu (gồm Sprite, CircleCollider2D Trigger, Coin.cs)
├── Sprites/                # Bổ sung các Sprite đồ họa mới
│   ├── Coin.png            # Sprite đồng xu vàng
│   ├── Enemy.png           # Sprite quái vật màu đỏ có sừng và mắt
│   ├── Trap.png            # Sprite hàng chông sắt nhọn
│   └── Key.png             # Sprite chìa khóa vàng
├── _Scripts/               # Bổ sung các script logic gameplay
│   ├── GameManager.cs      # Quản lý điểm số, UI và trạng thái game
│   ├── Coin.cs             # Logic nhặt xu và cộng điểm
│   ├── Hazard.cs           # Logic bẫy gai gây Game Over
│   ├── EnemyPatrol.cs      # Quái vật tuần tra tự động A - B
│   ├── KeyItem.cs          # Chìa khóa chiến thắng
│   ├── CameraFollow.cs     # Camera bám theo Player
│   └── UIButtonAction.cs   # Xử lý sự kiện click nút UI
└── Editor/
    └── Lab2Automator.cs    # Tool tự động hóa toàn bộ thiết lập Lab 2
```

---

## 3. GIẢI THÍCH CHI TIẾT TOÀN BỘ MÃ NGUỒN C#

---

### 3.1. `GameManager.cs` (Mẫu Singleton & Quản lý màn chơi)

```csharp
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Biến tĩnh lưu instance duy nhất

    [Header("--- ĐIỂM SỐ & GIAO DIỆN UI ---")]
    public TextMeshProUGUI scoreText;
    private int score = 0;

    [Header("--- BẢNG THÔNG BÁO ---")]
    public GameObject gameOverPanel;
    public GameObject winPanel;

    private bool isGameEnded = false;

    private void Awake()
    {
        // Triển khai mẫu thiết kế Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 1f; // Khôi phục thời gian bình thường khi bắt đầu
        isGameEnded = false;
        UpdateScoreUI();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    public void AddScore(int amount)
    {
        if (isGameEnded) return;
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Dừng toàn bộ chuyển động trong game
    }

    public void WinGame()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f; // Dừng game khi chiến thắng
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f; // Khôi phục thời gian trước khi tải lại màn
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        if (Application.CanStreamedLevelBeLoaded("MainMenu"))
            SceneManager.LoadScene("MainMenu");
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
```

#### 💡 Phân tích chuyên sâu:
1. **Mẫu thiết kế Singleton (`public static GameManager instance`):**
   - Cho phép bất kỳ script nào khác (như `Coin.cs`, `Hazard.cs`, `EnemyPatrol.cs`) có thể truy cập ngay vào `GameManager` thông qua cú pháp ngắn gọn: `GameManager.instance.AddScore(1)` mà **không cần dùng `FindObjectOfType`** (vốn rất chậm).
2. **Cơ chế dừng thời gian (`Time.timeScale = 0f`):**
   - Khi Game Over hoặc Chiến thắng, ta đặt `Time.timeScale = 0f`. Lúc này mọi hàm tính toán có dính đến `Time.deltaTime`, vận tốc `Rigidbody 2D` và diễn hoạt `Animator` đều lập tức đóng băng, tạo cảm giác game tạm dừng hoàn hảo.
   - **Lưu ý quan trọng:** Trong hàm `PlayAgain()` và `LoadMainMenu()`, bắt buộc phải gán `Time.timeScale = 1f` trước khi đổi màn, nếu không khi sang màn mới game sẽ bị "đóng băng" vĩnh viễn.

---

### 3.2. `Coin.cs` (Hệ thống thu thập tiền xu)

```csharp
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int scoreValue = 1;
    public float rotateSpeed = 100f;

    private void Update()
    {
        // Hiệu ứng tự xoay nhẹ quanh trục Y để tạo chiều sâu 3D giả lập
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng chạm vào có mang Tag là "Player" hay không
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(scoreValue);
            }

            // Xóa đối tượng Coin khỏi bộ nhớ và màn chơi
            Destroy(gameObject);
        }
    }
}
```

#### 💡 Phân tích chuyên sâu:
- **`OnTriggerEnter2D`:** Coin được bật thuộc tính `Is Trigger = true`. Điều này biến Collider của nó thành một "vùng cảm ứng xuyên qua được". Nhân vật chạy xuyên qua coin mượt mà chứ không bị vấp ngã hay dội ngược lại như khi va chạm với vật cứng.
- **`collision.CompareTag("Player")`:** Kiểm tra tag bằng hàm `CompareTag()` hiệu quả và tối ưu bộ nhớ hơn nhiều so với việc so sánh chuỗi `collision.tag == "Player"` (vì tránh sinh ra rác bộ nhớ Garbage Collection).
- **`Destroy(gameObject)`:** Giải phóng hoàn toàn đối tượng khỏi bộ nhớ RAM để đảm bảo hiệu năng.

---

### 3.3. `Hazard.cs` (Bẫy gai) & `EnemyPatrol.cs` (Quái tuần tra A-B)

#### `Hazard.cs`:
```csharp
using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
        }
    }
}
```

#### `EnemyPatrol.cs`:
```csharp
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2.5f;

    private Transform currentTarget;
    private bool isFacingRight = true;

    private void Start()
    {
        currentTarget = (pointB != null) ? pointB : pointA;
    }

    private void Update()
    {
        if (currentTarget == null) return;

        // Tịnh tiến vị trí dần dần về phía điểm đích
        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // Khi đã đến rất gần điểm đích (< 0.05 đơn vị) -> Đổi chiều
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.05f)
        {
            if (currentTarget == pointB)
            {
                currentTarget = pointA;
                Flip(false); // Quay sang trái
            }
            else
            {
                currentTarget = pointB;
                Flip(true);  // Quay sang phải
            }
        }
    }

    private void Flip(bool faceRight)
    {
        isFacingRight = faceRight;
        Vector3 scale = transform.localScale;
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance != null) GameManager.instance.GameOver();
        }
    }
}
```

#### 💡 Phân tích chuyên sâu:
- **Hàm `Vector2.MoveTowards(current, target, maxDistanceDelta)`:** Tính toán tọa độ tiếp theo trên đoạn thẳng nối giữa vị trí hiện tại và đích đến, với tốc độ ổn định không vượt quá `speed * Time.deltaTime`.
- **Hàm `Vector2.Distance()`:** Đo khoảng cách Euclidean giữa quái và điểm mốc. Khi khoảng cách đủ nhỏ (< 0.05m), hoán đổi mục tiêu `currentTarget` giữa `pointA` và `pointB` để tạo vòng lặp tuần tra vô tận.

---

### 3.4. `KeyItem.cs` (Chìa khóa chiến thắng)

```csharp
using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public float floatSpeed = 3f;
    public float floatHeight = 0.15f;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Hiệu ứng hàm lượng giác Sin tạo chuyển động nhấp nhô bập bồng
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.WinGame();
            }
            Destroy(gameObject);
        }
    }
}
```
- **Hàm lượng giác `Mathf.Sin()`:** Sinh ra dao động điều hòa trơn tru từ `-1` đến `+1` theo thời gian `Time.time`, làm cho chiếc chìa khóa lơ lửng nhấp nhô thu hút ánh nhìn người chơi.

---

### 3.5. `CameraFollow.cs` (Camera theo dõi nhân vật mượt mà)

```csharp
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0f, 1f, -10f);

    private void LateUpdate()
    {
        if (target == null)
        {
            GameObject playerGo = GameObject.FindWithTag("Player");
            if (playerGo != null) target = playerGo.transform;
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        // Nội suy mượt mà từ vị trí hiện tại sang vị trí mong muốn
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
```

#### 💡 Tại sao đặt Camera trong `LateUpdate()`?
- `Update()` và `FixedUpdate()` là nơi Player tính toán vị trí di chuyển.
- `LateUpdate()` luôn luôn chạy **sau cùng** trong một khung hình, sau khi mọi chuyển động của nhân vật đã hoàn tất. Đặt code bám camera trong `LateUpdate()` giúp camera không bao giờ bị hiện tượng rung giật (jittering/stuttering).
- **Hàm `Vector3.Lerp` (Linear Interpolation):** Nội suy tuyến tính giúp camera đuổi theo nhân vật một cách từ tốn, mượt mà thay vì giật cục.

---

## 4. CÁC KIẾN THỨC CỐT LÕI VỀ LẬP TRÌNH GAME 2D ĐÃ ÁP DỤNG

| Khái niệm | Ý nghĩa kỹ thuật | Ứng dụng cụ thể trong Lab 2 |
| :--- | :--- | :--- |
| **Prefab System** | Đối tượng mẫu định nghĩa sẵn cấu trúc, component và thuộc tính, lưu trữ dưới dạng asset để nhân bản hàng loạt. | Tạo `Coin.prefab` lưu vào `Assets/Prefabs/` để đặt nhiều vị trí trên bản đồ mà chỉ cần chỉnh sửa ở 1 nơi. |
| **Trigger Collider vs Collision** | Trigger là vùng cảm biến nhận sự kiện xuyên qua; Collision là vật cản cứng chặn đối tượng lại. | Coin, Trap, Key dùng `Is Trigger = true` (xuyên qua kích hoạt sự kiện); Tường, Sàn dùng `Is Trigger = false` (chặn đứng). |
| **Composite Collider 2D** | Hợp nhất nhiều Collider riêng lẻ của các ô Tilemap thành một đường bao va chạm đa giác duy nhất. | Gán vào Tilemap `Ground`, bật `Used By Composite` và đặt `Rigidbody2D = Static` để nhân vật không bị vấp chân giữa các khe gạch. |
| **Singleton Pattern** | Đảm bảo một lớp chỉ có một thể hiện duy nhất và cung cấp điểm truy cập toàn cục tới thể hiện đó. | `GameManager.instance` quản lý điểm số và trạng thái game. |
| **TextMeshPro UI** | Hệ thống hiển thị văn bản thế hệ mới của Unity dựa trên kỹ thuật Signed Distance Field (SDF). | `ScoreText`, tiêu đề `GAME OVER`, `YOU WIN!` chữ sắc nét ở mọi độ phân giải màn hình. |
| **Canvas Scaler** | Tự động co giãn giao diện UI theo kích thước và tỷ lệ màn hình (Resolution/Aspect Ratio). | Cài đặt chế độ `Scale With Screen Size` (1920x1080) giúp UI không bị vỡ trên các màn hình khác nhau. |

---

## 5. HƯỚNG DẪN TRẢI NGHIỆM & TEST LAB 2

### ⚡ Cách chạy Setup Lab 2:
1. Mở cửa sổ Unity Editor.
2. Trên thanh menu cao nhất của Unity, bấm vào: **`Lab 2`** ➜ Chọn **`Chạy Cài Đặt Tự Động Lab 2 (One-Click Setup)`**.
3. Bấm nút **Play (▶)** để bắt đầu chơi thử.

### 📋 Checklist kiểm tra khi chơi:
- [ ] **Camera Follow:** Di chuyển nhân vật sang trái / sang phải ➜ Camera di chuyển êm ái theo sau nhân vật, mở rộng tầm nhìn toàn cảnh.
- [ ] **Ăn Coin & Điểm số:** Chạy qua các đồng xu vàng ➜ Đồng xu biến mất, chỉ số trên góc màn hình tăng từ `Score: 0` lên `Score: 1, 2, 3...`.
- [ ] **Bẫy gai (Trap):** Cho nhân vật nhảy rơi trúng hàng gai nhọn ➜ Màn hình lập tức hiện bảng **GAME OVER**, dừng thời gian chuyển động.
- [ ] **Nút Play Again:** Bấm nút `Thử Lại` trên bảng Game Over ➜ Màn chơi lập tức được load lại từ đầu, điểm số reset về 0.
- [ ] **Quái tuần tra (Enemy Patrol):** Quan sát quái vật màu đỏ tự động đi từ PointA sang PointB và tự quay đầu lật mặt khi chạm mốc. Nếu Player va vào quái ➜ Game Over.
- [ ] **Chìa khóa chiến thắng (Key):** Nhảy vượt qua tường và quái vật, chạy về cuối đường nhặt chiếc chìa khóa vàng ➜ Màn hình hiện bảng **YOU WIN!** rực rỡ!

---

## 6. KỊCH BẢN THUYẾT TRÌNH VỚI GIẢNG VIÊN

Khi lên bảng báo cáo Lab 2, bạn hãy trình bày theo 3 phần ngắn gọn, súc tích (khoảng 3 phút):

### 🎤 Phần 1: Giới thiệu cấu trúc màn chơi & Camera (1 phút)
> *"Thưa Thầy/Cô, trong Lab 2 em đã mở rộng thế giới game với bản đồ dài hơn và hệ thống Camera bám theo Player. Về bản đồ, em thiết lập hệ thống Tilemap phân tách các lớp Background, Ground và Foreground. Để tối ưu hóa va chạm cho địa hình đất, em kết hợp `Tilemap Collider 2D` với `Composite Collider 2D` và chuyển Rigidbody 2D sang chế độ `Static`. Điều này gộp các khối gạch thành một đường biên va chạm phẳng duy nhất, triệt tiêu lỗi nhân vật bị vấp chân ở các khe nối gạch. Camera được em xử lý bằng `CameraFollow` trong hàm `LateUpdate()` sử dụng phép nội suy `Vector3.Lerp` để bám theo nhân vật mượt mà."*

### 🎤 Phần 2: Gameplay Loop & Singleton GameManager (1 phút)
> *"Về phần logic trò chơi: Em áp dụng mẫu thiết kế `Singleton` để xây dựng `GameManager` tập trung. 
> - Đồng xu Coin được em đóng gói thành `Prefab` có collider dạng `Trigger`. Khi Player va chạm, Coin gọi `GameManager.instance.AddScore()` để cộng điểm, hiển thị lên giao diện qua `TextMeshPro` và tự xóa bằng `Destroy()`.
> - Em tạo bẫy gai `Hazard` và quái vật `EnemyPatrol` có khả năng tự động đi tuần tra giữa 2 điểm A và B bằng `Vector2.MoveTowards()`. Khi va chạm với người chơi, chúng sẽ kích hoạt trạng thái `GameOver()`, đóng băng thời gian bằng `Time.timeScale = 0` và hiển thị Panel có nút chơi lại.
> - Cuối màn chơi em bố trí vật phẩm `KeyItem`. Khi người chơi thu thập chìa khóa, trạng thái `WinGame()` sẽ được kích hoạt để hoàn thành vòng lặp chiến thắng của game."*

### 🎤 Phần 3: Trình diễn Demo (1 phút)
> *(Bấm Play trực tiếp trên máy)*
> - *"Em xin demo: Camera trượt theo Player khi em di chuyển.*
> - *Em nhặt xu: Score nhảy từ 0 lên 1, 2, 3.*
> - *Em nhảy qua bẫy gai và vượt qua cột tường.*
> - *Quái vật đang tự động đi tuần tra giữa 2 điểm A và B và tự lật mặt.*
> - *Khi em nhặt chìa khóa ở đích đến: Bảng YOU WIN hiển thị và game dừng lại thành công!"*

---

## 7. BỘ 10 CÂU HỎI VẤN ĐÁP (ORAL EXAM) LAB 2 KÈM ĐÁP ÁN CHUẨN

### ❓ Câu 1: Tại sao trong Tilemap của sàn đất, em lại phải dùng thêm `Composite Collider 2D` kết hợp với `Tilemap Collider 2D`?
- **Trả lời:**  
  *"Nếu chỉ dùng `Tilemap Collider 2D`, mỗi ô gạch nhỏ sẽ có một hộp va chạm riêng biệt. Khi nhân vật di chuyển qua mép nối giữa hai ô gạch, hệ thống vật lý có thể tính toán sai bề mặt va chạm khiến nhân vật bị 'vấp chân' hoặc nhảy bị giật cục. Khi thêm `Composite Collider 2D` và tích chọn `Used By Composite`, Unity sẽ gộp tất cả các ô liền kề thành một đường bao va chạm duy nhất, vừa giúp nhân vật chạy phẳng mượt mà, vừa giảm số lượng Collider để tối ưu hóa CPU."*

---

### ❓ Câu 2: Tại sao `Rigidbody 2D` sinh ra trên `Composite Collider 2D` của Tilemap bắt buộc phải đặt là `Static`?
- **Trả lời:**  
  *"Vì địa hình đất là vật thể cố định trong không gian, không bị rơi do trọng lực và không di chuyển khi có lực tác động. Đặt là `Static` giúp Unity tối ưu hóa hoàn toàn việc tính toán vật lý (bỏ qua tính toán ma sát nội lực, trọng trường), tránh việc toàn bộ bản đồ sàn đất bị rơi tụt xuống dưới đáy màn hình."*

---

### ❓ Câu 3: Khái niệm `Prefab` trong Unity là gì và tại sao đồng Coin lại cần phải được tạo thành Prefab?
- **Trả lời:**  
  *"`Prefab` (viết tắt của Prefabricated Object) là một khuôn mẫu đối tượng được cấu hình sẵn đầy đủ component, thuộc tính và script, lưu thành một file asset trong project. Đồng Coin cần tạo thành Prefab để ta có thể kéo thả sinh ra hàng chục, hàng trăm đồng coin khắp bản đồ. Khi muốn thay đổi điểm số, sprite hay âm thanh, ta chỉ cần sửa đúng 1 file Prefab gốc là toàn bộ các đồng coin trong game sẽ tự động được cập nhật đồng loạt."*

---

### ❓ Câu 4: Sự khác nhau cơ bản giữa `OnCollisionEnter2D` và `OnTriggerEnter2D` là gì? Khi nào dùng cái nào?
- **Trả lời:**  
  - *`OnCollisionEnter2D` xảy ra khi 2 Collider cứng va chạm vật lý với nhau (cả hai đều không tích `Is Trigger`). Hai vật thể sẽ bị cản lại, tạo lực dội hoặc ma sát (dùng cho sàn đất, tường cản, thùng gỗ).*
  - *`OnTriggerEnter2D` xảy ra khi ít nhất một Collider bật thuộc tính `Is Trigger = true`. Lúc này không có lực cản vật lý, các vật thể đi xuyên qua nhau nhưng vẫn gửi tín hiệu báo va chạm (dùng cho khu vực cảm biến: nhặt coin, nhặt chìa khóa, rơi vào bẫy, cổng dịch chuyển).*

---

### ❓ Câu 5: Mẫu thiết kế Singleton trong `GameManager` hoạt động như thế nào và có ưu điểm gì?
- **Trả lời:**  
  *"Singleton sử dụng một biến tĩnh `public static GameManager instance`. Trong hàm `Awake()`, nếu `instance == null` thì gán `instance = this`. Ưu điểm của nó là tạo ra một trung tâm điều khiển duy nhất cho phép tất cả các script khác gọi trực tiếp bằng `GameManager.instance.<Hàm>()` ở bất kỳ đâu mà không cần phải tốn công kéo thả tham chiếu trên Inspector hay dùng hàm quét `GameObject.Find` tốn tài nguyên."*

---

### ❓ Câu 6: Lệnh `Time.timeScale = 0f` dùng để làm gì? Sau khi dùng lệnh này thì có cần lưu ý điều gì đặc biệt không?
- **Trả lời:**  
  *"Lệnh `Time.timeScale = 0f` làm chậm thời gian trò chơi về 0, giúp đóng băng toàn bộ chuyển động vật lý, hoạt ảnh và các hàm tính toán dựa trên `Time.deltaTime`, rất thích hợp khi Pause game, Game Over hoặc Win. Lưu ý quan trọng là khi người chơi bấm nút 'Chơi lại' hoặc 'Đổi màn', ta bắt buộc phải gán lại `Time.timeScale = 1f`, nếu không màn chơi mới tải lên cũng sẽ bị đóng băng theo."*

---

### ❓ Câu 7: Trong `EnemyPatrol.cs`, em điều khiển quái vật tuần tra bằng phương thức nào? Làm sao quái biết khi nào cần quay đầu?
- **Trả lời:**  
  *"Em dùng hàm `Vector2.MoveTowards()` để tịnh tiến vị trí của quái dần dần về phía điểm đích `currentTarget`. Để nhận biết quay đầu, em dùng hàm `Vector2.Distance(transform.position, currentTarget.position)`. Khi khoảng cách nhỏ hơn một ngưỡng rất bé (ví dụ `0.05f`), em đảo mục tiêu: nếu đang ở điểm B thì chuyển đích sang điểm A và ngược lại, đồng thời gọi hàm `Flip()` để đảo dấu trục X của `transform.localScale` cho quái quay mặt lại."*

---

### ❓ Câu 8: Tại sao lại đặt code Camera theo dõi nhân vật trong hàm `LateUpdate()` mà không phải là `Update()`?
- **Trả lời:**  
  *"Trong vòng lặp một khung hình của Unity, hàm `Update()` chạy trước để xử lý Input và di chuyển nhân vật. Hàm `LateUpdate()` luôn luôn chạy sau cùng, sau khi tất cả các `Update()` đã hoàn thành. Nếu đặt camera bám theo nhân vật trong `Update()`, camera có thể chạy trước khi nhân vật kịp cập nhật vị trí mới, dẫn đến hiện tượng khung hình bị rung giật (jitter). Đặt trong `LateUpdate()` đảm bảo camera luôn bám theo vị trí cuối cùng chuẩn xác nhất của nhân vật."*

---

### ❓ Câu 9: Tại sao em lại sử dụng `TextMeshPro` (TMP) thay vì component `Legacy Text` mặc định cũ của Unity?
- **Trả lời:**  
  *"`Legacy Text` sử dụng kỹ thuật rasterization truyền thống, khi phóng to hoặc chơi trên màn hình độ phân giải cao chữ sẽ bị vỡ hạt, răng cưa và mờ nhòe. `TextMeshPro` áp dụng thuật toán `Signed Distance Field (SDF)`, giúp phông chữ luôn sắc nét hoàn hảo ở mọi khoảng cách và độ phân giải, đồng thời hỗ trợ nhiều hiệu ứng đổ bóng, viền chữ chuyên nghiệp và tối ưu hiệu năng hơn."*

---

### ❓ Câu 10: Giả sử Thầy muốn thêm tính năng: Khi Player nhảy đạp trúng đầu quái vật thì quái chết thay vì Game Over, em sẽ lập trình như thế nào?
- **Trả lời:**  
  *"Em sẽ kiểm tra tọa độ Y trong hàm va chạm: nếu vị trí đáy chân Player cao hơn đỉnh đầu quái (ví dụ `collision.transform.position.y > transform.position.y + 0.3f`), nghĩa là Player đang rơi từ trên xuống đạp trúng đầu. Khi đó em sẽ gọi `Destroy(gameObject)` để tiêu diệt quái, đồng thời gán một lực đẩy nảy nhẹ `rb.linearVelocity = new Vector2(rb.linearVelocity.x, 6f)` cho Player nảy lên. Ngược lại, nếu va chạm ngang thân thì mới gọi `GameManager.instance.GameOver()`."*

---
*Chúc bạn có buổi báo cáo và bảo vệ bài Lab 2 thành công rực rỡ!*
