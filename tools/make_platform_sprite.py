import os
from PIL import Image, ImageDraw

def hex_to_rgba(h, a=255):
    h = h.lstrip('#')
    return tuple(int(h[i:i+2], 16) for i in (0, 2, 4)) + (a,)

def create_moving_platform():
    # 96 x 32 pixel platform
    w, h = 96, 32
    img = Image.new('RGBA', (w, h), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # Palette
    metal_dark = hex_to_rgba('#1e2430')
    metal_mid = hex_to_rgba('#374256')
    metal_light = hex_to_rgba('#5a6b8a')
    metal_high = hex_to_rgba('#8ea5cb')
    
    top_surface = hex_to_rgba('#2b3648')
    top_grid = hex_to_rgba('#43536f')
    
    glow_cyan = hex_to_rgba('#00f0ff')
    glow_cyan_bright = hex_to_rgba('#a8fcff')
    glow_cyan_deep = hex_to_rgba('#0088aa')
    
    gold_trim = hex_to_rgba('#e5a93b')
    gold_dark = hex_to_rgba('#8c5e12')
    
    # Main platform body: rounded polygon
    # y: 4 to 26
    # Outer dark outline
    draw.rounded_rectangle([2, 4, w - 3, 26], radius=4, fill=metal_dark, outline=hex_to_rgba('#10141d'))
    # Inner metallic fill
    draw.rounded_rectangle([4, 6, w - 5, 24], radius=3, fill=metal_mid)
    
    # Top walking plate (y: 6 to 12)
    draw.rectangle([6, 6, w - 7, 12], fill=top_surface)
    # Top edge highlight
    draw.line([(6, 6), (w - 7, 6)], fill=metal_high)
    
    # Non-slip tread grooves on top plate
    for x in range(12, w - 10, 8):
        draw.line([(x, 8), (x + 3, 11)], fill=top_grid)
        draw.line([(x + 1, 8), (x + 4, 11)], fill=metal_high)
        
    # Gold corner brackets
    draw.rectangle([3, 5, 7, 13], fill=gold_trim)
    draw.rectangle([w - 8, 5, w - 4, 13], fill=gold_trim)
    draw.line([(3, 5), (7, 5)], fill=gold_dark)
    draw.line([(w - 8, 5), (w - 4, 5)], fill=gold_dark)
    
    # Lower glowing energy strip (y: 16 to 19)
    draw.rounded_rectangle([16, 16, w - 17, 20], radius=2, fill=glow_cyan_deep)
    draw.line([(18, 18), (w - 19, 18)], fill=glow_cyan)
    draw.line([(24, 18), (w - 25, 18)], fill=glow_cyan_bright)
    
    # Small side thrusters / anti-grav nodes at bottom corners
    # Left thruster
    draw.polygon([(10, 26), (18, 26), (15, 30), (13, 30)], fill=metal_mid, outline=metal_dark)
    draw.line([(13, 29), (15, 29)], fill=glow_cyan)
    # Right thruster
    draw.polygon([(w - 19, 26), (w - 11, 26), (w - 14, 30), (w - 16, 30)], fill=metal_mid, outline=metal_dark)
    draw.line([(w - 16, 29), (w - 14, 29)], fill=glow_cyan)
    
    # Rivet bolts
    for rx in [10, 22, w - 23, w - 11]:
        draw.rectangle([rx, 14, rx + 1, 15], fill=metal_high)
        draw.point((rx + 1, 15), fill=metal_dark)

    return img

if __name__ == '__main__':
    sprites_dir = os.path.join(os.path.dirname(__file__), '..', 'My project (5)', 'Assets', 'Sprites')
    os.makedirs(sprites_dir, exist_ok=True)
    img = create_moving_platform()
    img.save(os.path.join(sprites_dir, 'MovingPlatform.png'))
    print("Generated MovingPlatform.png successfully!")
