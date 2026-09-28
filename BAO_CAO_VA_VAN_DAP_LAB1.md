# 🎓 TÀI LIỆU BÁO CÁO, GIẢI THÍCH MÃ NGUỒN & BỘ CÂU HỎI VẤN ĐÁP LAB 1 (UNITY 2D PLATFORMER)

> **Dự án:** Game 2D Platformer (Lab 1)  
> **Phiên bản Engine:** Unity 6 (6000.3.23f1)  
> **Hệ thống Render:** Universal Render Pipeline (URP 2D)  
> **Ngôn ngữ:** C# (.NET Standard / Unity API)  

---

## 📑 MỤC LỤC
1. [Tổng quan dự án & Mục tiêu Lab 1](#1-tổng-quan-dự-án--mục-tiêu-lab-1)
2. [Giải thích chi tiết cấu trúc thư mục dự án](#2-giải-thích-chi-tiết-cấu-trúc-thư-mục-dự-án)
3. [Giải thích chi tiết mã nguồn PlayerController.cs](#3-giải-thích-chi-tiết-mã-nguồn-playercontrollercs)
4. [Các kiến thức cốt lõi về lập trình Game 2D đã áp dụng](#4-các-kiến-thức-cốt-lõi-về-lập-trình-game-2d-đã-áp-dụng)
5. [Kịch bản thuyết trình với Giảng viên](#5-kịch-bản-thuyết-trình-với-giảng-viên)
6. [Bộ câu hỏi vấn đáp (Oral Exam) mô phỏng kèm đáp án](#6-bộ-câu-hỏi-vấn-đáp-oral-exam-mô-phỏng-kèm-đáp-án)

---

## 1. TỔNG QUAN DỰ ÁN & MỤC TIÊU LAB 1

Bài Lab 1 là nền móng của một tựa game 2D Platformer hoàn chỉnh. Mục tiêu bao gồm:
1. **Kiến trúc tài nguyên chuẩn:** Sắp xếp thư mục theo chuẩn công nghiệp trong Unity.
2. **Cơ chế vật lý nhân vật (Character Physics):**
   - Thiết lập `Rigidbody 2D` và `Box Collider 2D`.
   - Chống xoay lật nhân vật (`Freeze Rotation Z`).
   - Chống hiện tượng rơi xuyên sàn ở tốc độ cao (`Continuous Collision Detection`).
   - Khử ma sát dính tường bằng `Physics Material 2D` (`Friction = 0`).
3. **Cơ chế điều khiển (Movement & Jump):**
   - Di chuyển ngang mượt mà, độc lập với tốc độ khung hình (FPS).
   - Tự động quay mặt theo hướng di chuyển bằng phép biến đổi hình học (`transform.localScale`).
   - Cơ chế nhảy tiếp đất chuẩn xác bằng kiểm tra va chạm hình học `Physics2D.OverlapCircle` kết hợp lớp mặt nạ `LayerMask`. Tuyệt đối chống bug nhảy vô hạn trên không (infinite jump).
4. **Hệ thống diễn hoạt (Animation System):**
   - Tạo các Animation Clips: `Idle`, `Run`, `Jump`.
   - Xây dựng máy trạng thái hữu hạn (**FSM - Finite State Machine**) trong `Animator Controller`.
   - Kết nối dữ liệu thời gian thực từ script C# sang Animator thông qua các tham số logic (Parameters).

---

## 2. GIẢI THÍCH CHI TIẾT CẤU TRÚC THƯ MỤC DỰ ÁN

Trong một dự án Unity tiêu chuẩn, cấu trúc thư mục được chia làm hai tầng: **Tầng hệ thống của Engine** và **Tầng tài nguyên trò chơi (`Assets`)**.

```text
My project (5)/
├── Assets/                 ⭐ [QUAN TRỌNG NHẤT] Chứa toàn bộ tài nguyên game
│   ├── _Scripts/           # Mã nguồn C# logic game
│   ├── Sprites/            # Hình ảnh đồ họa 2D (Textures, Sprites)
│   ├── Animations/         # Animation Clips (.anim) & Animator Controller (.controller)
│   ├── PhysicsMaterials/   # Vật liệu vật lý 2D (.physicsMaterial2D)
│   ├── Editor/             # Các công cụ hỗ trợ tự động hóa trong Unity Editor
│   ├── Scenes/             # Các màn chơi / phân cảnh (.unity)
│   └── Settings/           # Cấu hình đồ họa URP (Render Pipeline)
├── Packages/               # Quản lý các thư viện mở rộng từ Unity Package Manager
├── ProjectSettings/        # Cài đặt toàn cục (Layers, Tags, Physics, Input, Audio...)
├── Library/                # Cache dữ liệu biên dịch của Unity (Không đưa lên Git)
├── Logs/                   # Nhật ký lỗi và cảnh báo của Editor
└── UserSettings/           # Cấu hình giao diện riêng của từng lập trình viên
```

### 2.1. Thư mục con trong `Assets/`

#### 📁 `Assets/_Scripts/`
- **Mục đích:** Lưu trữ tất cả các lớp C# kế thừa từ `MonoBehaviour` điều khiển logic gameplay.
- **Quy ước:** Dấu gạch dưới `_` ở đầu tên thư mục giúp thư mục này luôn nằm ở trên cùng danh sách bảng Project theo thứ tự bảng chữ cái, giúp lập trình viên mở code nhanh chóng.
- **Tập tin:**
  - `PlayerController.cs`: Bộ não điều khiển nhân vật chính (di chuyển, nhảy, kiểm tra va chạm đất, lật hướng, cập nhật animation).

#### 📁 `Assets/Sprites/`
- **Mục đích:** Chứa các hình ảnh đồ họa 2D pixel/vector thô.
- **Tập tin:**
  - `Player.png`: Sprite nhân vật 64x64 được thiết kế có hướng mắt rõ ràng (nhìn sang phải) để kiểm tra tính năng lật mặt.
  - `Ground.png`: Texture khối đất cỏ dùng để lát nền sàn va chạm.
  - `Wall.png`: Texture tường gạch dùng làm chướng ngại vật kiểm tra khả năng nhảy vượt và tính năng chống dính tường.
- **Kiến thức kỹ thuật:** Các ảnh khi đưa vào Unity đều được cấu hình Texture Type là `Sprite (2D and UI)`, chế độ nén chuẩn pixel art với Filter Mode là `Point (no filter)` để ảnh sắc nét, không bị nhòe mờ.

#### 📁 `Assets/Animations/`
- **Mục đích:** Chứa dữ liệu chuyển động và máy trạng thái hoạt ảnh.
- **Tập tin:**
  - `Player_Idle.anim`: Clip hoạt ảnh nhân vật đứng yên thở nhẹ.
  - `Player_Run.anim`: Clip hoạt ảnh nhân vật chạy bộ.
  - `Player_Jump.anim`: Clip hoạt ảnh nhân vật co chân nhảy trên không.
  - `PlayerAnimatorController.controller`: Bộ điều khiển máy trạng thái (Animator Controller) quản lý các nút trạng thái (States) và điều kiện chuyển đổi (Transitions) giữa các clip dựa trên các tham số logic (`isRunning`, `isJumping`).

#### 📁 `Assets/PhysicsMaterials/`
- **Mục đích:** Chứa các thuộc tính ma sát và độ đàn hồi bề mặt cho hệ thống vật lý 2D.
- **Tập tin:**
  - `ZeroFriction.physicsMaterial2D`: Thiết lập `Friction = 0` và `Bounciness = 0`. Khi gán vào `Box Collider 2D` của Player, nhân vật sẽ trượt mượt mà dọc theo tường khi va chạm, không bao giờ bị lực ma sát giữ lại gây hiện tượng "dính lơ lửng trên tường".

#### 📁 `Assets/Editor/`
- **Mục đích:** Thư mục đặc biệt của Unity. Tất cả script nằm trong thư mục `Editor` sẽ sử dụng namespace `UnityEditor` và chỉ chạy trong môi trường phát triển (Editor), không bị biên dịch vào gói game đóng gói xuất xưởng (.exe / APK).
- **Tập tin:**
  - `Lab1Automator.cs`: Script tự động hóa toàn bộ việc cấu hình Layer, Sorting Layer, tạo vật liệu, sinh ra các GameObject và kết nối tham chiếu chỉ với 1 cú click chuột (menu `Lab 1 > Chạy Cài Đặt Tự Động`).

#### 📁 `Assets/Scenes/`
- **Mục đích:** Chứa các màn chơi (Scene). Mỗi Scene là một môi trường chứa `Hierarchy`, các GameObject, Camera và ánh sáng.
- **Tập tin:** `SampleScene.unity`.

---

## 3. GIẢI THÍCH CHI TIẾT MÃ NGUỒN `PlayerController.cs`

File `PlayerController.cs` là thành phần cốt lõi của bài Lab. Dưới đây là giải thích chi tiết từng khối mã:

```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("--- THIẾT LẬP DI CHUYỂN ---")]
    [Tooltip("Tốc độ di chuyển ngang của nhân vật")]
    public float moveSpeed = 7f;
    
    [Tooltip("Lực nhảy của nhân vật (đủ để nhảy vượt chướng ngại vật)")]
    public float jumpForce = 10.5f;

    [Header("--- KIỂM TRA MẶT ĐẤT ---")]
    [Tooltip("Điểm kiểm tra đặt dưới chân nhân vật")]
    public Transform groundCheck;
    
    [Tooltip("Bán kính vòng tròn quét va chạm")]
    public float checkRadius = 0.15f;
    
    [Tooltip("Layer đại diện cho mặt đất")]
    public LayerMask groundLayer;

    // Các biến nội bộ
    private Rigidbody2D rb;
    private Animator anim;
    private float horizontalInput;
    private bool isGrounded;
    private bool isFacingRight = true;
```
### 💡 Phân tích khối khai báo biến:
- `[Header]`, `[Tooltip]`: Các Attribute giúp giao diện Inspector trên Unity hiển thị tiêu đề ngăn nắp và chú thích giải thích khi rê chuột vào biến.
- `public float moveSpeed`: Vận tốc di chuyển theo trục X.
- `public float jumpForce`: Độ lớn của vận tốc tức thời đẩy nhân vật lên trục Y khi nhảy.
- `public Transform groundCheck`: Lưu toạ độ của GameObject con nằm ở đáy bàn chân.
- `public LayerMask groundLayer`: Mặt nạ nhị phân xác định đối tượng nào được coi là "mặt đất" (chỉ những đối tượng có Layer là `Ground` mới được nhận diện).
- `private Rigidbody2D rb`, `private Animator anim`: Bộ nhớ đệm lưu tham chiếu đến các component vật lý và chuyển động trên nhân vật.

---

### 💡 Khối `Awake()`: Khởi tạo thông số & Tối ưu Render
```csharp
    void Awake()
    {
        // Tự động nâng jumpForce nếu đang nhỏ hơn 10f để người chơi nhảy qua được tường
        if (jumpForce < 10f)
        {
            jumpForce = 10.5f;
        }

        // 1. Căn chỉnh lại BoxCollider2D vừa khít bàn chân để nhân vật chạm sàn đất chuẩn xác
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = new Vector2(0.55f, 0.62f);
            col.offset = new Vector2(0, 0f);
        }

        // 2. Căn chỉnh vị trí GroundCheck sát đáy chân
        if (groundCheck != null)
        {
            groundCheck.localPosition = new Vector3(0, -0.32f, 0);
        }

        // 3. Sửa lỗi nhân vật bị đen bằng cách dùng Shader Unlit (không bị ảnh hưởng bởi bóng tối)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Shader unlitShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit") 
                              ?? Shader.Find("Sprites/Default");
            if (unlitShader != null)
            {
                sr.material = new Material(unlitShader);
            }
        }
    }
```
- **Ý nghĩa `Awake()`:** Phương thức được Unity gọi đầu tiên ngay khi GameObject được nạp vào bộ nhớ (trước cả `Start()`). Dùng để khởi tạo cấu hình nội tại.
- **Sửa lỗi chân lơ lửng:** Đặt lại `col.size = (0.55f, 0.62f)` để mép đáy hộp va chạm trùng khít với đáy bàn chân của Sprite 64x64, loại bỏ hoàn toàn khoảng hở nổi trên sàn.
- **Sửa lỗi nhân vật bị đen bóng (Silhouette):** Trong URP 2D, nếu Sprite dùng shader `Sprite-Lit` mà Sorting Layer `Player` không nằm trong danh sách chiếu sáng của `Global Light 2D`, nhân vật sẽ bị tối đen. Đoạn code tự động chuyển material sang `Sprite-Unlit` giúp nhân vật luôn hiển thị đầy đủ màu sắc chân thật.

---

### 💡 Khối `Start()`: Lấy tham chiếu Component
```csharp
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
```
- Sử dụng `GetComponent<T>()` một lần duy nhất tại `Start()` để lưu vào biến tham chiếu, tránh việc gọi liên tục trong `Update()` (rất tốn tài nguyên CPU).

---

### 💡 Khối `Update()`: Tiếp nhận Input & Logic trạng thái
```csharp
    void Update()
    {
        // 1. Nhận tín hiệu phím di chuyển ngang & phím nhảy tương thích cả 2 hệ thống Input của Unity
        horizontalInput = 0f;
        bool jumpPressed = false;

#if ENABLE_INPUT_SYSTEM
        var kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb != null)
        {
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontalInput += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) horizontalInput -= 1f;
            if (kb.spaceKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame)
            {
                jumpPressed = true;
            }
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (horizontalInput == 0f)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
#endif
```
- **Tại sao bắt Input trong `Update()`?** `Update()` chạy theo từng khung hình đồ họa (tần số quét màn hình, ví dụ 60Hz, 144Hz). Các sự kiện bấm phím như `wasPressedThisFrame` hoặc `GetButtonDown` diễn ra tức thời trong 1 frame duy nhất, nếu đặt trong `FixedUpdate()` rất dễ bị hiện tượng "nuốt phím" (hụt thao tác người dùng).
- **Kỹ thuật tương thích đa Input (Cross-Input Compatibility):** Sử dụng các chỉ thị tiền biên dịch `#if ENABLE_INPUT_SYSTEM` và `#if ENABLE_LEGACY_INPUT_MANAGER` giúp script chạy hoàn hảo trên cả Unity 6 mới (Input System package) lẫn các phiên bản Unity cũ sử dụng Input Manager truyền thống mà không bị quăng lỗi `InvalidOperationException`.

```csharp
        // 2. Sử dụng Physics2D.OverlapCircle để quét xem chân có chạm đất không
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        }

        // 3. Cơ chế nhảy: Chỉ nhảy khi bấm phím Jump VÀ đang tiếp đất
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 4. Tự động quay mặt theo hướng di chuyển
        Flip();

        // 5. Cập nhật các thông số cho Animator Controller
        UpdateAnimation();
    }
```
- **Hàm `Physics2D.OverlapCircle(position, radius, layerMask)`:** Vẽ một vòng tròn ảo tại vị trí `groundCheck.position` với bán kính `checkRadius`. Nếu có bất kỳ Collider nào thuộc lớp `groundLayer` giao cắt với vòng tròn này, hàm trả về `true` (nhân vật đang đứng trên đất).
- **Cơ chế nhảy an toàn:** Điều kiện `jumpPressed && isGrounded` ngăn chặn hoàn toàn việc người chơi bấm phím nhảy liên tục khi đang rơi giữa không trung (Double-jump / Infinite-jump bug).
- **API `rb.linearVelocity`:** Trong Unity 6, thuộc tính vận tốc vật lý tuyến tính được đổi tên chính thức từ `rb.velocity` thành `rb.linearVelocity` để chuẩn hoá cùng Unity Physics Core.

---

### 💡 Khối `FixedUpdate()`: Xử lý lực vật lý
```csharp
    void FixedUpdate()
    {
        // Áp dụng vận tốc vật lý trong FixedUpdate để đảm bảo mượt mà
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }
```
- **Tại sao tính toán vật lý trong `FixedUpdate()`?** `FixedUpdate()` được gọi ở các bước thời gian cố định độc lập với tốc độ khung hình đồ họa (mặc định 0.02 giây = 50 lần/giây). Đặt vận tốc vật lý ở đây đảm bảo chuyển động vật lý luôn đồng nhất trên mọi dòng máy tính (máy mạnh hay máy yếu đều di chuyển cùng một vận tốc, không bị giật lag hay biến dạng gia tốc).
- **Bảo toàn trục Y:** Giữ nguyên `rb.linearVelocity.y` để lực hút trọng lực rơi tự do không bị can thiệp khi đang di chuyển ngang.

---

### 💡 Khối `Flip()`: Lật hướng mặt nhân vật
```csharp
    void Flip()
    {
        // Đang đi sang phải nhưng mặt đang quay trái -> Lật sang phải
        if (horizontalInput > 0 && !isFacingRight)
        {
            isFacingRight = true;
            Vector3 scaler = transform.localScale;
            scaler.x = Mathf.Abs(scaler.x);
            transform.localScale = scaler;
        }
        // Đang đi sang trái nhưng mặt đang quay phải -> Lật sang trái
        else if (horizontalInput < 0 && isFacingRight)
        {
            isFacingRight = false;
            Vector3 scaler = transform.localScale;
            scaler.x = -Mathf.Abs(scaler.x);
            transform.localScale = scaler;
        }
    }
```
- **Cơ chế:** Đảo dấu trục X của `transform.localScale`.
- **Ưu điểm vượt trội so với `SpriteRenderer.flipX`:**
  - Nếu dùng `flipX`, Unity chỉ đảo chiều hình ảnh của Sprite đó, nhưng các GameObject con (như điểm `GroundCheck`, súng, mắt, điểm bắn đạn) sẽ **không** đổi chiều theo.
  - Khi dùng `transform.localScale.x = -Mathf.Abs(...)`, toàn bộ hệ tọa độ cục bộ của nhân vật và tất cả các đối tượng con bên trong đều quay 180 độ một cách chuẩn xác.

---

### 💡 Khối `UpdateAnimation()` & `OnDrawGizmosSelected()`
```csharp
    void UpdateAnimation()
    {
        if (anim != null)
        {
            // isRunning = true khi có bấm di chuyển (horizontalInput khác 0)
            anim.SetBool("isRunning", horizontalInput != 0);
            
            // isJumping = true khi nhân vật đang ở trên không (không chạm đất)
            anim.SetBool("isJumping", !isGrounded);
        }
    }

    // Hiển thị vòng tròn GroundCheck trong Scene để dễ dàng căn chỉnh bằng mắt
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
```
- **Đồng bộ với Animator:** Gửi giá trị boolean `isRunning` và `isJumping` vào Animator Controller.
- **Gizmos:** Dùng `Gizmos.DrawWireSphere` giúp lập trình viên nhìn thấy vòng tròn va chạm màu đỏ trong màn hình Scene để dễ dàng căn chỉnh toạ độ điểm tiếp đất bằng mắt thường mà không cần đoán mò.

---

## 4. CÁC KIẾN THỨC CỐT LÕI VỀ LẬP TRÌNH GAME 2D ĐÃ ÁP DỤNG

| Khái niệm | Giải thích bản chất | Cách triển khai trong Lab 1 |
| :--- | :--- | :--- |
| **Game Loop (Vòng lặp trò chơi)** | Chu trình lặp vô hạn xử lý: Input ➔ Update ➔ Physics ➔ Render. | Tách rõ Input trong `Update()` và vận tốc trong `FixedUpdate()`. |
| **Rigidbody 2D Dynamic** | Thành phần gán đặc tính vật lý (khối lượng, vận tốc, trọng lực) cho đối tượng 2D. | Thiết lập `Collision Detection: Continuous` và `Freeze Rotation Z`. |
| **Collision Detection (Continuous)** | Quét va chạm liên tục giữa 2 frame thay vì rời rạc (`Discrete`). | Giúp nhân vật khi rơi với vận tốc cao không bị "xuyên thủng" qua sàn đất. |
| **Physics Material 2D** | Xác định độ ma sát (Friction) và độ nảy (Bounciness) giữa 2 Collider khi tiếp xúc. | Tạo file `ZeroFriction` (`Friction = 0`) gán vào Player để trượt tường mượt mà. |
| **LayerMask & Bitwise** | Hệ thống mặt nạ bit lọc va chạm cực nhanh trên phần cứng. | `groundLayer` chỉ chọn lớp `Ground`, bỏ qua các vật thể khác (quái, coin, background). |
| **Finite State Machine (FSM)** | Mô hình toán học quản lý trạng thái chuyển động (chỉ ở 1 trạng thái tại 1 thời điểm). | `Animator Controller` với 3 trạng thái `Idle`, `Run`, `Jump`. |
| **Transitions (Animator)** | Cung chuyển đổi trạng thái hoạt ảnh. | Tắt `Has Exit Time`, đặt `Duration = 0` để nhân vật đổi động tác tức thì, không bị trễ. |
| **Sorting Layers** | Lớp thứ tự vẽ điểm ảnh 2D từ xa đến gần. | Đặt Player ở layer `Player` để nhân vật luôn nằm đè lên layer `Ground` và `Background`. |

---

## 5. KỊCH BẢN THUYẾT TRÌNH VỚI GIẢNG VIÊN

Khi lên bảng hoặc mở máy báo cáo với Thầy/Cô, bạn có thể trình bày tự tin theo 3 phần ngắn gọn sau:

### 🎤 Phần 1: Giới thiệu kiến trúc & Thiết lập môi trường (1 phút)
> *"Thưa Thầy/Cô, trong bài Lab 1 này, em đã xây dựng hoàn chỉnh nhân vật 2D Platformer với cấu trúc thư mục chuẩn: tách biệt rõ ràng giữa Scripts, Sprites, Animations và PhysicsMaterials. Em cấu hình hệ thống Sorting Layer với lớp `Player` nằm trước lớp `Ground`, đồng thời thiết lập vật lý cho nhân vật bằng `Rigidbody 2D` ở chế độ `Continuous` và khoá xoay trục Z để nhân vật luôn đứng vững khi va chạm."*

### 🎤 Phần 2: Giải thích cơ chế điều khiển & Vật lý (2 phút)
> *"Về phần điều khiển trong `PlayerController.cs`:*
> *1. Em xử lý nhận tín hiệu di chuyển ngang trong hàm `Update()` và áp dụng vận tốc qua `rb.linearVelocity` trong hàm `FixedUpdate()` để đảm bảo chuyển động vật lý mượt mà và độc lập với FPS.*
> *2. Để nhân vật quay mặt khi đổi hướng, em thay đổi dấu trục X của `transform.localScale`, giúp đồng bộ hướng của cả nhân vật lẫn các điểm con như `GroundCheck`.*
> *3. Để xử lý nhảy, em sử dụng kỹ thuật `Physics2D.OverlapCircle` kết hợp `LayerMask` tại điểm `GroundCheck` dưới đáy bàn chân. Điều kiện nhảy chỉ thỏa mãn khi nhân vật đang thực sự chạm đất, triệt tiêu hoàn toàn lỗi nhảy vô hạn trên không.*
> *4. Để khắc phục lỗi nhân vật bị dính chặt vào tường khi vừa nhảy vừa bấm phím ép vào tường, em đã tạo và gán một `Physics Material 2D` có hệ số ma sát `Friction = 0` vào Box Collider của Player."*

### 🎤 Phần 3: Trình diễn Demo (1 phút)
> *"Sau đây em xin phép chạy thử Demo trực tiếp trên Unity:*
> *(Bấm Play)*
> - *Khi em bấm A/D: Nhân vật chạy và đổi hướng nhìn mặt chuẩn xác, Animator chuyển sang trạng thái `Run`.*
> - *Khi em bấm Space: Nhân vật nhảy lên vừa tầm, Animator chuyển sang `Jump` và khi chạm đất quay về `Idle`.*
> - *Khi em bấm liên tục Space trên không trung: Nhân vật hoàn toàn không thể nhảy tiếp.*
> - *Khi em nhảy áp sát vào cột tường `Wall_Test`: Nhân vật trượt êm xuống đất, không bị kẹt dính trên tường."*

---

## 6. BỘ CÂU HỎI VẤN ĐÁP (ORAL EXAM) MÔ PHỎNG KÈM ĐÁP ÁN

Dưới đây là 10 câu hỏi "sát sườn" mà các giảng viên lập trình game thường hỏi nhất, kèm câu trả lời chuẩn xác:

### ❓ Câu 1: Tại sao em lại nhận Input trong hàm `Update()` mà lại gán vận tốc vật lý trong hàm `FixedUpdate()`?
- **Trả lời:**  
  *"`Update()` chạy đồng bộ với tần số quét của màn hình (Frame Rate), là nơi lý tưởng để bắt các sự kiện bấm phím diễn ra tức thời trong 1 khung hình như `Input.GetButtonDown` để không bị hụt phím. Trong khi đó, `FixedUpdate()` chạy ở chu kỳ thời gian vật lý cố định (Fixed Timestep 0.02s), giúp việc tính toán vận tốc của `Rigidbody 2D` luôn ổn định và không bị phụ thuộc vào việc máy tính đang chạy FPS cao hay thấp."*

---

### ❓ Câu 2: Tại sao nhân vật khi nhảy áp sát vào tường lại hay bị "dính" lơ lửng không rơi xuống? Em xử lý việc này bằng cách nào?
- **Trả lời:**  
  *"Hiện tượng đó xảy ra do ma sát mặc định (`Friction`) giữa hai Collider khi cọ xát với nhau. Khi người chơi nhấn giữ phím di chuyển ép nhân vật vào bề mặt thẳng đứng của tường, lực ma sát lớn hơn hoặc cân bằng với trọng lực làm triệt tiêu vận tốc rơi tự do của nhân vật. Em đã khắc phục bằng cách tạo một `Physics Material 2D` với thuộc tính `Friction = 0` và gán vào `Box Collider 2D` của Player, loại bỏ hoàn toàn ma sát trượt."*

---

### ❓ Câu 3: Làm thế nào em ngăn chặn được lỗi người chơi bấm phím nhảy liên tục khi đang ở trên không (Infinite Jump)?
- **Trả lời:**  
  *"Em tạo một GameObject con tên là `GroundCheck` đặt ngay dưới chân Player. Trong code, em dùng hàm `Physics2D.OverlapCircle` quét một vòng tròn nhỏ tại vị trí này với bộ lọc `LayerMask groundLayer`. Hàm này chỉ trả về `true` khi chân Player chạm đúng vào bề mặt có layer là `Ground`. Em đặt điều kiện: `if (jumpPressed && isGrounded)` thì mới cho phép gán vận tốc nhảy theo trục Y."*

---

### ❓ Câu 4: Sự khác nhau giữa việc lật mặt nhân vật bằng `transform.localScale.x = -1` và dùng `SpriteRenderer.flipX = true` là gì?
- **Trả lời:**  
  *"`SpriteRenderer.flipX` chỉ đơn thuần đảo ngược hình ảnh hiển thị của riêng Sprite đó, nhưng không hề thay đổi hệ tọa độ của GameObject. Các đối tượng con nằm trong nó (ví dụ điểm `GroundCheck`, điểm bắn súng Muzzle, các Collider lệch tâm) vẫn giữ nguyên vị trí cũ. Còn khi thay đổi dấu của `transform.localScale.x`, toàn bộ hệ quy chiếu không gian cục bộ của nhân vật và toàn bộ con cái của nó đều lật đồng bộ 180 độ theo hướng đi."*

---

### ❓ Câu 5: Trong Unity 6, tại sao em dùng `rb.linearVelocity` thay vì `rb.velocity`?
- **Trả lời:**  
  *"Trong Unity 6, đội ngũ phát triển đã cập nhật kiến trúc Unity Physics Core mới. Thuộc tính `rb.velocity` cũ đã bị đánh dấu lỗi thời (deprecated) và được thay thế bằng `rb.linearVelocity` (vận tốc tuyến tính) để phân biệt rõ ràng với `rb.angularVelocity` (vận tốc góc xoay)."*

---

### ❓ Câu 6: Thuộc tính `Freeze Rotation Z` trong Rigidbody 2D có tác dụng gì? Nếu không bật thì điều gì sẽ xảy ra?
- **Trả lời:**  
  *"Trong không gian 2D, trục Z là trục vuông góc với màn hình máy tính (trục xoay của vật thể). `Freeze Rotation Z` khóa hoàn toàn mô-men xoắn trên trục này. Nếu không bật, khi nhân vật nhảy va chạm vào các góc gờ của sàn đất hoặc tường, lực phản hồi sẽ làm nhân vật bị xoay tròn, ngã lộn nhào hoặc cắm đầu xuống đất."*

---

### ❓ Câu 7: Trong cửa sổ Animator Controller, tại sao các đường chuyển đổi (Transitions) giữa Idle, Run và Jump cần phải tắt `Has Exit Time` và đặt `Transition Duration = 0`?
- **Trả lời:**  
  *"`Has Exit Time` nếu bật sẽ bắt hoạt ảnh hiện tại phải phát hết thời lượng của nó (hoặc đạt một tỷ lệ phần trăm nhất định) thì mới cho phép chuyển sang hoạt ảnh tiếp theo, điều này sẽ tạo cảm giác trễ (input lag) cực kỳ khó chịu trong game hành động. Đặt `Has Exit Time = false` và `Duration = 0` giúp nhân vật chuyển trạng thái tức thì ngay khi điều kiện (`isRunning` hoặc `isJumping`) thay đổi."*

---

### ❓ Câu 8: Tại sao em lại đặt giá trị của `Collision Detection` trên Rigidbody 2D là `Continuous` thay vì `Discrete`?
- **Trả lời:**  
  *"`Discrete` (rời rạc) kiểm tra va chạm theo từng bước thời gian cố định. Nếu nhân vật rơi với vận tốc quá lớn, vị trí ở frame trước có thể đang ở trên sàn và vị trí ở frame sau đã vượt qua mặt sàn, dẫn đến lỗi rơi xuyên sàn (tunneling). `Continuous` (liên tục) sẽ dựng một thể tích quét (swept volume) giữa vị trí cũ và vị trí mới để phát hiện va chạm trên đường đi, ngăn chặn hoàn toàn lỗi xuyên sàn."*

---

### ❓ Câu 9: Tác dụng của hàm `OnDrawGizmosSelected()` trong script của em là gì? Nó có chạy khi xuất bản game ra file `.exe` không?
- **Trả lời:**  
  *"Hàm `OnDrawGizmosSelected()` là một hàm hỗ trợ trực quan hóa của Unity Editor, giúp vẽ một vòng tròn dây (`DrawWireSphere`) màu đỏ thể hiện chính xác phạm vi quét của điểm `GroundCheck` trên cửa sổ Scene khi ta click chọn Player. Hàm này chỉ chạy trong Unity Editor để lập trình viên căn chỉnh bằng mắt, hoàn toàn tự động bị loại bỏ (strip) khi build ra file game `.exe`, không gây tốn tài nguyên runtime."*

---

### ❓ Câu 10: Nếu muốn cho nhân vật có tính năng nhảy 2 lần (Double Jump), em sẽ sửa mã nguồn như thế nào?
- **Trả lời:**  
  *"Em sẽ khai báo thêm một biến nguyên đếm số lần nhảy, ví dụ `int extraJumps = 1;` và `int extraJumpsValue = 1;`. Khi `isGrounded == true`, em sẽ reset `extraJumps = extraJumpsValue`. Khi người chơi nhấn nút Jump, nếu `isGrounded` đúng thì nhảy lần 1; nếu đang ở trên không (`!isGrounded`) mà `extraJumps > 0` thì vẫn cho phép gán vận tốc nhảy và trừ `extraJumps--`."*

---
*Chúc bạn có buổi báo cáo và bảo vệ bài Lab đạt điểm tuyệt đối!*
