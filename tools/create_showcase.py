import os
from PIL import Image, ImageDraw, ImageFont

def make_showcase():
    sprites = [
        ('Player', 'Adventurer Knight', 'My project (5)/Assets/Sprites/_backup/Player.png', 'My project (5)/Assets/Sprites/Player.png'),
        ('Enemy', 'Shadow Void Slime', 'My project (5)/Assets/Sprites/_backup/Enemy.png', 'My project (5)/Assets/Sprites/Enemy.png'),
        ('Ground', 'Ancient Flagstone', 'My project (5)/Assets/Sprites/_backup/Ground.png', 'My project (5)/Assets/Sprites/Ground.png'),
        ('Wall', 'Castle Dungeon Brick', 'My project (5)/Assets/Sprites/_backup/Wall.png', 'My project (5)/Assets/Sprites/Wall.png'),
        ('Coin', 'Royal Gold Medallion', 'My project (5)/Assets/Sprites/_backup/Coin.png', 'My project (5)/Assets/Sprites/Coin.png'),
        ('Key', 'Ornate Dungeon Key', 'My project (5)/Assets/Sprites/_backup/Key.png', 'My project (5)/Assets/Sprites/Key.png'),
        ('Trap', 'Forged Iron Spikes', 'My project (5)/Assets/Sprites/_backup/Trap.png', 'My project (5)/Assets/Sprites/Trap.png')
    ]

    card_w = 420
    card_h = 170
    gap = 20
    margin = 30
    cols = 2
    rows = (len(sprites) + cols - 1) // cols
    
    total_w = margin * 2 + cols * card_w + (cols - 1) * gap
    header_h = 100
    total_h = margin * 2 + header_h + rows * card_h + (rows - 1) * gap

    # Dark fantasy UI canvas
    canvas = Image.new('RGBA', (total_w, total_h), (18, 20, 28, 255))
    draw = ImageDraw.Draw(canvas)

    # Header
    draw.rectangle([0, 0, total_w, header_h], fill=(26, 30, 44, 255))
    draw.line([(0, header_h), (total_w, header_h)], fill=(64, 76, 105, 255), width=2)
    
    # Try basic font or default
    try:
        title_font = ImageFont.truetype("arial.ttf", 26)
        sub_font = ImageFont.truetype("arial.ttf", 15)
        label_font = ImageFont.truetype("arialbd.ttf", 14)
        tag_font = ImageFont.truetype("arial.ttf", 12)
    except:
        title_font = sub_font = label_font = tag_font = ImageFont.load_default()

    draw.text((margin, 24), "2D PLATFORMER - FANTASY SPRITES UPGRADE", fill=(245, 205, 75, 255), font=title_font)
    draw.text((margin, 58), "Before & After comparison of the 7 core game sprites (Scaled 3x - 4x for detail)", fill=(160, 175, 205, 255), font=sub_font)

    for idx, (name, subtitle, old_path, new_path) in enumerate(sprites):
        c = idx % cols
        r = idx // cols
        x0 = margin + c * (card_w + gap)
        y0 = margin + header_h + r * (card_h + gap)
        x1 = x0 + card_w
        y1 = y0 + card_h

        # Card container
        draw.rounded_rectangle([x0, y0, x1, y1], radius=8, fill=(28, 33, 48, 255), outline=(50, 60, 85, 255), width=1)
        
        # Card title
        draw.text((x0 + 16, y0 + 12), name.upper(), fill=(255, 255, 255, 255), font=label_font)
        draw.text((x0 + 80, y0 + 13), f"• {subtitle}", fill=(140, 160, 190, 255), font=tag_font)

        # Draw Old Sprite Slot
        slot_size = 96
        slot1_x = x0 + 20
        slot1_y = y0 + 40
        draw.rounded_rectangle([slot1_x, slot1_y, slot1_x + slot_size, slot1_y + slot_size], radius=6, fill=(15, 17, 24, 255), outline=(40, 48, 68, 255))
        draw.text((slot1_x + 8, slot1_y + slot_size - 18), "BEFORE", fill=(180, 80, 80, 255), font=tag_font)
        
        # Old sprite render
        if os.path.exists(old_path):
            old_im = Image.open(old_path).convert('RGBA')
            scale = min(slot_size // old_im.width, slot_size // old_im.height)
            scale = max(1, min(scale, 3))
            old_scaled = old_im.resize((old_im.width * scale, old_im.height * scale), Image.NEAREST)
            ox = slot1_x + (slot_size - old_scaled.width) // 2
            oy = slot1_y + (slot_size - 16 - old_scaled.height) // 2
            canvas.alpha_composite(old_scaled, (ox, oy))

        # Arrow
        arrow_x = slot1_x + slot_size + 15
        arrow_y = slot1_y + slot_size // 2 - 8
        draw.text((arrow_x, arrow_y), "➔", fill=(90, 130, 190, 255), font=title_font)

        # Draw New Sprite Slot
        slot2_x = arrow_x + 35
        slot2_y = slot1_y
        draw.rounded_rectangle([slot2_x, slot2_y, slot2_x + slot_size, slot2_y + slot_size], radius=6, fill=(20, 26, 38, 255), outline=(80, 140, 210, 255), width=2)
        draw.text((slot2_x + 8, slot2_y + slot_size - 18), "AFTER (NEW)", fill=(80, 200, 120, 255), font=tag_font)

        # New sprite render
        if os.path.exists(new_path):
            new_im = Image.open(new_path).convert('RGBA')
            scale = min(slot_size // new_im.width, slot_size // new_im.height)
            scale = max(1, min(scale, 3))
            new_scaled = new_im.resize((new_im.width * scale, new_im.height * scale), Image.NEAREST)
            nx = slot2_x + (slot_size - new_scaled.width) // 2
            ny = slot2_y + (slot_size - 16 - new_scaled.height) // 2
            canvas.alpha_composite(new_scaled, (nx, ny))

        # Feature badges on right
        desc_x = slot2_x + slot_size + 15
        desc_y = slot1_y + 10
        bullets = {
            'Player': ["Hiệp sĩ giáp bạc", "Khăn choàng đỏ", "Mũ giáp & mắt sáng", "Khớp chuẩn Collider"],
            'Enemy': ["Slime bóng tối gai", "Mắt ác quỷ phát sáng", "Răng nanh nhọn", "Độ bóng 3D chân thực"],
            'Ground': ["Đá tảng cổ rêu phong", "Tiled ngang mượt", "Gờ sáng viền đi", "Cấu trúc thành lũy"],
            'Wall': ["Gạch tường ngục tối", "Tiled ngang & dọc", "Vân đá nứt & rêu", "Khối 3D đổ bóng"],
            'Coin': ["Đồng vàng hoàng gia", "Viền vát 3D sáng lóa", "Ngôi sao 4 cánh", "Kim loại ấm áp"],
            'Key': ["Chìa khóa cổ hoàng gia", "Đính ngọc Ruby đỏ", "Rãnh mở kho báu", "Chi tiết tinh xảo"],
            'Trap': ["Bẫy chông sắt rèn", "3 mũi nhọn gai góc", "Đinh tán lục giác", "Đầu chông sắc bén"]
        }
        for b_idx, b_text in enumerate(bullets.get(name, [])):
            draw.text((desc_x, desc_y + b_idx * 18), f"• {b_text}", fill=(200, 215, 235, 255), font=tag_font)

    # Save outputs
    canvas.save('tools/output/Sprites_Showcase.png')
    
    artifact_dir = r'C:\Users\RinHeo\.gemini\antigravity-ide\brain\2c765379-a03c-4ecb-af15-235679110bd7'
    if os.path.exists(artifact_dir):
        canvas.save(os.path.join(artifact_dir, 'Sprites_Showcase.png'))
    print("Showcase image created successfully!")

if __name__ == '__main__':
    make_showcase()
