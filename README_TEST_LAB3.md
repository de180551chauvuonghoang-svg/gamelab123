# 🧪 HƯỚNG DẪN CHI TIẾT TEST NGHIỆM THU LAB 3 (UNITY 2D PLATFORMER)

Tài liệu này hướng dẫn chi tiết từng bước kiểm thử (testing), kiểm tra tính năng và nghiệm thu bài **Lab 3** trực tiếp trong Unity Editor và bản Build độc lập theo đúng các tiêu chí đánh giá của đề bài: **AudioManager (BGM & SFX)**, **Moving Platform (Sàn di chuyển)**, **Main Menu Scene** và **Build game .EXE**.

---

## 🚀 BƯỚC 1: CHUẨN BỊ MÔI TRƯỜNG TEST (1 CLICK TỰ ĐỘNG)

1. Mở cửa sổ **Unity Editor**.
2. Nhìn lên thanh Menu cao nhất (góc trên cùng):
   - Bấm vào menu **`Lab 3`** ➜ Chọn **`Chạy Cài Đặt Tự Động Toàn Bộ Lab 3 (One-Click Setup)`**.
   - Hộp thoại xuất hiện báo *"Đã thiết lập toàn bộ nội dung Lab 3 thành công rực rỡ!"* ➜ Bấm **Tuyệt vời!**.
3. Công cụ tự động sẽ:
   - Cấu hình toàn bộ file âm thanh (`bgm.wav`, `jump.wav`, `coin.wav`, `gameover.wav`, `win.wav`, `click.wav`) trong `Assets/Audio/`.
   - Cấu hình Sprite pixel art cho Sàn di chuyển `MovingPlatform.png`.
   - Thiết lập `AudioManager` với 2 AudioSource độc lập trong màn chơi.
   - Tạo Sàn di chuyển (Moving Platform) qua lại giữa 2 điểm A & B, gắn cơ chế `SetParent`.
   - Tạo mới Scene **`Assets/Scenes/MainMenu.unity`** với đầy đủ giao diện Title, nút **Play Game** và **Thoát Game**.
   - Cập nhật sẵn **Build Settings**: `MainMenu.unity` (Index 0) và `SampleScene.unity` (Index 1).
   - Tự động mở Scene `MainMenu.unity` để bạn test ngay.

---

## 📋 CHI TIẾT TỪNG KỊCH BẢN KIỂM THỬ (TEST CASES)

---

### 🔹 Test Case 1: Kiểm tra Giao diện Main Menu Scene & Nhạc Nền BGM
- **Mục tiêu:** Màn hình Menu chính hiển thị thẩm mỹ cao, có đầy đủ tiêu đề, phụ đề, nút chức năng và phát nhạc nền ngay khi mở.
- **Thao tác:**
  1. Trong cửa sổ **Project**, mở Scene `Assets/Scenes/MainMenu.unity`.
  2. Bấm nút **Play (▶)** ở đỉnh giữa Unity.
- **Kết quả đạt chuẩn:**
  - Nhạc nền chiptune 8-bit sống động (`bgm.wav`) tự động cất lên mượt mà.
  - Giữa màn hình xuất hiện khung Menu hiện đại tông màu Midnight Blue sang trọng:
    - Tiêu đề vàng kim: **`ADVENTURE HERO 2D`**.
    - Phụ đề thông tin: `★ BÀI THỰC HÀNH LAB 3 - NÂNG CAO ★`.
    - Hai nút bấm lớn: nút xanh lá **`BẮT ĐẦU CHƠI (PLAY)`** và nút đỏ **`THOÁT GAME (QUIT)`**.

---

### 🔹 Test Case 2: Kiểm tra nút "BẮT ĐẦU CHƠI" (Play Game)
- **Mục tiêu:** Nút Play phát âm thanh click và chuyển cảnh sang màn chơi chính `SampleScene`.
- **Thao tác:**
  1. Dùng chuột click vào nút **`BẮT ĐẦU CHƠI (PLAY)`**.
- **Kết quả đạt chuẩn:**
  - Nghe thấy tiếng phản hồi click giòn giã (`click.wav`).
  - Unity lập tức chuyển mượt mà từ MainMenu sang `SampleScene`.
  - Nhạc nền BGM vẫn tiếp tục ngân vang không bị ngắt quãng nhờ cơ chế `DontDestroyOnLoad`.
  - Nhân vật Player xuất hiện sẵn sàng nhận lệnh điều khiển.

---

### 🔹 Test Case 3: Kiểm tra Hiệu ứng Âm thanh Nhảy (Jump SFX)
- **Mục tiêu:** Kiểm tra AudioManager phát âm thanh nhảy mỗi khi Player thực hiện nhảy từ mặt đất.
- **Thao tác:**
  1. Điều khiển Player, bấm phím **`Space`** (hoặc `W` / phím Mũi tên Lên).
- **Kết quả đạt chuẩn:**
  - Nhân vật nhảy vút lên cao, đồng thời cất lên âm thanh vút cao retro đặc trưng (`jump.wav`).
  - Âm thanh phát bằng `PlayOneShot()` trên kênh `sfxSource`, không làm gián đoạn hay ngắt nhạc nền BGM.
  - Khi đang ở trên không trung mà bấm tiếp phím nhảy: Không nhảy và không phát lại tiếng nhảy (đảm bảo đúng cơ chế GroundCheck).

---

### 🔹 Test Case 4: Kiểm tra Hiệu ứng Âm thanh Thu thập Coin (Coin SFX)
- **Mục tiêu:** Kiểm tra âm thanh keng keng vui tai phát ra ngay khi chạm đồng xu vàng.
- **Thao tác:**
  1. Điều khiển Player chạy qua ăn đồng xu `Coin_1`, `Coin_2` trên đường đi.
- **Kết quả đạt chuẩn:**
  - Mỗi lần chạm một đồng xu, tiếng chuông leng keng 2 tông trong trẻo (`coin.wav`) lập tức vang lên.
  - Điểm số `Score` trên góc trái tăng lên thời gian thực.
  - Đồng xu biến mất ngay lập tức.
  - Ăn liên tiếp nhiều đồng xu thì âm thanh vẫn vang lên đầy đủ, không bị nuốt tiếng nhờ cơ chế `PlayOneShot()`.

---

### 🔹 Test Case 5: Kiểm tra Sàn Di Chuyển Tuần Tra Tự Động (Moving Platform A <-> B)
- **Mục tiêu:** Sàn cơ khí nổi tự động trượt qua lại nhịp nhàng giữa 2 mốc toạ độ PointA và PointB.
- **Thao tác:**
  1. Điều khiển Player vượt qua hàng bẫy gai, vượt qua cột tường và né quái vật tuần tra.
  2. Dừng lại quan sát khu vực sàn lơ lửng phía trước (toạ độ x = 16.5 đến 22.0).
- **Kết quả đạt chuẩn:**
  - Tấm sàn kim loại ánh xanh neon `MovingPlatform` tự động trượt ngang đều đặn từ điểm mốc A sang điểm mốc B với tốc độ ổn định.
  - Khi đến điểm mốc, sàn dừng nhẹ 0.5 giây tạo cảm giác cơ học chân thực rồi tự động đổi chiều quay về điểm xuất phát.
  - Quá trình lặp đi lặp lại tuần hoàn, liên tục không ngừng.

---

### 🔹 Test Case 6: Kiểm tra Cơ Chế Đứng Lên Sàn (SetParent - Không Trượt Khỏi Sàn)
- **Mục tiêu:** Khi Player đứng trên sàn, Player được gán làm con của Platform (`SetParent`) và di chuyển đồng tốc cùng sàn.
- **Thao tác:**
  1. Canh lúc sàn di chuyển lại gần, điều khiển Player nhảy lên đặt chân lên mặt sàn.
  2. **Thả tay hoàn toàn khỏi bàn phím** (không bấm phím A, D hay mũi tên).
- **Kết quả đạt chuẩn:**
  - Nhân vật Player đứng vững vàng trên mặt sàn và **tự động trượt ngang cùng với sàn** sang bờ bên kia vực!
  - Player **hoàn toàn không bị trượt té hay rơi khỏi sàn** khi sàn di chuyển.
  - Nhìn vào cửa sổ **Hierarchy** lúc này: GameObject `Player` đã tự động trở thành con (`Child`) nằm thụt lề bên dưới `MovingPlatform`.

---

### 🔹 Test Case 7: Kiểm tra Cơ Chế Rời Sàn (SetParent(null))
- **Mục tiêu:** Khi Player nhảy ra khỏi sàn, quan hệ cha-con lập tức được hủy bỏ để Player hoạt động độc lập bình thường.
- **Thao tác:**
  1. Khi sàn chở Player đến gần bờ bên kia (điểm B), bấm **`Space`** để nhảy vút khỏi sàn sang bờ đất.
- **Kết quả đạt chuẩn:**
  - Player nhảy sang bờ đất nhẹ nhàng, tiếp đất an toàn.
  - Nhìn vào Hierarchy: `Player` lập tức thoát ra khỏi `MovingPlatform`, trở về làm GameObject gốc độc lập (`SetParent(null)`).
  - Nhân vật tiếp tục di chuyển, chạy nhảy bình thường.

---

### 🔹 Test Case 8: Kiểm tra Chinh Phục Chìa Khóa (Win Game) & Quay Về Main Menu
- **Mục tiêu:** Sau khi đi qua sàn di chuyển, người chơi nhặt chìa khóa chiến thắng và có thể quay về Main Menu.
- **Thao tác:**
  1. Sau khi đáp sang bờ đất bên kia sàn di chuyển, chạm vào chiếc **Chìa khóa vàng** (`Key_WinItem`).
  2. Bảng **YOU WIN!** hiện lên kèm âm thanh chiến thắng hân hoan (`win.wav`).
  3. Dùng chuột click vào nút **`Menu Chính`** trên bảng.
- **Kết quả đạt chuẩn:**
  - Tiếng click vang lên.
  - Game lập tức tải lại màn hình **MainMenu Scene**.
  - Nhạc nền tiếp tục ngân vang, sẵn sàng cho lượt chơi mới!

---

### 🔹 Test Case 9: Kiểm tra Thiết Lập Build Settings
- **Mục tiêu:** Danh sách Scene trong Build Settings được cấu hình chuẩn mực theo thứ tự ưu tiên.
- **Thao tác:**
  1. Trên thanh công cụ Unity, chọn **`File`** ➜ **`Build Settings...`** (hoặc tổ hợp phím `Ctrl + Shift + B`).
  2. Quan sát khung danh sách **`Scenes In Build`**.
- **Kết quả đạt chuẩn:**
  - Vị trí số **0**: `Assets/Scenes/MainMenu.unity` (được tích dấu chọn).
  - Vị trí số **1**: `Assets/Scenes/SampleScene.unity` (được tích dấu chọn).
  - Nền tảng mục tiêu (Platform): `Windows, Mac, Linux` (Standalone).

---

### 🔹 Test Case 10: Kiểm tra Xuất Bản File Thực Thi (.EXE Standalone Build)
- **Mục tiêu:** Đóng gói toàn bộ trò chơi thành file `.exe` chạy độc lập ngoài hệ điều hành Windows mà không cần mở Unity.
- **Thao tác:**
  1. Trên thanh Menu Unity: Chọn **`Lab 3`** ➜ Chọn **`5. Xuất Bản Game Độc Lập (.EXE Standalone Build)`**.
  2. Chờ Unity biên dịch và đóng gói trong khoảng 30-60 giây.
  3. Sau khi hoàn tất, thư mục **`Build/`** sẽ tự động mở ra trên Windows Explorer chứa file **`GameLab3.exe`**.
  4. Nhấp đúp chuột vào file **`GameLab3.exe`** để chạy trực tiếp trên Windows!
- **Kết quả đạt chuẩn:**
  - Cửa sổ Game mở lên mượt mà với độ phân giải sắc nét.
  - Màn hình Menu chính xuất hiện đầu tiên, nhạc nền BGM vang lên.
  - Bấm nút **BẮT ĐẦU CHƠI**: Chuyển vào chơi game, âm thanh nhảy và ăn xu đầy đủ, sàn di chuyển mượt mà.
  - Bấm nút **THOÁT GAME (QUIT)** ở Main Menu: Trò chơi tự động đóng hoàn toàn dứt khoát.

---

## 📊 BẢNG TỔNG KẾT CHECKLIST NGHIỆM THU LAB 3

| STT | Hạng mục kiểm tra | Thao tác kiểm thử | Tiêu chuẩn đánh giá | Trạng thái |
| :---: | :--- | :--- | :--- | :---: |
| **1** | **AudioManager Architecture** | Kiểm tra GameObject `AudioManager` | Có 2 AudioSource: 1 nguồn BGM Loop, 1 nguồn SFX PlayOneShot | [x] Đạt |
| **2** | **Background Music (BGM)** | Bấm Play game | Nhạc nền 8-bit tự phát, lặp liên tục, xuyên suốt các scene | [x] Đạt |
| **3** | **Player Jump SFX** | Bấm phím Space / W | Phát âm thanh `jump.wav` mỗi lần nhảy từ mặt đất | [x] Đạt |
| **4** | **Coin Collect SFX** | Chạy qua ăn đồng xu | Phát âm thanh `coin.wav` trong trẻo, không ngắt BGM | [x] Đạt |
| **5** | **Moving Platform A <-> B** | Quan sát sàn lơ lửng | Sàn tự động trượt đều đặn giữa 2 mốc toạ độ A và B | [x] Đạt |
| **6** | **Platform Parenting** | Nhảy đứng lên mặt sàn | Gọi `SetParent()`, Player di chuyển đồng bộ cùng sàn | [x] Đạt |
| **7** | **Platform Un-Parenting** | Nhảy rời khỏi mặt sàn | Gọi `SetParent(null)`, Player trở lại trạng thái độc lập | [x] Đạt |
| **8** | **Main Menu Scene** | Mở Scene `MainMenu.unity` | Có Title, Subtitle, nút Play, nút Quit, BGM và UI TMP | [x] Đạt |
| **9** | **Main Menu Navigation** | Bấm Play / Quit / Menu Chính | Chuyển cảnh chính xác giữa MainMenu và SampleScene | [x] Đạt |
| **10** | **Build Settings & .EXE** | Kiểm tra Build Settings & thư mục `Build/` | MainMenu ở vị trí 0, SampleScene vị trí 1; xuất file `GameLab3.exe` chạy mượt mà | [x] Đạt |

---

## 💡 MẸO XỬ LÝ & TỐI ƯU KHI TEST LAB 3:

1. **Nếu không nghe thấy âm thanh trong Unity Editor:**
   - Hãy kiểm tra góc trên bên phải của cửa sổ **Game** xem nút loa **`Mute Audio`** có đang bị bật màu xanh không. Nếu có, hãy click để tắt Mute.
2. **Muốn chỉnh tốc độ sàn di chuyển:**
   - Chọn GameObject `MovingPlatform` trong Hierarchy ➜ Chỉnh giá trị `Speed` trong Component `Moving Platform` ở cửa sổ Inspector (mặc định là `2.5`).
3. **Muốn thay đổi khoảng cách di chuyển của sàn:**
   - Trong Hierarchy, mở rộng `Moving_Platform_Group`, chọn `PointA` hoặc `PointB` và dùng công cụ Move Tool (`W`) kéo sang vị trí mới theo ý bạn!
