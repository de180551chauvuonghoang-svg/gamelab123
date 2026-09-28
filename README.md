# 🎮 2D Platformer Game - Unity (Lab 1, 2, 3)

Tài liệu hướng dẫn tổng quan và chi tiết thực hiện bài Lab phát triển Game 2D Platformer bằng Unity Engine.

---

## 📑 Mục lục
1. [Giới thiệu dự án](#-giới-thiệu-dự-án)
2. [Yêu cầu hệ thống & Môi trường](#-yêu-cầu-hệ-thống--môi-trường)
3. [Cấu trúc thư mục dự án](#-cấu-trúc-thư-mục-dự-án)
4. [Tóm tắt nội dung các bài Lab](#-tóm-tắt-nội-dung-các-bài-lab)
   - [Lab 1: Player, Vật lý & Animation](#lab-1-player-vật-lý--animation)
   - [Lab 2: Tilemap, Cinemachine, Logic & UI](#lab-2-tilemap-cinemachine-logic--ui)
   - [Lab 3: Audio, Sàn di chuyển, Menu & Build](#lab-3-audio-sàn-di-chuyển-menu--build)
5. [Danh sách Script C# và chức năng](#-danh-sách-script-c-và-chức-năng)
6. [Phím bấm điều khiển (Controls)](#-phím-bấm-điều-khiển-controls)
7. [Hướng dẫn cài đặt & Build Game](#-hướng-dẫn-cài-đặt--build-game)

---

## 📌 Giới thiệu dự án
Dự án game 2D Platformer được phát triển qua 3 giai đoạn (Lab 1, 2, 3) giúp người học nắm vững:
- Cơ chế vật lý 2D, điều khiển nhân vật mượt mà, hạn chế ma sát tường.
- Quản lý trạng thái Animation (Idle, Run, Jump).
- Thiết kế màn chơi với Tilemap và camera thông minh Cinemachine.
- Xây dựng Gameplay loop: Thu thập vật phẩm (Coin, Key), vượt bẫy (Spikes, Enemy), thắng/thua (Win/Game Over).
- Quản lý âm thanh bằng mẫu thiết kế Singleton (AudioManager).
- Xây dựng Scene Main Menu và đóng gói xuất bản game (.exe).

---

## 💻 Yêu cầu hệ thống & Môi trường
- **Unity Editor**: Khuyến nghị phiên bản `2021.3 LTS`, `2022.3 LTS` hoặc mới hơn.
- **Template dự án**: `2D (Core)`.
- **Packages cần cài đặt qua Package Manager**:
  - `Cinemachine` (Window > Package Manager > Unity Registry > Cinemachine).
  - `TextMeshPro` (Tự động kích hoạt khi tạo Text TMP).
  - `2D Tilemap Editor` (mặc định đã có trong template 2D).

---

## 📂 Cấu trúc thư mục dự án (Assets)
```text
Assets/
├── _Scripts/               # Toàn bộ mã nguồn C#
│   ├── PlayerController.cs
│   ├── GameManager.cs
│   ├── AudioManager.cs
│   ├── EnemyPatrol.cs
│   ├── MovingPlatform.cs
│   ├── Coin.cs
│   ├── Hazard.cs
│   ├── KeyItem.cs
│   └── MainMenuController.cs
├── Animations/             # Animation Clips & Animator Controller
│   ├── Player_Idle.anim
│   ├── Player_Run.anim
│   ├── Player_Jump.anim
│   └── PlayerController.controller
├── Audio/                  # Nhạc nền (BGM) & Hiệu ứng âm thanh (SFX)
│   ├── BGM.mp3
│   ├── Jump.wav
│   └── Coin.wav
├── Fonts/                  # Font chữ tùy chỉnh và TextMeshPro Font Assets
├── PhysicsMaterials/       # Vật liệu vật lý (ZeroFriction.physicsMaterial2D)
├── Prefabs/                # Các đối tượng mẫu (Coin, Enemy, Platform, Trap)
├── Scenes/                 # Các màn chơi
│   ├── MainMenu.unity      (Build Index 0)
│   └── Level01.unity       (Build Index 1)
├── Sprites/                # Hình ảnh nhân vật, quái, môi trường
└── Tiles/                  # Tile Palettes và Tilesets
```

---

## 🚀 Tóm tắt nội dung các bài Lab

### Lab 1: Player, Vật lý & Animation
- **Player Setup**:
  - GameObject `Player` với `Rigidbody 2D` (Continuous, Freeze Rotation Z) và `Box Collider 2D`.
  - Tạo `Physics Material 2D` có `Friction = 0` gán vào Collider để không bị dính tường.
- **Movement & Jump**:
  - Nhận input qua `Input.GetAxisRaw("Horizontal")`.
  - Kiểm tra tiếp đất bằng `Physics2D.OverlapCircle` tại điểm `GroundCheck` dưới chân nhân vật và LayerMask `Ground`.
  - Chỉ cho phép nhảy khi `isGrounded == true`.
  - Lật hướng nhân vật bằng cách đổi dấu trục X của `transform.localScale`.
- **Hệ thống Animation**:
  - Tạo các clip: `Idle`, `Run`, `Jump`.
  - Animator Parameters: `isRunning` (Bool), `isJumping` (Bool).
  - Transitions không delay (`Has Exit Time = false`, `Transition Duration = 0`).

---

### Lab 2: Tilemap, Cinemachine, Logic & UI
- **Cinemachine 2D Camera**:
  - Tạo Virtual Camera 2D, gán mục **Follow** tới GameObject `Player`.
- **Hệ thống Tilemap**:
  - Thiết lập các lớp: `Background`, `Ground`, `Foreground`.
  - Tối ưu va chạm sàn: Sử dụng `Tilemap Collider 2D` kết hợp `Composite Collider 2D` (Body Type: `Static`, bật `Used By Composite`).
- **Coin & Điểm số**:
  - Coin có `Circle Collider 2D` (bật `Is Trigger`). Chạm vào sẽ tăng điểm trong `GameManager`, phát âm thanh và gọi `Destroy()`.
  - Hiển thị điểm số theo thời gian thực bằng `TextMeshPro UI`.
- **Kẻ địch (Enemy) & Bẫy (Trap)**:
  - `Hazard` (gai/bẫy hố) chạm vào gây Game Over ngay lập tức.
  - `EnemyPatrol` tự động di chuyển tuần tra qua lại giữa 2 điểm A và B, tự động quay mặt theo hướng đi.
- **UI Thắng / Thua**:
  - `GameOverPanel`: Hiển thị nút `Play Again` (load lại màn chơi) và `Main Menu`.
  - `WinPanel`: Kích hoạt khi Player thu thập chìa khóa `Key`.

---

### Lab 3: Audio, Sàn di chuyển, Menu & Build
- **Hệ thống âm thanh (AudioManager)**:
  - Quản lý tập trung bằng mẫu Singleton (`DontDestroyOnLoad`).
  - Phân tách 2 nguồn `AudioSource`:
    - `bgmSource`: Nhạc nền lặp liên tục (`Loop = true`).
    - `sfxSource`: Hiệu ứng âm thanh ngắn (nhảy, ăn xu) qua `PlayOneShot()`.
- **Sàn di chuyển (Moving Platform)**:
  - Di chuyển tự động tịnh tiến giữa điểm A và B.
  - Sử dụng cơ chế gán quan hệ cha-con (`transform.SetParent(transform)` / `SetParent(null)`) khi Player bước lên hoặc rời khỏi sàn để không bị trượt.
- **Main Menu**:
  - Scene riêng biệt với nút **Play** (bắt đầu chơi) và **Quit** (`Application.Quit()`).
  - Tùy chỉnh giao diện đẹp mắt bằng `Font Asset Creator` của TextMeshPro.
- **Build & Xuất bản**:
  - Cấu hình Build Settings: Scene `MainMenu` ở Index `0`, Scene chơi ở Index `1`.
  - Xuất ra file thực thi `.exe` chạy độc lập trên Windows.

---

## 📜 Danh sách Script C# và chức năng

| Tên Script | Component / GameObject áp dụng | Chức năng chính |
| :--- | :--- | :--- |
| `PlayerController.cs` | GameObject `Player` | Xử lý di chuyển trái/phải, nhảy tiếp đất, lật hướng, cập nhật Animator. |
| `GameManager.cs` | GameObject `GameManager` | Quản lý điểm số, trạng thái Game Over, You Win, dừng/tiếp tục thời gian (`Time.timeScale`). |
| `AudioManager.cs` | GameObject `AudioManager` | Phát nhạc nền (BGM) và hiệu ứng âm thanh (SFX) cho toàn game. |
| `EnemyPatrol.cs` | GameObject `Enemy` | Quái vật tự động đi qua lại giữa PointA và PointB, va chạm Player gây Game Over. |
| `MovingPlatform.cs`| GameObject `MovingPlatform` | Sàn nâng tự động, gán Player làm Child để đứng vững khi di chuyển. |
| `Coin.cs` | Prefab `Coin` | Phát hiện va chạm với Player, cộng điểm và xóa vật thể. |
| `Hazard.cs` | GameObject `Trap` / Bẫy | Kích hoạt Game Over khi Player rơi vào bẫy. |
| `KeyItem.cs` | GameObject `Key` | Kích hoạt màn hình chiến thắng (Win) khi Player nhặt được chìa khóa. |
| `MainMenuController.cs`| Canvas của Scene `MainMenu` | Điều khiển nút Chơi game (chuyển scene) và Thoát game. |

---

## 🕹️ Phím bấm điều khiển (Controls)

| Phím bấm | Hành động |
| :--- | :--- |
| `A` / `D` hoặc `Mũi tên Trái` / `Phải` | Di chuyển nhân vật sang trái / phải |
| `Space` (Phím cách) hoặc `W` | Nhảy lên (khi đang đứng trên mặt đất) |
| `Esc` (hoặc Click chuột) | Tương tác với các nút trên bảng UI Menu |

---

## 📦 Hướng dẫn cài đặt & Build Game

### 1. Mở và chạy thử trong Unity:
1. Mở **Unity Hub** ➜ Bấm **Open** ➜ Chọn thư mục dự án này.
2. Mở Scene chính: `Assets/Scenes/MainMenu.unity` hoặc `Level01.unity`.
3. Nhấn nút **Play** (`Ctrl + P`) ở giữa phía trên màn hình để trải nghiệm.

### 2. Xuất bản game (.exe):
1. Vào menu **File** > **Build Settings...** (`Ctrl + Shift + B`).
2. Kiểm tra danh sách **Scenes In Build**:
   - `0: Assets/Scenes/MainMenu.unity`
   - `1: Assets/Scenes/Level01.unity`
3. Chọn mục **Target Platform**: `Windows`.
4. Bấm **Build** ➜ Chọn thư mục lưu trữ (ví dụ: `Builds/`) ➜ Unity sẽ tạo ra file `<TênGame>.exe` hoàn chỉnh.

---
*Chúc bạn thực hiện bài Lab thành công và có một tựa game 2D Platformer hoàn chỉnh!*
