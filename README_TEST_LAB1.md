# 🧪 HƯỚNG DẪN CHI TIẾT CÁCH TEST BÀI LAB 1 (UNITY)

Tài liệu này hướng dẫn chi tiết từng bước cách kiểm tra (test) toàn bộ các tính năng của **Lab 1** trực tiếp trên Unity Editor để nghiệm thu bài làm.

---

## 🚀 BƯỚC CHUẨN BỊ TRƯỚC KHI TEST

1. **Tắt chế độ Play** (nếu nút Play ▶ đang sáng màu xanh thì bấm vào để tắt).
2. **Xóa log lỗi cũ**: 
   - Mở tab **Console** (ở dưới hoặc cạnh tab Project).
   - Bấm nút **`Clear`** ở góc trên bên trái của cửa sổ Console để làm sạch các thông báo cũ.
3. **Bắt đầu Test**:
   - Nhấn nút **Play (▶)** ở đỉnh giữa màn hình Unity.
   - **Click chuột 1 lần vào màn hình Game** (cửa sổ bên dưới) để hệ thống nhận diện bàn phím của bạn.

---

## 📋 CHI TIẾT TỪNG KỊCH BẢN TEST (TEST CASES)

### 🔹 Test Case 1: Kiểm tra di chuyển ngang
- **Mục tiêu**: Kiểm tra Player nhận tín hiệu di chuyển mượt mà qua lại bằng Rigidbody 2D.
- **Thao tác**:
  1. Nhấn và giữ phím **`D`** (hoặc phím mũi tên sang phải `→`).
  2. Thả tay ra.
  3. Nhấn và giữ phím **`A`** (hoặc phím mũi tên sang trái `←`).
- **Kết quả đạt chuẩn**:
  - Bấm `D`: Nhân vật chạy đều sang phải.
  - Thả phím: Nhân vật dừng lại ngay, không bị trượt trôi dài.
  - Bấm `A`: Nhân vật chạy đều sang trái.

---

### 🔹 Test Case 2: Kiểm tra tự động lật hướng mặt (Flip Sprite)
- **Mục tiêu**: Kiểm tra hàm `Flip()` đảo chiều `transform.localScale.x` chuẩn xác.
- **Thao tác**:
  1. Bấm phím **`D`** để nhân vật đi sang phải.
  2. Bấm phím **`A`** để nhân vật quay sang trái.
- **Quan sát**:
  - Khi đi sang phải: Mắt và mặt của nhân vật nhìn về phía bên phải.
  - Khi đi sang trái: Mắt và mặt của nhân vật lập tức lật ngược nhìn về phía bên trái.
  - Thân hình nhân vật giữ nguyên tỷ lệ, không bị méo hay co rúm.

---

### 🔹 Test Case 3: Kiểm tra cơ chế nhảy và tiếp đất (Grounded Check)
- **Mục tiêu**: Kiểm tra `Physics2D.OverlapCircle` nhận diện đúng sàn đất `Ground` và áp dụng lực nhảy lên trục Y.
- **Thao tác**:
  1. Cho nhân vật đứng yên trên sàn đất.
  2. Bấm phím **`Space`** (Phím cách) 1 lần.
- **Kết quả đạt chuẩn**:
  - Nhân vật nhảy vọt lên cao một khoảng vừa phải rồi rơi xuống sàn tự nhiên nhờ trọng lực 2D.
  - Khi chạm mặt sàn đất, nhân vật đứng vững, không bị lún hay rơi xuyên qua sàn.

---

### 🔹 Test Case 4: Kiểm tra chống nhảy vô hạn trên không (Anti Double-Jump)
- **Mục tiêu**: Đảm bảo nhân vật **chỉ được phép nhảy khi chân đang chạm đất**, tránh bug "nhảy trên mây".
- **Thao tác**:
  1. Bấm **`Space`** để nhân vật nhảy lên.
  2. **Khi nhân vật đang ở trên không trung (chưa rơi chạm đất), bạn bấm liên tục phím `Space` nhiều lần**.
- **Kết quả đạt chuẩn**:
  - Nhân vật hoàn toàn phớt lờ các lần bấm phím tiếp theo và tiếp tục rơi xuống đất bình thường.
  - Chỉ khi chân nhân vật chạm lại sàn đất, lần bấm `Space` tiếp theo mới có tác dụng.

---

### 🔹 Test Case 5: Kiểm tra chống dính tường (Zero Friction Material)
- **Mục tiêu**: Kiểm tra vật liệu `ZeroFriction.physicsMaterial2D` (Friction = 0) hoạt động trên Box Collider 2D của Player.
- **Thao tác**:
  1. Điều khiển nhân vật di chuyển lại gần cột tường gạch bên phải (**`Wall_Test`**).
  2. Bấm **`Space`** để nhảy lên, đồng thời **nhấn giữ phím `D`** để ép sát người nhân vật vào bề mặt tường gạch.
- **Kết quả đạt chuẩn**:
  - Nhân vật **không bị dính lơ lửng** trên tường dù bạn vẫn đang nhấn đè phím `D`.
  - Nhân vật trượt êm xuôi theo mép tường và rơi chạm xuống đất tự nhiên.

---

### 🔹 Test Case 6: Kiểm tra chuyển đổi trạng thái Animation
- **Mục tiêu**: Kiểm tra Animator Controller nhận các biến `isRunning` và `isJumping`.
- **Thao tác**:
  1. Mở cửa sổ Animator: Vào menu **Window** ➜ **Animation** ➜ **Animator** (kéo tab này đặt cạnh tab Game để vừa chơi vừa quan sát).
  2. Bấm chọn đối tượng **Player** trong bảng Hierarchy.
  3. Bấm **Play (▶)** và quan sát thanh tiến trình màu xanh trên các ô trạng thái:
     - **Khi đứng yên**: Ô `Player_Idle` đang chạy vòng lặp; tham số `isRunning = false`, `isJumping = false`.
     - **Khi nhấn di chuyển (`A`/`D`)**: Chuyển ngay sang ô `Player_Run`; tham số `isRunning = true`.
     - **Khi nhấn nhảy (`Space`)**: Chuyển ngay sang ô `Player_Jump`; tham số `isJumping = true`.
     - **Khi chạm đất**: Lập tức chuyển ngược về `Player_Idle` hoặc `Player_Run` mượt mà không bị delay.

---

## 📊 BẢNG TỔNG KẾT CHECKLIST NGHIỆM THU LAB 1

Bạn hãy tích chọn vào bảng này sau khi thực hiện xong các bước test:

| STT | Hạng mục kiểm tra | Phím thao tác | Tiêu chí đạt | Đánh giá |
| :---: | :--- | :---: | :--- | :---: |
| 1 | Di chuyển ngang | `A` / `D` | Chạy mượt sang 2 bên, nhả phím dừng lại | [ ] Đạt |
| 2 | Lật hướng nhân vật | `A` / `D` | Mắt quay đúng hướng di chuyển trái/phải | [ ] Đạt |
| 3 | Nhảy cơ bản | `Space` | Nhảy lên và rơi xuống đất bình thường | [ ] Đạt |
| 4 | Chống nhảy trên không | Bấm `Space` liên tục | Không thể nhảy lần 2 khi đang ở trên không | [ ] Đạt |
| 5 | Không dính tường | Nhảy + ép vào tường | Trượt rơi xuống đất, không bị kẹt lơ lửng | [ ] Đạt |
| 6 | Animator Controller | Quan sát tab Animator | Trạng thái chuyển đổi nhịp nhàng Idle/Run/Jump | [ ] Đạt |

---

## 💡 MẸO XỬ LÝ NHANH NẾU GẶP VẤN ĐỀ KHI TEST

- **Không nhận phím điều khiển?** ➜ Hãy nhớ **click chuột vào cửa sổ Game** để màn hình game nhận tiêu điểm bàn phím.
- **Muốn chỉnh nhân vật nhảy cao hơn hoặc chạy nhanh hơn?** ➜ Chọn đối tượng `Player` trong Hierarchy, nhìn sang Inspector tại component `PlayerController`:
  - Tăng `Move Speed` (ví dụ từ `8` lên `10`) để chạy nhanh hơn.
  - Tăng `Jump Force` (ví dụ từ `13` lên `15`) để nhảy cao hơn.
