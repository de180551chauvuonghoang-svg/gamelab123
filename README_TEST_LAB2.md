# 🧪 HƯỚNG DẪN CHI TIẾT TEST NGHIỆM THU LAB 2 (UNITY 2D PLATFORMER)

Tài liệu này hướng dẫn chi tiết từng bước kiểm thử (testing), kiểm tra tính năng và nghiệm thu bài **Lab 2** trực tiếp trong Unity Editor theo đúng các tiêu chí đánh giá của đề bài.

---

## 🚀 BƯỚC 1: CHUẨN BỊ MÔI TRƯỜNG TEST (1 CLICK)

1. Mở cửa sổ **Unity Editor**.
2. Nhìn lên thanh Menu cao nhất (góc trên bên trái):
   - Bấm vào menu **`Lab 2`** ➜ Chọn **`Chạy Cài Đặt Tự Động Lab 2 (One-Click Setup)`**.
   - Chờ 1 giây để bảng thông báo *"Đã thiết lập toàn bộ nội dung Lab 2 thành công!"* xuất hiện ➜ Nhấn **OK** (hoặc *Tuyệt vời*).
3. **Bắt đầu Test:**
   - Nhấn nút **Play (▶)** ở đỉnh giữa màn hình Unity.
   - **Click chuột 1 lần vào cửa sổ Game** để bàn phím nhận diện điều khiển.

---

## 📋 CHI TIẾT TỪNG KỊCH BẢN KIỂM THỬ (TEST CASES)

---

### 🔹 Test Case 1: Kiểm tra Camera Follow (Camera bám theo Player)
- **Mục tiêu:** Camera tự động tịnh tiến bám theo nhân vật trên bản đồ dài, không bị rung giật.
- **Thao tác:**
  1. Bấm và giữ phím **`D`** để điều khiển Player chạy liên tục về phía bên phải.
- **Kết quả đạt chuẩn:**
  - Cửa sổ Game mở rộng theo bước chạy của nhân vật: Camera tự động trượt mượt mà sang phải, giữ nhân vật luôn ở vị trí trung tâm tầm nhìn.
  - Khi nhân vật nhảy lên cao, Camera cũng nâng nhẹ lên theo trục Y để người chơi thấy rõ khoảng không bên trên.

---

### 🔹 Test Case 2: Kiểm tra cấu trúc Tilemap & Composite Collider 2D
- **Mục tiêu:** Kiểm tra cấu trúc phân lớp bản đồ và tính mượt mà của bề mặt va chạm sàn đất.
- **Quan sát & Thao tác:**
  1. Trong cửa sổ **Hierarchy**, kiểm tra GameObject `Grid` có đủ 3 lớp Tilemap: `Background`, `Ground_Tilemap`, `Foreground`.
  2. Tại `Ground_Tilemap`, kiểm tra đã có `Tilemap Collider 2D` (bật `Used By Composite`) và `Composite Collider 2D` (`Rigidbody 2D: Static`).
  3. Cho Player chạy liên tục trên sàn đất từ đầu đường đến cuối đường.
- **Kết quả đạt chuẩn:**
  - Nhân vật chạy lướt trên sàn phẳng mượt mà, **tuyệt đối không bị vấp ngã hay giật cục** tại các khe tiếp giáp giữa các ô gạch.

---

### 🔹 Test Case 3: Kiểm tra thu thập Coin Prefab & TextMeshPro UI
- **Mục tiêu:** Kiểm tra cơ chế `OnTriggerEnter2D`, đồng xu tự xoay, cộng điểm thời gian thực và tự xóa bằng `Destroy()`.
- **Thao tác:**
  1. Nhìn lên góc trên bên trái màn hình: Ban đầu hiển thị dòng chữ màu vàng **`Score: 0`**.
  2. Điều khiển Player chạy qua chạm vào đồng xu vàng thứ 1 và thứ 2 (`Coin_1`, `Coin_2`).
  3. Nhảy lên đỉnh cột tường gạch để ăn đồng xu thứ 3 (`Coin_3`).
- **Kết quả đạt chuẩn:**
  - Các đồng xu có hiệu ứng tự xoay nhẹ quanh trục Y trông rất sinh động.
  - Khi Player chạm vào, đồng xu biến mất ngay lập tức (không cản bước chạy của Player).
  - Điểm số trên giao diện TextMeshPro tự động nhảy số theo thời gian thực: `Score: 1` ➜ `Score: 2` ➜ `Score: 3`...
  - Trong thư mục `Assets/Prefabs/` đã có sẵn file `Coin.prefab`.

---

### 🔹 Test Case 4: Kiểm tra Bẫy gai (Trap/Hazard) & Màn hình Game Over
- **Mục tiêu:** Kiểm tra vùng cảm ứng bẫy gai kích hoạt trạng thái Game Over và đóng băng game.
- **Thao tác:**
  1. Điều khiển Player chạy đến vị trí hàng chông gai sắt nhọn (`Trap_Spikes`).
  2. Cho Player bước chân hoặc nhảy rơi trúng vào hàng gai.
- **Kết quả đạt chuẩn:**
  - Bảng màu đen mờ **GAME OVER** xuất hiện lập tức giữa màn hình với dòng chữ đỏ rực rỡ.
  - Toàn bộ chuyển động trong game lập tức dừng lại (`Time.timeScale = 0f`), Player và cảnh vật đóng băng, không thể di chuyển tiếp.
  - Trên bảng có 2 nút lựa chọn: **`Thử Lại`** (`Play Again`) và **`Menu Chính`** (`Main Menu`).

---

### 🔹 Test Case 5: Kiểm tra nút "Thử Lại" (Play Again)
- **Mục tiêu:** Kiểm tra hàm `PlayAgain()` tải lại màn chơi và phục hồi thời gian (`Time.timeScale = 1f`).
- **Thao tác:**
  1. Khi đang ở bảng **GAME OVER**, dùng chuột click vào nút **`Thử Lại`**.
- **Kết quả đạt chuẩn:**
  - Màn chơi lập tức được làm mới và bắt đầu lại từ đầu.
  - Player hồi sinh về vị trí xuất phát ban đầu.
  - Điểm số reset về lại `Score: 0`.
  - Game hoạt động bình thường, nhân vật di chuyển và nhảy lại bình thường (thời gian đã được rã đông).

---

### 🔹 Test Case 6: Kiểm tra Quái vật tuần tra (Enemy Patrol A-B)
- **Mục tiêu:** Quái vật di chuyển tự động giữa 2 điểm mốc, tự động quay mặt và gây Game Over khi va chạm.
- **Thao tác:**
  1. Đứng yên quan sát quái vật màu đỏ (`Enemy`) ở phía bên phải cột tường:
     - Quái tự động đi từ điểm A sang điểm B.
     - Khi đến gần điểm B, quái tự động quay mặt sang trái và đi ngược lại về điểm A.
     - Khi chạm điểm A, quái lại quay mặt sang phải và tiếp tục lặp lại chu kỳ.
  2. Điều khiển Player nhảy vào chạm trúng quái vật.
- **Kết quả đạt chuẩn:**
  - Chuyển động tuần tra của quái diễn ra đều đặn, mượt mà và tự lật mặt chuẩn xác.
  - Khi va chạm với Player, bảng **GAME OVER** xuất hiện ngay lập tức.

---

### 🔹 Test Case 7: Kiểm tra Chìa khóa (Key Item) & Màn hình YOU WIN!
- **Mục tiêu:** Kiểm tra vòng lặp chiến thắng hoàn chỉnh của màn chơi.
- **Thao tác:**
  1. Điều khiển Player khéo léo:
     - Nhảy qua hàng bẫy gai.
     - Nhảy vượt qua cột tường gạch.
     - Nhảy né quái vật đang tuần tra.
  2. Chạy về cuối con đường, chạm vào chiếc **Chìa khóa vàng** đang bập bồng (`Key_WinItem`).
- **Kết quả đạt chuẩn:**
  - Chìa khóa vàng có hiệu ứng nhấp nhô lơ lửng trên không trung.
  - Khi Player chạm vào, chiếc chìa khóa biến mất.
  - Bảng vàng rực rỡ **`YOU WIN!`** hiện ra giữa màn hình!
  - Trò chơi dừng lại báo hiệu chiến thắng trọn vẹn màn chơi.
  - Nhấn nút **`Chơi Lại`** trên bảng Win ➜ game tải lại để bạn tiếp tục chơi lại từ đầu.

---

## 📊 BẢNG TỔNG KẾT CHECKLIST NGHIỆM THU LAB 2

Bạn hãy đánh dấu `[x]` vào bảng dưới đây khi hoàn thành từng tiêu chí:

| STT | Hạng mục kiểm tra | Thao tác kiểm thử | Tiêu chuẩn đánh giá | Đạt / Chưa |
| :---: | :--- | :--- | :--- | :---: |
| **1** | **Camera Follow** | Chạy sang phải trên bản đồ dài | Camera lướt theo Player mượt mà, bao quát cảnh quan | [ ] Đạt |
| **2** | **Cấu trúc Tilemap** | Kiểm tra Grid & chạy trên sàn | Có đủ Background, Ground, Foreground; Composite Collider phẳng, không kẹt mép gạch | [ ] Đạt |
| **3** | **Coin & Điểm số** | Chạy qua ăn các đồng xu | Coin xoay nhẹ, biến mất khi chạm; TextMeshPro UI tăng điểm liên tục | [ ] Đạt |
| **4** | **Coin Prefab** | Kiểm tra trong `Assets/Prefabs/` | Đã đóng gói thành file `Coin.prefab` tái sử dụng | [ ] Đạt |
| **5** | **Bẫy gai (Trap)** | Nhảy rơi vào bẫy gai | Kích hoạt Game Over, dừng chuyển động game | [ ] Đạt |
| **6** | **Quái tuần tra (Enemy)** | Quan sát quái đi lại | Quái tự đi qua lại giữa PointA và PointB, tự đổi hướng quay mặt | [ ] Đạt |
| **7** | **Va chạm Quái** | Chạm người vào quái | Kích hoạt Game Over | [ ] Đạt |
| **8** | **Nút Play Again** | Bấm nút `Thử Lại` / `Chơi Lại` | Tải lại màn chơi thành công, thời gian tiếp tục chạy bình thường | [ ] Đạt |
| **9** | **Chìa khóa (Key Item)** | Chạy về đích nhặt chìa khóa | Chìa khóa biến mất, màn hình **YOU WIN!** hiển thị rực rỡ | [ ] Đạt |
| **10** | **Mẫu Singleton** | Kiểm tra code `GameManager.cs` | Sử dụng `public static GameManager instance` tập trung | [ ] Đạt |

---

## 💡 MẸO XỬ LÝ KHI TEST LAB 2:

- **Nếu không ăn được Coin hoặc không dính bẫy gai:**  
  ➜ Hãy chắc chắn rằng GameObject `Player` đã được đặt **Tag = "Player"** (ở góc trên cùng bên trái Inspector của Player). Công cụ tự động đã gán sẵn cho bạn, nhưng nếu bạn tạo Player mới thì cần kiểm tra lại mục này.
- **Muốn chỉnh quái vật đi nhanh hơn hoặc phạm vi tuần tra rộng hơn:**  
  ➜ Chọn GameObject `Enemy` trong Hierarchy, chỉnh biến `Speed` trong Inspector. Muốn mở rộng quãng đường, bạn chỉ cần kéo xa vị trí của `PointA` và `PointB`.
- **Muốn thêm nhiều đồng xu nữa:**  
  ➜ Mở thư mục `Assets/Prefabs/`, kéo thả trực tiếp `Coin.prefab` vào màn hình Scene ở bất kỳ vị trí nào bạn muốn!
