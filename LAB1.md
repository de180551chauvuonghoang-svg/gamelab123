# 📘 HƯỚNG DẪN CHI TIẾT BÀI LAB 1: UNITY 2D PLATFORMER

> **Mục tiêu bài Lab 1:**
> - Khởi tạo dự án Unity 2D chuẩn và sắp xếp tài nguyên khoa học.
> - Tạo nhân vật chính (Player), cấu hình Sorting Layer và thiết lập vật lý 2D (`Rigidbody 2D`, `Box Collider 2D`).
> - Triệt tiêu ma sát dính tường bằng `Physics Material 2D (Friction = 0)`.
> - Viết script `PlayerController.cs` điều khiển di chuyển ngang, tự động lật hướng (`transform.localScale`), cơ chế nhảy an toàn với `Physics2D.OverlapCircle` (chỉ nhảy khi chạm đất).
> - Xây dựng hệ thống Animation hoàn chỉnh: Tạo Clip (`Idle`, `Run`, `Jump`), cấu hình `Animator Controller`, cài đặt `Parameters` và `Transitions` mượt mà.

---

## 📑 MỤC LỤC
1. [Bước 1: Khởi tạo dự án & Cấu trúc thư mục Assets](#bước-1-khởi-tạo-dự-án--cấu-trúc-thư-mục-assets)
2. [Bước 2: Tạo Player & Thiết lập vật lý](#bước-2-tạo-player--thiết-lập-vật-lý)
3. [Bước 3: Tạo mặt đất thử nghiệm (Ground)](#bước-3-tạo-mặt-đất-thử-nghiệm-ground)
4. [Bước 4: Lập trình PlayerController.cs](#bước-4-lập-trình-playercontrollercs)
5. [Bước 5: Hệ thống Animation cho Player](#bước-5-hệ-thống-animation-cho-player)
6. [Bước 6: Checklist kiểm tra hoàn thành](#bước-6-checklist-kiểm-tra-hoàn-thành)
7. [Các lỗi thường gặp và cách khắc phục](#các-lỗi-thường-gặp-và-cách-khắc-phục)

---

## BƯỚC 1: KHỞI TẠO DỰ ÁN & CẤU TRÚC THƯ MỤC ASSETS

### 1.1. Tạo dự án mới
1. Mở **Unity Hub**.
2. Nhấn nút **New Project** (góc trên bên phải).
3. Chọn template: **2D (Core)**.
4. Đặt tên dự án tại ô **Project Name**: `Lab1_Platformer`.
5. Chọn thư mục lưu trữ và nhấn **Create Project**.

### 1.2. Sắp xếp cây thư mục trong `Assets`
Tại cửa sổ **Project** (nằm ở phía dưới giao diện Unity), chuột phải vào khoảng trống trong thư mục `Assets` ➜ chọn **Create** ➜ **Folder** để tạo lần lượt các thư mục sau:
```text
Assets/
├── _Scripts/            # Chứa các mã nguồn C#
├── Sprites/             # Chứa hình ảnh nhân vật, gạch, địa hình
├── Animations/          # Chứa các Animation Clips và Animator Controller
└── PhysicsMaterials/    # Chứa vật liệu vật lý 2D
```

---

## BƯỚC 2: TẠO PLAYER & THIẾT LẬP VẬT LÝ

### 2.1. Tạo GameObject Player
1. Trong cửa sổ **Hierarchy** (bên trái màn hình), chuột phải ➜ **2D Object** ➜ **Sprites** ➜ **Capsule** (hoặc kéo ảnh sprite nhân vật của bạn vào Hierarchy).
2. Chuột phải vào đối tượng vừa tạo ➜ chọn **Rename** ➜ đổi tên thành: `Player`.
3. Nhìn sang cửa sổ **Inspector** (bên phải), tại component **Transform**:
   - Bấm vào dấu 3 chấm góc phải của Transform ➜ chọn **Reset** để đưa tọa độ về `Position (0, 0, 0)`.

### 2.2. Thiết lập Sorting Layer
*Sorting Layer giúp quyết định thứ tự hiển thị của các đối tượng đồ họa (đối tượng nào nằm trước, đối tượng nào nằm sau).*
1. Chọn `Player` trong Hierarchy.
2. Tại component **Sprite Renderer** trên Inspector, tìm dòng **Sorting Layer**.
3. Bấm vào menu thả xuống (đang hiển thị `Default`) ➜ chọn **Add Sorting Layer...**
4. Bấm vào dấu `+` và gõ tên layer mới: `Player`.
5. Chọn lại đối tượng `Player` trong Hierarchy ➜ tại mục **Sorting Layer** chọn layer `Player` vừa tạo.

### 2.3. Thêm các Component Vật lý
1. Chọn `Player` ➜ nhấn nút **Add Component** ở dưới cùng bảng Inspector.
2. Tìm và chọn **Rigidbody 2D**:
   - **Collision Detection**: Chọn `Continuous` (ngăn ngừa lỗi nhân vật rơi xuyên sàn khi tốc độ cao).
   - Mở mục **Constraints**: Tích chọn ô **Freeze Rotation Z** (bắt buộc tích ô này để nhân vật luôn đứng thẳng, không bị xoay tròn hay lật ngửa khi va chạm).
3. Nhấn tiếp nút **Add Component** ➜ tìm và chọn **Box Collider 2D**:
   - Nhấn vào nút **Edit Collider** (biểu tượng hình vuông có 4 điểm nút).
   - Dùng chuột kéo các cạnh màu xanh lá cây trên màn hình Scene sao cho vừa khít với cơ thể của Player.

### 2.4. Xử lý va chạm tường (Physics Material 2D Friction = 0)
*Hiện tượng:* Trong game 2D, khi nhân vật nhảy áp sát vào tường thẳng đứng và người chơi vẫn nhấn giữ phím di chuyển, lực ma sát mặc định sẽ làm nhân vật bị "dính chặt" vào tường không rơi xuống được.
*Cách khắc phục:* Gán vật liệu vật lý có độ ma sát bằng 0.

1. Trong tab **Project**, mở thư mục `Assets/PhysicsMaterials`.
2. Chuột phải ➜ **Create** ➜ **2D** ➜ **Physics Material 2D**.
3. Đặt tên file là: `ZeroFriction`.
4. Nhấp chọn file `ZeroFriction`, nhìn sang bảng Inspector:
   - **Friction**: Điền số `0`.
   - **Bounciness**: Điền số `0`.
5. Chọn lại đối tượng `Player` trong Hierarchy.
6. Kéo file `ZeroFriction` từ Project thả vào ô **Material** của component **Box Collider 2D**.

---

## BƯỚC 3: TẠO MẶT ĐẤT THỬ NGHIỆM (GROUND)

Để có sàn cho nhân vật đứng và kiểm tra cơ chế rơi/nhảy:
1. Chuột phải trong **Hierarchy** ➜ **2D Object** ➜ **Sprites** ➜ **Square**.
2. Đổi tên đối tượng thành: `Ground`.
3. Chỉnh thông số tại component **Transform**:
   - `Position`: `X = 0, Y = -3, Z = 0`
   - `Scale`: `X = 16, Y = 1, Z = 1` (kéo dài thành một thanh sàn).
4. Nhấn **Add Component** ➜ chọn **Box Collider 2D**.
5. **Cài đặt Layer cho mặt đất**:
   - Nhìn lên góc trên bên phải Inspector của `Ground`, tìm dòng **Layer: Default**.
   - Bấm vào danh sách ➜ chọn **Add Layer...**
   - Tại dòng **User Layer 8**, gõ tên: `Ground`.
   - Chọn lại đối tượng `Ground` trong Hierarchy ➜ chuyển **Layer** của nó thành `Ground`.

---

## BƯỚC 4: LẬP TRÌNH PLAYERCONTROLLER.CS

### 4.1. Tạo điểm kiểm tra tiếp đất (`GroundCheck`)
1. Chuột phải vào `Player` trong Hierarchy ➜ chọn **Create Empty** (tạo đối tượng con nằm bên trong Player).
2. Đổi tên đối tượng con này thành: `GroundCheck`.
3. Dùng công cụ Move Tool (phím `W`) kéo điểm `GroundCheck` xuống **ngay sát mép dưới bàn chân** của Player.

### 4.2. Viết mã nguồn C#
1. Vào thư mục `Assets/_Scripts`.
2. Chuột phải ➜ **Create** ➜ **C# Script** ➜ Đặt tên chính xác là: `PlayerController`.
3. Mở file và dán toàn bộ đoạn mã sau:

```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("--- THIẾT LẬP DI CHUYỂN ---")]
    [Tooltip("Tốc độ di chuyển ngang của nhân vật")]
    public float moveSpeed = 8f;
    
    [Tooltip("Lực nhảy của nhân vật")]
    public float jumpForce = 12f;

    [Header("--- KIỂM TRA MẶT ĐẤT ---")]
    [Tooltip("Điểm kiểm tra đặt dưới chân nhân vật")]
    public Transform groundCheck;
    
    [Tooltip("Bán kính vòng tròn quét va chạm")]
    public float checkRadius = 0.2f;
    
    [Tooltip("Layer đại diện cho mặt đất")]
    public LayerMask groundLayer;

    // Các biến nội bộ
    private Rigidbody2D rb;
    private Animator anim;
    private float horizontalInput;
    private bool isGrounded;
    private bool isFacingRight = true;

    void Start()
    {
        // Lấy tham chiếu đến các component trên cùng GameObject
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Nhận tín hiệu phím di chuyển ngang (-1: Trái, 0: Đứng yên, 1: Phải)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 2. Sử dụng Physics2D.OverlapCircle để quét xem chân có chạm đất không
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // 3. Cơ chế nhảy: Chỉ nhảy khi bấm phím Jump (phím Space) VÀ đang tiếp đất
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // 4. Tự động quay mặt theo hướng di chuyển
        Flip();

        // 5. Cập nhật các thông số cho Animator Controller
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        // Áp dụng vận tốc vật lý trong FixedUpdate để đảm bảo mượt mà
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }

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

    // Vẽ vòng tròn GroundCheck trong Scene để dễ dàng căn chỉnh bằng mắt
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}
```

### 4.3. Gán Script và cấu hình thông số trên Inspector
1. Kéo script `PlayerController` thả vào đối tượng `Player` trong Hierarchy.
2. Nhìn sang bảng Inspector của `Player`:
   - Kéo GameObject con `GroundCheck` thả vào ô **Ground Check**.
   - Tại ô **Ground Layer**: Bấm vào và tích chọn layer **Ground**.
   - **Check Radius**: Giữ nguyên `0.2`.
   - **Move Speed**: `8`.
   - **Jump Force**: `12`.

---

## BƯỚC 5: HỆ THỐNG ANIMATION CHO PLAYER

### 5.1. Tạo Animation Clips
1. Chọn đối tượng `Player` trong Hierarchy.
2. Mở cửa sổ Animation: Menu **Window** ➜ **Animation** ➜ **Animation** (hoặc nhấn `Ctrl + 6`).
3. Nhấn nút **Create** xuất hiện giữa cửa sổ Animation:
   - Lưu vào thư mục `Assets/Animations/`.
   - Đặt tên clip: `Player_Idle.anim`.
4. Bấm vào danh sách clip vừa tạo (chữ `Player_Idle`) ➜ chọn **Create New Clip...**:
   - Đặt tên: `Player_Run.anim`.
5. Tiếp tục bấm **Create New Clip...**:
   - Đặt tên: `Player_Jump.anim`.
6. **Thêm sprite vào từng Clip**:
   - Chọn clip `Player_Idle` ➜ Kéo các frame ảnh đứng yên thả vào timeline.
   - Chọn clip `Player_Run` ➜ Kéo các frame ảnh chạy bộ thả vào timeline.
   - Chọn clip `Player_Jump` ➜ Kéo frame ảnh lúc đang nhảy/trên không thả vào timeline.

### 5.2. Thiết lập Animator Controller
1. Vào menu **Window** ➜ **Animation** ➜ **Animator** để mở giao diện State Machine.
2. **Tạo Parameters**:
   - Chọn tab **Parameters** (cột bên trái).
   - Bấm dấu `+` ➜ chọn kiểu dữ liệu **Bool** ➜ đặt tên: `isRunning`.
   - Bấm dấu `+` ➜ chọn kiểu dữ liệu **Bool** ➜ đặt tên: `isJumping`.

3. **Cấu hình các Transitions (Đường chuyển đổi)**:

| Từ State | Tới State | Has Exit Time | Transition Duration | Điều kiện (Conditions) |
| :--- | :--- | :---: | :---: | :--- |
| `Player_Idle` | `Player_Run` | ❌ Bỏ tích | `0` | `isRunning` = `true` |
| `Player_Run` | `Player_Idle` | ❌ Bỏ tích | `0` | `isRunning` = `false` |
| `Any State` | `Player_Jump` | ❌ Bỏ tích | `0` | `isJumping` = `true` |
| `Player_Jump` | `Player_Idle` | ❌ Bỏ tích | `0` | `isJumping` = `false` |

*Cách tạo từng Transition:*
- Chuột phải vào State xuất phát ➜ **Make Transition** ➜ kéo mũi tên nối vào State đích.
- Nhấp chuột trái vào chính mũi tên vừa tạo, nhìn sang Inspector bên phải:
  - Bỏ tích ô **Has Exit Time**.
  - Bấm mở rộng **Settings** ➜ sửa **Transition Duration (s)** thành `0`.
  - Tại bảng **Conditions**, bấm dấu `+` và chọn đúng tham số theo bảng trên.

---

## BƯỚC 6: CHECKLIST KIỂM TRA HOÀN THÀNH

Nhấn nút **Play** (biểu tượng tam giác ở giữa cạnh trên cửa sổ Unity) và kiểm tra các tiêu chí:

- [ ] **Di chuyển**: Bấm phím `A`/`D` hoặc mũi tên trái/phải, Player di chuyển mượt mà.
- [ ] **Lật mặt**: Khi đi sang trái, nhân vật quay mặt sang trái; khi đi sang phải, nhân vật quay mặt sang phải.
- [ ] **Nhảy**: Bấm phím `Space`, nhân vật nhảy lên một lực vừa phải.
- [ ] **Chống nhảy trên không (Double Jump Bug)**: Nhấn liên tục phím `Space` khi đang rơi trên không trung, nhân vật **không** được phép nhảy tiếp.
- [ ] **Không dính tường**: Tạo một bức tường thẳng đứng bằng Square có Box Collider, cho Player áp sát vào tường và bấm phím di chuyển, Player vẫn rơi xuống đất bình thường chứ không bị kẹt lơ lửng trên tường.
- [ ] **Animation**:
  - Đứng yên: Chạy animation `Idle`.
  - Chạy: Tự động đổi sang `Run`.
  - Nhảy lên: Đổi sang `Jump`. Khi chạm đất lập tức trở về `Idle` hoặc `Run`.

---

## CÁC LỖI THƯỜNG GẶP VÀ CÁCH KHẮC PHỤC

1. **Nhân vật không nhảy được khi bấm Space:**
   - *Nguyên nhân:* Điểm `GroundCheck` chưa chạm tới Layer `Ground` hoặc chưa gán `Ground Layer` trong script.
   - *Khắc phục:* Chọn `Player`, nhìn vào Inspector, kiểm tra ô **Ground Layer** đã chọn lớp `Ground` chưa. Kéo điểm `GroundCheck` xuống thấp hơn một chút.
2. **Nhân vật bị ngã lăn tròn khi va chạm:**
   - *Nguyên nhân:* Chưa khóa trục quay Z.
   - *Khắc phục:* Chọn `Player` ➜ component **Rigidbody 2D** ➜ mở mục **Constraints** ➜ tích chọn **Freeze Rotation Z**.
3. **Animation chuyển đổi bị chậm hoặc trễ:**
   - *Nguyên nhân:* Quên chưa tắt `Has Exit Time` trên các đường nối Transition trong Animator.
   - *Khắc phục:* Nhấp vào từng mũi tên trong cửa sổ Animator ➜ bỏ chọn **Has Exit Time** và đưa **Transition Duration (s)** về `0`.
4. **Nhân vật bị biến mất hoặc co rúm khi quay mặt:**
   - *Nguyên nhân:* Trong hàm `Flip()`, giá trị trục X bị gán sai tỷ lệ ban đầu.
   - *Khắc phục:* Đảm bảo sử dụng hàm `Mathf.Abs(scaler.x)` như code mẫu để giữ nguyên kích thước gốc.
