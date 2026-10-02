import os
from PIL import Image, ImageDraw

def hex_to_rgba(h, a=255):
    h = h.lstrip('#')
    return tuple(int(h[i:i+2], 16) for i in (0, 2, 4)) + (a,)

def create_player():
    img = Image.new('RGBA', (64, 64), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # Knight Character inside x in [13..51], y in [9..63] (PIL coords)
    # Width ~38, Height ~54
    # Center X ~ 32
    
    # 1. Flowing Crimson Cape (Behind character)
    cape_shadow = hex_to_rgba('#500b18')
    cape_dark = hex_to_rgba('#7d1326')
    cape_mid = hex_to_rgba('#aa1c34')
    cape_bright = hex_to_rgba('#d92b47')
    cape_high = hex_to_rgba('#f0526b')
    
    # Cape flows from back left and right
    for y in range(25, 56):
        # Left billow of cape
        w_left = int(2 + (y - 25) * 0.28)
        x_start = 22 - w_left
        for x in range(x_start, 26):
            if x < 15: continue
            col = cape_shadow if x < x_start + 2 else (cape_dark if x < x_start + 4 else cape_mid)
            img.putpixel((x, y), col)
        # Right billow of cape (fluttering back)
        w_right = int(3 + (y - 25) * 0.35)
        x_end = 39 + w_right
        for x in range(37, min(50, x_end)):
            col = cape_bright if (x - 37) % 3 == 0 else cape_mid
            if x >= x_end - 2: col = cape_dark
            img.putpixel((x, y), col)
    
    # 2. Scarf folds fluttering
    scarf_pts = [
        (42, 22), (45, 23), (48, 25), (51, 28), (50, 31), (47, 30), (43, 27)
    ]
    draw.polygon(scarf_pts, fill=cape_bright, outline=cape_shadow)
    
    # 3. Steel Armor & Helmet Palette
    sh_out = hex_to_rgba('#12151c')
    st_deep = hex_to_rgba('#222838')
    st_dark = hex_to_rgba('#374158')
    st_mid = hex_to_rgba('#586885')
    st_light = hex_to_rgba('#879bba')
    st_high = hex_to_rgba('#c6d6eb')
    st_white = hex_to_rgba('#f0f6ff')
    
    # Gold Accents
    gold_dark = hex_to_rgba('#78470a')
    gold_mid = hex_to_rgba('#c48216')
    gold_bright = hex_to_rgba('#f7be2a')
    gold_high = hex_to_rgba('#fff07c')
    
    # Leather & Belt
    leath_dark = hex_to_rgba('#361e10')
    leath_mid = hex_to_rgba('#5a331c')
    leath_light = hex_to_rgba('#8c5332')
    
    # Glow eye / visor
    eye_glow = hex_to_rgba('#4af0df')
    eye_core = hex_to_rgba('#e0ffff')

    # Plume / Crest on Helmet (Red / Gold) - fits y >= 9
    for px, py in [(31, 10), (32, 9), (33, 9), (34, 10), (35, 11), (36, 12), (37, 13)]:
        img.putpixel((px, py), cape_high)
        img.putpixel((px, py+1), cape_bright)
        img.putpixel((px, py+2), cape_mid)
    img.putpixel((32, 9), cape_bright)
    img.putpixel((33, 9), cape_high)

    # HELMET (y: 11 to 24, x: 23 to 41)
    # Outline
    draw.rounded_rectangle([23, 11, 41, 24], radius=5, fill=st_dark, outline=sh_out)
    # Helmet shading dome
    for y in range(12, 24):
        for x in range(24, 41):
            if x <= 26:
                img.putpixel((x, y), st_deep)
            elif x <= 30:
                img.putpixel((x, y), st_dark)
            elif x <= 35:
                img.putpixel((x, y), st_mid)
            elif x <= 38:
                img.putpixel((x, y), st_light)
            else:
                img.putpixel((x, y), st_high)
    # Highlight gleam on helmet
    draw.line([(35, 12), (37, 14)], fill=st_white, width=1)
    img.putpixel((36, 13), st_white)
    
    # Helmet Visor (T-slit or horizontal knight slit)
    # Visor slit plate (y: 17 to 20, x: 26 to 40)
    for x in range(26, 40):
        img.putpixel((x, 17), gold_mid)
        img.putpixel((x, 18), sh_out)
        img.putpixel((x, 19), sh_out)
        img.putpixel((x, 20), gold_dark)
    # Glowing visor eye slit
    for x in range(29, 39):
        img.putpixel((x, 18), eye_glow)
        if 31 <= x <= 36:
            img.putpixel((x, 18), eye_core)

    # Helmet Cheekguards & Chin
    draw.polygon([(26, 21), (28, 25), (36, 25), (38, 21)], fill=st_dark, outline=sh_out)
    draw.line([(30, 22), (34, 22)], fill=st_light)

    # NECK / SCARF (y: 24 to 27)
    for y in range(24, 28):
        for x in range(25, 40):
            img.putpixel((x, y), cape_bright if y == 25 and 28 <= x <= 36 else cape_mid)
    draw.line([(25, 27), (39, 27)], fill=cape_dark)

    # CHESTPLATE / BREASTPLATE (y: 28 to 39, x: 23 to 42)
    draw.rounded_rectangle([23, 28, 41, 39], radius=3, fill=st_mid, outline=sh_out)
    # Chest shading
    for y in range(28, 39):
        for x in range(24, 41):
            if x <= 26: img.putpixel((x, y), st_deep)
            elif x <= 30: img.putpixel((x, y), st_dark)
            elif x <= 36: img.putpixel((x, y), st_mid)
            elif x <= 39: img.putpixel((x, y), st_light)
            else: img.putpixel((x, y), st_high)
    # Golden Royal Crest on chest
    crest_gold = [(32, 30), (31, 31), (32, 31), (33, 31), (32, 32), (30, 33), (32, 33), (34, 33), (32, 34), (32, 35)]
    for gx, gy in crest_gold:
        img.putpixel((gx, gy), gold_bright)
    img.putpixel((32, 31), gold_high)
    img.putpixel((32, 33), gold_high)
    # Shoulder Pauldrons
    # Left Pauldron
    draw.rounded_rectangle([20, 27, 26, 34], radius=2, fill=st_mid, outline=sh_out)
    img.putpixel((21, 28), st_light)
    img.putpixel((22, 28), st_high)
    # Right Pauldron
    draw.rounded_rectangle([39, 27, 45, 34], radius=2, fill=st_light, outline=sh_out)
    draw.line([(40, 28), (43, 28)], fill=st_white)

    # BELT & POUCH (y: 40 to 43)
    draw.rectangle([23, 40, 41, 43], fill=leath_mid, outline=sh_out)
    for x in range(24, 41):
        img.putpixel((x, 40), leath_light)
        img.putpixel((x, 43), leath_dark)
    # Gold Belt Buckle
    draw.rectangle([30, 39, 34, 43], fill=gold_bright, outline=gold_dark)
    img.putpixel((32, 41), gold_high)
    # Leather Pouch (left hip)
    draw.rectangle([23, 41, 27, 45], fill=leath_dark, outline=sh_out)
    img.putpixel((25, 42), gold_bright)
    # Dagger / Sword hilt (right hip)
    draw.line([(42, 38), (45, 35)], fill=st_white, width=1) # blade pommel
    draw.line([(40, 37), (43, 40)], fill=gold_bright, width=1) # guard
    draw.line([(42, 39), (45, 42)], fill=leath_dark, width=1) # hilt

    # GAUNTLETS (Hands)
    # Left hand resting
    draw.rounded_rectangle([19, 35, 24, 41], radius=2, fill=st_dark, outline=sh_out)
    img.putpixel((21, 36), st_mid)
    # Right hand
    draw.rounded_rectangle([41, 35, 46, 41], radius=2, fill=st_mid, outline=sh_out)
    img.putpixel((43, 36), st_light)

    # Tassets / Armored Faulds (y: 44 to 47)
    for y in range(44, 48):
        for x in range(24, 41):
            if x <= 31:
                img.putpixel((x, y), st_dark if x < 28 else st_mid)
            else:
                img.putpixel((x, y), st_light if x > 36 else st_mid)
    draw.line([(32, 44), (32, 47)], fill=sh_out) # center slit

    # LEGS & GREAVES (y: 48 to 61)
    # Left Leg (x: 23 to 31)
    draw.rectangle([23, 48, 30, 56], fill=st_dark, outline=sh_out)
    # Right Leg (x: 34 to 42)
    draw.rectangle([34, 48, 41, 56], fill=st_mid, outline=sh_out)
    # Knee cops
    draw.rectangle([23, 48, 29, 50], fill=st_mid, outline=st_deep)
    draw.rectangle([34, 48, 40, 50], fill=st_light, outline=st_dark)

    # BOOTS (y: 56 to 62)
    # Left boot
    draw.rounded_rectangle([21, 56, 30, 62], radius=2, fill=leath_dark, outline=sh_out)
    draw.line([(22, 62), (30, 62)], fill=st_deep) # sole
    img.putpixel((24, 57), leath_light)
    # Right boot
    draw.rounded_rectangle([34, 56, 43, 62], radius=2, fill=leath_mid, outline=sh_out)
    draw.line([(34, 62), (42, 62)], fill=st_dark) # sole
    img.putpixel((37, 57), leath_light)
    img.putpixel((38, 57), leath_light)

    return img

def create_enemy():
    img = Image.new('RGBA', (48, 48), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Void/Shadow Spiked Slime inside x in [5..43], y in [4..44] (PIL coords)
    # Palette
    out_col = hex_to_rgba('#0d0817')
    deep_col = hex_to_rgba('#220b38')
    body_dark = hex_to_rgba('#48136e')
    body_mid = hex_to_rgba('#7b21a8')
    body_light = hex_to_rgba('#aa3cd9')
    body_high = hex_to_rgba('#d972ff')
    body_shine = hex_to_rgba('#f8d9ff')

    eye_out = hex_to_rgba('#400000')
    eye_red = hex_to_rgba('#e61919')
    eye_orange = hex_to_rgba('#ff8c00')
    eye_pupil = hex_to_rgba('#ffffaa')
    
    horn_out = hex_to_rgba('#120a1c')
    horn_base = hex_to_rgba('#3c1b52')
    horn_high = hex_to_rgba('#934bc7')

    # Spikes/Horns at top
    # Left horn
    horn_left = [(13, 17), (11, 10), (8, 4), (12, 7), (16, 12), (17, 16)]
    draw.polygon(horn_left, fill=horn_base, outline=horn_out)
    draw.line([(10, 7), (13, 13)], fill=horn_high)

    # Right horn
    horn_right = [(35, 17), (37, 10), (40, 4), (36, 7), (32, 12), (31, 16)]
    draw.polygon(horn_right, fill=horn_base, outline=horn_out)
    draw.line([(38, 7), (35, 13)], fill=horn_high)

    # Center mini crest
    draw.polygon([(24, 7), (21, 14), (27, 14)], fill=horn_high, outline=horn_out)

    # Slime Main Jelly Body (ellipse / blob shape from y: 14 to 43, x: 7 to 41)
    draw.ellipse([7, 14, 41, 43], fill=body_mid, outline=out_col)
    # Flatten base
    draw.ellipse([8, 33, 40, 43], fill=deep_col, outline=out_col)

    # Inner Shading / Jelly Volume
    for y in range(15, 43):
        for x in range(8, 41):
            if img.getpixel((x, y))[3] > 0:
                dist_y = y - 14
                dist_x = abs(x - 24)
                if y > 36:
                    img.putpixel((x, y), deep_col)
                elif y > 28:
                    img.putpixel((x, y), body_dark if dist_x > 11 else body_mid)
                elif y > 20:
                    if dist_x < 9 and dist_y < 12:
                        img.putpixel((x, y), body_light)
                    else:
                        img.putpixel((x, y), body_mid if dist_x < 14 else body_dark)
                else:
                    img.putpixel((x, y), body_light if dist_x < 10 else body_mid)

    # Jelly Translucent Specular Highlights (Top left curved shine)
    draw.arc([10, 16, 26, 26], start=180, end=300, fill=body_shine, width=2)
    img.putpixel((14, 18), body_shine)
    img.putpixel((15, 18), body_shine)
    img.putpixel((15, 19), body_high)
    img.putpixel((24, 17), body_shine)
    img.putpixel((25, 17), body_shine)

    # EVIL GLOWING EYES (Fierce diagonal angular menace)
    # Left eye
    left_eye_pts = [(14, 25), (21, 23), (20, 28), (14, 28)]
    draw.polygon(left_eye_pts, fill=eye_red, outline=eye_out)
    draw.line([(16, 25), (19, 24)], fill=eye_orange, width=1)
    img.putpixel((18, 25), eye_pupil)

    # Right eye
    right_eye_pts = [(34, 25), (27, 23), (28, 28), (34, 28)]
    draw.polygon(right_eye_pts, fill=eye_red, outline=eye_out)
    draw.line([(32, 25), (29, 24)], fill=eye_orange, width=1)
    img.putpixel((30, 25), eye_pupil)

    # Wicked Tooth Mouth / Smirk (y: 31 to 35, x: 20 to 28)
    mouth_pts = [(19, 31), (24, 35), (29, 31)]
    draw.line(mouth_pts, fill=out_col, width=2)
    # Sharp white monster fangs
    draw.polygon([(21, 31), (22, 34), (23, 31)], fill=hex_to_rgba('#ffffff'))
    draw.polygon([(25, 31), (26, 34), (27, 31)], fill=hex_to_rgba('#ffffff'))

    # Droplets / Slime Drips at bottom
    img.putpixel((12, 44), body_dark)
    img.putpixel((13, 44), deep_col)
    img.putpixel((35, 44), body_dark)
    img.putpixel((36, 44), deep_col)

    return img

def create_coin():
    img = Image.new('RGBA', (32, 32), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Rich 3D Gold Medallion, fits inside [3..28]
    # Center (15.5, 15.5), Radius 12
    out_col = hex_to_rgba('#422003')
    deep_gold = hex_to_rgba('#7d4002')
    bronze = hex_to_rgba('#b06509')
    mid_gold = hex_to_rgba('#e39a0e')
    bright_gold = hex_to_rgba('#f7c51e')
    high_gold = hex_to_rgba('#ffea63')
    white_shine = hex_to_rgba('#ffffff')

    # Base circle with border
    draw.ellipse([3, 3, 28, 28], fill=mid_gold, outline=out_col)

    # Beveled rim (Shading)
    for y in range(4, 28):
        for x in range(4, 28):
            dx = x - 15.5
            dy = y - 15.5
            d = (dx*dx + dy*dy)**0.5
            if d <= 12:
                # 3D spherical gradient & directional lighting from top-left (-1, -1)
                light = -(dx + dy) / 17.0
                if d > 9.5: # Outer Rim
                    if light > 0.4:
                        img.putpixel((x, y), high_gold if light < 0.8 else white_shine)
                    elif light > -0.2:
                        img.putpixel((x, y), bright_gold)
                    else:
                        img.putpixel((x, y), bronze if light > -0.6 else deep_gold)
                elif d > 8.0: # Recessed Inner Groove
                    img.putpixel((x, y), deep_gold if light < 0 else bronze)
                else: # Inner Medallion Plate
                    if light > 0.2:
                        img.putpixel((x, y), bright_gold)
                    elif light > -0.4:
                        img.putpixel((x, y), mid_gold)
                    else:
                        img.putpixel((x, y), bronze)

    # Radiant 4-pointed Star Emblem in center
    star_pts = [
        (15, 9), (16, 9), (17, 13), (21, 14), (22, 15), (22, 16), (21, 17), (17, 18),
        (16, 22), (15, 22), (14, 18), (10, 17), (9, 16), (9, 15), (10, 14), (14, 13)
    ]
    draw.polygon(star_pts, fill=high_gold, outline=deep_gold)
    draw.line([(15, 11), (15, 20)], fill=white_shine)
    draw.line([(11, 15), (20, 15)], fill=white_shine)
    img.putpixel((15, 15), white_shine)

    # Specular Corner Glint at top-left rim (7, 6)
    draw.line([(6, 6), (9, 6)], fill=white_shine)
    draw.line([(7, 5), (7, 8)], fill=white_shine)

    return img

def create_key():
    img = Image.new('RGBA', (32, 32), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Ornate Royal Dungeon Key in horizontal orientation (x: 4 to 27, y: 9 to 22)
    out_col = hex_to_rgba('#3b2203')
    deep_gold = hex_to_rgba('#784506')
    mid_gold = hex_to_rgba('#c9820e')
    bright_gold = hex_to_rgba('#f5b91b')
    high_gold = hex_to_rgba('#ffea69')
    white_shine = hex_to_rgba('#ffffff')
    gem_ruby = hex_to_rgba('#e6193c')
    gem_shine = hex_to_rgba('#ff94a6')

    # Bow (Handle on left, x: 4 to 12, y: 10 to 20)
    draw.ellipse([4, 10, 12, 20], fill=mid_gold, outline=out_col)
    draw.ellipse([6, 12, 10, 18], fill=deep_gold, outline=out_col)
    # Ruby inlaid at center of bow
    draw.ellipse([7, 13, 9, 17], fill=gem_ruby, outline=out_col)
    img.putpixel((8, 14), gem_shine)

    # Crown crest on bow top & bottom
    img.putpixel((8, 9), high_gold)
    img.putpixel((7, 9), out_col)
    img.putpixel((9, 9), out_col)

    img.putpixel((8, 21), high_gold)
    img.putpixel((7, 21), out_col)
    img.putpixel((9, 21), out_col)

    # Shaft / Stem (x: 12 to 27, y: 13 to 17)
    draw.rectangle([12, 13, 27, 17], fill=mid_gold, outline=out_col)
    draw.line([(12, 14), (27, 14)], fill=bright_gold)
    draw.line([(13, 14), (26, 14)], fill=high_gold)
    draw.line([(12, 16), (27, 16)], fill=deep_gold)

    # Collar ring on shaft
    draw.rectangle([14, 12, 16, 18], fill=bright_gold, outline=out_col)
    img.putpixel((15, 13), high_gold)

    # Key Bit / Ward Teeth (x: 22 to 27, y: 17 to 22)
    bit_pts = [
        (22, 17), (22, 22), (24, 22), (24, 19), (25, 19), (25, 22), (27, 22), (27, 17)
    ]
    draw.polygon(bit_pts, fill=mid_gold, outline=out_col)
    draw.line([(23, 17), (23, 22)], fill=bright_gold)
    draw.line([(26, 17), (26, 22)], fill=high_gold)

    # Specular shine
    img.putpixel((6, 11), white_shine)
    img.putpixel((18, 14), white_shine)

    return img

def create_trap():
    img = Image.new('RGBA', (48, 48), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # 3 Forged Iron Spikes with heavy baseplate, x: 2 to 46, y: 9 to 47
    sh_out = hex_to_rgba('#101317')
    plate_deep = hex_to_rgba('#1e2229')
    plate_mid = hex_to_rgba('#38404d')
    plate_high = hex_to_rgba('#5a667a')
    
    st_shadow = hex_to_rgba('#272f3d')
    st_mid = hex_to_rgba('#505f78')
    st_light = hex_to_rgba('#7d92b5')
    st_high = hex_to_rgba('#b5cbe8')
    st_gleam = hex_to_rgba('#ffffff')
    danger_red = hex_to_rgba('#b8182a')

    # BASEPLATE (y: 38 to 47, x: 2 to 45)
    draw.rectangle([2, 38, 45, 47], fill=plate_mid, outline=sh_out)
    draw.line([(3, 39), (44, 39)], fill=plate_high) # top bevel
    draw.line([(3, 47), (44, 47)], fill=plate_deep) # bottom shadow
    # Heavy Hexagonal Studded Rivets on plate
    for rx in [6, 15, 24, 33, 41]:
        draw.rectangle([rx, 42, rx+2, 44], fill=st_high, outline=sh_out)
        img.putpixel((rx+1, 43), st_gleam)

    # SPIKES: Left, Center (Taller), Right
    # Left Spike: apex at (12, 15), base from x: 5 to 19, y: 38
    # Center Spike: apex at (24, 9), base from x: 17 to 31, y: 38
    # Right Spike: apex at (36, 15), base from x: 29 to 43, y: 38

    spikes = [
        (12, 15, 5, 19),   # Left
        (36, 15, 29, 43),  # Right
        (24, 9, 17, 31)    # Center (Front)
    ]

    for apex_x, apex_y, base_l, base_r in spikes:
        # Outline triangle
        draw.polygon([(apex_x, apex_y), (base_l, 38), (base_r, 38)], outline=sh_out, fill=st_shadow)
        # Left facet (Shadow facet)
        draw.polygon([(apex_x, apex_y), (base_l+1, 38), (apex_x, 38)], fill=st_mid)
        # Right facet (Light facet)
        draw.polygon([(apex_x, apex_y), (apex_x, 38), (base_r-1, 38)], fill=st_light)
        # Center Spine / Ridge Highlight
        draw.line([(apex_x, apex_y), (apex_x, 37)], fill=st_gleam, width=1)
        # Razor tip gleam & danger blood tint
        img.putpixel((apex_x, apex_y), st_gleam)
        img.putpixel((apex_x, apex_y+1), danger_red)
        img.putpixel((apex_x-1, apex_y+2), danger_red)
        img.putpixel((apex_x+1, apex_y+2), st_high)
        # Serration notches
        img.putpixel((base_l+2, 30), sh_out)
        img.putpixel((base_r-2, 30), sh_out)

    return img

def create_ground():
    # 64x64 Seamless horizontally tileable Dungeon Flagstone / Stone Platform
    img = Image.new('RGBA', (64, 64), (0, 0, 0, 255))
    draw = ImageDraw.Draw(img)

    # Color Palette: Ancient Dungeon Stone & Flagstones with emerald moss crevices
    out_dark = hex_to_rgba('#14171a')
    stone_darkest = hex_to_rgba('#1f232b')
    stone_dark = hex_to_rgba('#2f3642')
    stone_mid = hex_to_rgba('#434d5e')
    stone_light = hex_to_rgba('#5d6a80')
    stone_high = hex_to_rgba('#8294b0')
    stone_peak = hex_to_rgba('#aabdd9')

    moss_dark = hex_to_rgba('#163d1f')
    moss_mid = hex_to_rgba('#2b6e3b')
    moss_bright = hex_to_rgba('#489e5a')
    moss_high = hex_to_rgba('#74c987')

    # Fill base
    draw.rectangle([0, 0, 63, 63], fill=stone_dark)

    # 1. TOP WALKING FLAGSTONES (y: 0 to 18)
    # Distinct paving slabs: Slab 1 (x: 0 to 31), Slab 2 (x: 32 to 63)
    # Wrap horizontally: x=0 and x=63 join mortar at x=32 and x=0/64
    for slab_x0, slab_x1 in [(0, 31), (32, 63)]:
        draw.rectangle([slab_x0, 0, slab_x1, 16], fill=stone_mid)
        # Top bevel edge (illuminated top face)
        draw.line([(slab_x0, 0), (slab_x1, 0)], fill=stone_peak)
        draw.line([(slab_x0, 1), (slab_x1, 1)], fill=stone_high)
        # Left bevel (light)
        draw.line([(slab_x0, 0), (slab_x0, 15)], fill=stone_light)
        # Right bevel (shadow)
        draw.line([(slab_x1, 0), (slab_x1, 16)], fill=stone_darkest)
        # Bottom bevel (shadow)
        draw.line([(slab_x0, 16), (slab_x1, 16)], fill=out_dark)

        # Subtle stone surface noise/chisel marks
        for i in range(5):
            cx = (slab_x0 + 5 + i * 5) % 64
            draw.line([(cx, 4), (cx + 2, 4)], fill=stone_light)
            draw.line([(cx + 1, 9), (cx + 4, 9)], fill=stone_darkest)

    # Mortar joints between top slabs (at x=31/32 and x=63/0)
    for mx in [0, 31, 32, 63]:
        draw.line([(mx, 0), (mx, 17)], fill=out_dark)

    # Moss growing on top & in mortar crevices
    for mx in [31, 32, 0, 63]:
        draw.line([(mx, 0), (mx, 5)], fill=moss_mid)
        img.putpixel(((mx+1)%64, 1), moss_bright)
        img.putpixel(((mx-1)%64, 1), moss_bright)
        img.putpixel((mx, 2), moss_high)

    # Subtle moss fringe along top walking edge
    for x in range(0, 64):
        if (x * 7 + 3) % 11 < 4:
            img.putpixel((x, 0), moss_bright)
            img.putpixel((x, 1), moss_mid)

    # 2. SUB-SURFACE FOUNDATION MASONRY (y: 18 to 63)
    # Staggered stone blocks:
    # Row 1 (y: 18 to 32): 2 blocks: [0..23], [24..63] -> wrap check: [0..23] and [24..63]
    # Row 2 (y: 33 to 48): 2 blocks: [0..43], [44..63]
    # Row 3 (y: 49 to 63): 2 blocks: [0..15], [16..51], [52..63]
    
    rows = [
        (18, 32, [0, 28]),
        (33, 47, [0, 48]),
        (48, 63, [0, 20, 50])
    ]

    for y0, y1, cut_points in rows:
        # Draw horizontal mortar line
        draw.line([(0, y0), (63, y0)], fill=out_dark)
        draw.line([(0, y1), (63, y1)], fill=out_dark)
        
        # Block bounds
        cuts = sorted(list(set(cut_points + [64])))
        for i in range(len(cuts)-1):
            bx0 = cuts[i]
            bx1 = cuts[i+1] - 1
            # Fill block
            draw.rectangle([bx0, y0+1, bx1, y1-1], fill=stone_mid if (i + y0)%2 == 0 else stone_dark)
            # Top bevel
            draw.line([(bx0, y0+1), (bx1, y0+1)], fill=stone_light)
            # Bottom bevel
            draw.line([(bx0, y1-1), (bx1, y1-1)], fill=stone_darkest)
            # Left & Right mortar
            draw.line([(bx0, y0), (bx0, y1)], fill=out_dark)
            draw.line([(bx1, y0), (bx1, y1)], fill=out_dark)

            # Crack / weathering texture
            mid_x = (bx0 + bx1) // 2
            mid_y = (y0 + y1) // 2
            draw.line([(mid_x-2, mid_y), (mid_x+1, mid_y)], fill=stone_darkest)
            img.putpixel((mid_x+2, mid_y), stone_light)

    # Ancient moss spores in lower mortar joints
    for px, py in [(28, 20), (28, 21), (48, 35), (48, 36), (20, 50), (20, 51)]:
        img.putpixel((px, py), moss_mid)
        img.putpixel(((px+1)%64, py), moss_dark)

    # Ambient darkening at the very bottom
    for y in range(58, 64):
        for x in range(64):
            r, g, b, a = img.getpixel((x, y))
            factor = (64 - y) / 7.0
            img.putpixel((x, y), (int(r*factor), int(g*factor), int(b*factor), a))

    return img

def create_wall():
    # 64x64 Seamless horizontally and vertically tileable Castle Dungeon Wall
    img = Image.new('RGBA', (64, 64), (0, 0, 0, 255))
    draw = ImageDraw.Draw(img)

    out_dark = hex_to_rgba('#121417')
    brick_deep = hex_to_rgba('#252b36')
    brick_dark = hex_to_rgba('#373f4d')
    brick_mid = hex_to_rgba('#4b5566')
    brick_light = hex_to_rgba('#657287')
    brick_high = hex_to_rgba('#8a99b0')

    moss_mid = hex_to_rgba('#2d5e38')
    moss_bright = hex_to_rgba('#489458')

    draw.rectangle([0, 0, 63, 63], fill=brick_dark)

    # 4 rows of brickwork (16px high each): y in [0..15], [16..31], [32..47], [48..63]
    # Running bond pattern: offset by 32px every row
    # Row 0: cuts at 0, 32
    # Row 1: cuts at 16, 48
    # Row 2: cuts at 0, 32
    # Row 3: cuts at 16, 48
    # Seamless wrap in Y: top of row 0 matches bottom of row 3 mortar!
    
    wall_rows = [
        (0, 15, [0, 32]),
        (16, 31, [16, 48]),
        (32, 47, [0, 32]),
        (48, 63, [16, 48])
    ]

    for y0, y1, cut_list in wall_rows:
        # Mortar line top and bottom
        draw.line([(0, y0), (63, y0)], fill=out_dark)
        draw.line([(0, y1), (63, y1)], fill=out_dark)
        
        cuts = sorted(list(set(cut_list + [64])))
        if cuts[0] != 0: cuts = [0] + cuts
        
        for i in range(len(cuts)-1):
            bx0 = cuts[i]
            bx1 = cuts[i+1] - 1
            if bx0 > bx1: continue
            
            # Brick base
            fill_col = brick_mid if (i + y0//16) % 2 == 0 else brick_dark
            draw.rectangle([bx0+1, y0+1, bx1-1, y1-1], fill=fill_col)
            
            # Top bevel highlight
            draw.line([(bx0+1, y0+1), (bx1-1, y0+1)], fill=brick_light)
            # Left bevel highlight
            draw.line([(bx0+1, y0+1), (bx0+1, y1-1)], fill=brick_light)
            # Bottom bevel shadow
            draw.line([(bx0+1, y1-1), (bx1-1, y1-1)], fill=brick_deep)
            # Right bevel shadow
            draw.line([(bx1-1, y0+1), (bx1-1, y1-1)], fill=brick_deep)
            
            # Mortar joints
            draw.line([(bx0, y0), (bx0, y1)], fill=out_dark)
            draw.line([(bx1, y0), (bx1, y1)], fill=out_dark)

            # Weathered stone detail
            cx = (bx0 + bx1) // 2
            cy = (y0 + y1) // 2
            img.putpixel((cx, cy), brick_high)
            img.putpixel((cx+1, cy), brick_deep)
            img.putpixel((cx-3, cy+2), brick_deep)

    # Moss accents in crevices
    for mx, my in [(16, 16), (48, 32), (32, 48), (0, 0), (63, 0)]:
        img.putpixel((mx % 64, my % 64), moss_bright)
        img.putpixel(((mx+1) % 64, my % 64), moss_mid)

    return img

if __name__ == '__main__':
    os.makedirs('tools/output', exist_ok=True)
    create_player().save('tools/output/Player.png')
    create_enemy().save('tools/output/Enemy.png')
    create_coin().save('tools/output/Coin.png')
    create_key().save('tools/output/Key.png')
    create_trap().save('tools/output/Trap.png')
    create_ground().save('tools/output/Ground.png')
    create_wall().save('tools/output/Wall.png')
    print('Generated all 7 sprites in tools/output/')
