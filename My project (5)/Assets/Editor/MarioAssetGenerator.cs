#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// Trình tạo toàn bộ Texture & Sprite chuẩn phong cách pixel art Super Mario 8-bit / 16-bit.
/// Tự động sinh file PNG và cấu hình Sprite (Point filter, không mờ, pixel per unit = 16).
/// </summary>
public static class MarioAssetGenerator
{
    private const string FOLDER = "Assets/Sprites/Mario/";

    [MenuItem("Lab 2/🍄 Tạo Toàn Bộ Sprite Chuẩn Mario (Generate Pixel Art)", false, 10)]
    public static void GenerateAllMarioSprites()
    {
        if (!Directory.Exists(FOLDER))
        {
            Directory.CreateDirectory(FOLDER);
        }

        CreateQuestionBlock();
        CreateBrickBlock();
        CreateEmptyBlock();
        CreateSuperMushroom();
        CreateFireFlower();
        CreateStarman();
        CreateGoomba();
        CreateKoopa();
        CreateKoopaShell();
        CreatePipeTop();
        CreatePipeBody();
        CreateFlagpole();
        CreateCastle();
        CreateFireball();

        AssetDatabase.Refresh();
        Debug.Log("<color=#FFCC00><b>[Mario] Toàn bộ 14 Sprite pixel art Mario đã được khởi tạo thành công!</b></color>");
    }

    private static void SaveTexture(Texture2D tex, string fileName)
    {
        byte[] bytes = tex.EncodeToPNG();
        string fullPath = Path.Combine(FOLDER, fileName);
        File.WriteAllBytes(fullPath, bytes);

        AssetDatabase.ImportAsset(fullPath, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = AssetImporter.GetAtPath(fullPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 16;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }

    // ========================================================================
    // 1. QUESTION BLOCK [?] (16x16)
    // ========================================================================
    private static void CreateQuestionBlock()
    {
        Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        Color gold = new Color(0.98f, 0.72f, 0.15f);
        Color shadow = new Color(0.72f, 0.44f, 0.05f);
        Color light = new Color(1f, 0.92f, 0.55f);
        Color darkBorder = new Color(0.2f, 0.1f, 0.05f);

        // Fill gold
        for (int x = 0; x < 16; x++)
            for (int y = 0; y < 16; y++)
                tex.SetPixel(x, y, gold);

        // Borders
        for (int i = 0; i < 16; i++)
        {
            tex.SetPixel(i, 0, darkBorder);
            tex.SetPixel(i, 15, darkBorder);
            tex.SetPixel(0, i, darkBorder);
            tex.SetPixel(15, i, darkBorder);
        }

        // Inner light top & left
        for (int i = 1; i < 15; i++)
        {
            tex.SetPixel(i, 14, light);
            tex.SetPixel(1, i, light);
            tex.SetPixel(i, 1, shadow);
            tex.SetPixel(14, i, shadow);
        }

        // Corner rivets
        tex.SetPixel(2, 13, darkBorder);
        tex.SetPixel(13, 13, darkBorder);
        tex.SetPixel(2, 2, darkBorder);
        tex.SetPixel(13, 2, darkBorder);

        // '?' pattern in dark brown
        int[,] qmark = new int[,] {
            { 0,1,1,1,0 },
            { 1,0,0,0,1 },
            { 0,0,0,1,0 },
            { 0,0,1,0,0 },
            { 0,0,1,0,0 },
            { 0,0,0,0,0 },
            { 0,0,1,0,0 }
        };

        for (int r = 0; r < 7; r++)
        {
            for (int c = 0; c < 5; c++)
            {
                if (qmark[r, c] == 1)
                {
                    tex.SetPixel(5 + c, 11 - r, darkBorder);
                }
            }
        }

        tex.Apply();
        SaveTexture(tex, "Block_Question.png");
    }

    // ========================================================================
    // 2. BRICK BLOCK (16x16)
    // ========================================================================
    private static void CreateBrickBlock()
    {
        Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        Color brickColor = new Color(0.85f, 0.35f, 0.12f);
        Color mortar = new Color(0.2f, 0.08f, 0.02f);
        Color highlight = new Color(0.98f, 0.55f, 0.28f);

        for (int x = 0; x < 16; x++)
            for (int y = 0; y < 16; y++)
                tex.SetPixel(x, y, brickColor);

        // Mortar lines (horizontal)
        for (int x = 0; x < 16; x++)
        {
            tex.SetPixel(x, 0, mortar);
            tex.SetPixel(x, 5, mortar);
            tex.SetPixel(x, 10, mortar);
            tex.SetPixel(x, 15, mortar);
        }

        // Mortar lines (vertical)
        for (int y = 1; y < 5; y++) { tex.SetPixel(8, y, mortar); }
        for (int y = 6; y < 10; y++) { tex.SetPixel(3, y, mortar); tex.SetPixel(12, y, mortar); }
        for (int y = 11; y < 15; y++) { tex.SetPixel(8, y, mortar); }

        // Highlights on top edge of each brick
        for (int x = 1; x < 15; x++)
        {
            if (x != 8) { tex.SetPixel(x, 4, highlight); tex.SetPixel(x, 14, highlight); }
            if (x != 3 && x != 12) { tex.SetPixel(x, 9, highlight); }
        }

        tex.Apply();
        SaveTexture(tex, "Block_Brick.png");
    }

    // ========================================================================
    // 3. EMPTY SOLID BLOCK (16x16)
    // ========================================================================
    private static void CreateEmptyBlock()
    {
        Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        Color brown = new Color(0.55f, 0.35f, 0.2f);
        Color dark = new Color(0.25f, 0.15f, 0.08f);
        Color light = new Color(0.7f, 0.48f, 0.3f);

        for (int x = 0; x < 16; x++)
            for (int y = 0; y < 16; y++)
                tex.SetPixel(x, y, brown);

        for (int i = 0; i < 16; i++)
        {
            tex.SetPixel(i, 0, dark);
            tex.SetPixel(i, 15, dark);
            tex.SetPixel(0, i, dark);
            tex.SetPixel(15, i, dark);
            tex.SetPixel(i, 14, light);
            tex.SetPixel(1, i, light);
        }

        // 4 bolts
        tex.SetPixel(3, 12, dark);
        tex.SetPixel(12, 12, dark);
        tex.SetPixel(3, 3, dark);
        tex.SetPixel(12, 3, dark);

        tex.Apply();
        SaveTexture(tex, "Block_Empty.png");
    }

    // ========================================================================
    // 4. SUPER MUSHROOM (16x16)
    // ========================================================================
    private static void CreateSuperMushroom()
    {
        Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        Color red = new Color(0.9f, 0.15f, 0.1f);
        Color white = Color.white;
        Color skin = new Color(1f, 0.85f, 0.7f);
        Color black = Color.black;
        Color clear = new Color(0, 0, 0, 0);

        for (int x = 0; x < 16; x++)
            for (int y = 0; y < 16; y++)
                tex.SetPixel(x, y, clear);

        // Cap shape
        for (int y = 6; y <= 14; y++)
        {
            int minX = (y >= 13) ? 4 : (y >= 10) ? 2 : 1;
            int maxX = 15 - minX;
            for (int x = minX; x <= maxX; x++)
            {
                tex.SetPixel(x, y, red);
            }
        }

        // White spots on cap
        for (int x = 6; x <= 9; x++)
            for (int y = 9; y <= 13; y++)
                tex.SetPixel(x, y, white);

        tex.SetPixel(2, 9, white); tex.SetPixel(3, 9, white); tex.SetPixel(3, 10, white);
        tex.SetPixel(13, 9, white); tex.SetPixel(12, 9, white); tex.SetPixel(12, 10, white);

        // Stem & face
        for (int y = 1; y <= 5; y++)
        {
            for (int x = 4; x <= 11; x++)
                tex.SetPixel(x, y, skin);
        }

        // Eyes
        tex.SetPixel(6, 3, black); tex.SetPixel(6, 4, black);
        tex.SetPixel(9, 3, black); tex.SetPixel(9, 4, black);

        // Black outline bottom
        for (int x = 4; x <= 11; x++) tex.SetPixel(x, 0, black);

        tex.Apply();
        SaveTexture(tex, "Item_Mushroom.png");
    }

    // ========================================================================
    // 5. FIRE FLOWER (16x16)
    // ========================================================================
    private static void CreateFireFlower()
    {
        Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        Color orange = new Color(1f, 0.5f, 0f);
        Color yellow = new Color(1f, 0.95f, 0.1f);
        Color white = Color.white;
        Color green = new Color(0.2f, 0.8f, 0.2f);
        Color black = Color.black;
        Color clear = new Color(0, 0, 0, 0);

        for (int x = 0; x < 16; x++)
            for (int y = 0; y < 16; y++)
                tex.SetPixel(x, y, clear);

        // Flower Petals (Orange outer ring, Yellow inner)
        for (int x = 4; x <= 11; x++)
            for (int y = 7; y <= 14; y++)
                tex.SetPixel(x, y, orange);

        for (int x = 5; x <= 10; x++)
            for (int y = 8; y <= 13; y++)
                tex.SetPixel(x, y, yellow);

        for (int x = 6; x <= 9; x++)
            for (int y = 9; y <= 12; y++)
                tex.SetPixel(x, y, white);

        // Eyes
        tex.SetPixel(7, 10, black); tex.SetPixel(7, 11, black);
        tex.SetPixel(8, 10, black); tex.SetPixel(8, 11, black);

        // Stem & leaves
        for (int y = 1; y <= 6; y++)
        {
            tex.SetPixel(7, y, green);
            tex.SetPixel(8, y, green);
        }
        tex.SetPixel(5, 3, green); tex.SetPixel(6, 3, green); tex.SetPixel(4, 2, green);
        tex.SetPixel(9, 3, green); tex.SetPixel(10, 3, green); tex.SetPixel(11, 2, green);

        tex.Apply();
        SaveTexture(tex, "Item_FireFlower.png");
    }

    // ========================================================================
    // 6. STARMAN (16x16)
    // ========================================================================
    private static void CreateStarman()
    {
        Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        Color yellow = new Color(1f, 0.92f, 0.1f);
        Color eye = Color.black;
        Color clear = new Color(0, 0, 0, 0);

        for (int x = 0; x < 16; x++)
            for (int y = 0; y < 16; y++)
                tex.SetPixel(x, y, clear);

        // Star body
        for (int y = 3; y <= 13; y++)
        {
            int minX = (y >= 11) ? 6 : (y >= 8) ? 3 : 2;
            int maxX = 15 - minX;
            for (int x = minX; x <= maxX; x++)
                tex.SetPixel(x, y, yellow);
        }
        // Bottom legs
        for (int y = 1; y <= 3; y++)
        {
            tex.SetPixel(3, y, yellow); tex.SetPixel(4, y, yellow);
            tex.SetPixel(11, y, yellow); tex.SetPixel(12, y, yellow);
        }
        // Eyes
        tex.SetPixel(6, 7, eye); tex.SetPixel(6, 8, eye);
        tex.SetPixel(9, 7, eye); tex.SetPixel(9, 8, eye);

        tex.Apply();
        SaveTexture(tex, "Item_Star.png");
    }

    // ========================================================================
    // 7. GOOMBA (16x16)
    // ========================================================================
    private static void CreateGoomba()
    {
        Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        Color brown = new Color(0.68f, 0.35f, 0.12f);
        Color dark = new Color(0.35f, 0.15f, 0.05f);
        Color skin = new Color(1f, 0.85f, 0.65f);
        Color black = Color.black;
        Color white = Color.white;
        Color clear = new Color(0, 0, 0, 0);

        for (int x = 0; x < 16; x++)
            for (int y = 0; y < 16; y++)
                tex.SetPixel(x, y, clear);

        // Mushroom head
        for (int y = 5; y <= 14; y++)
        {
            int minX = (y >= 13) ? 4 : (y >= 10) ? 2 : 1;
            int maxX = 15 - minX;
            for (int x = minX; x <= maxX; x++)
                tex.SetPixel(x, y, brown);
        }

        // Stem
        for (int y = 2; y <= 5; y++)
        {
            for (int x = 4; x <= 11; x++)
                tex.SetPixel(x, y, skin);
        }

        // Eyes & eyebrows
        tex.SetPixel(5, 7, white); tex.SetPixel(6, 7, black);
        tex.SetPixel(9, 7, black); tex.SetPixel(10, 7, white);
        tex.SetPixel(4, 9, dark); tex.SetPixel(5, 8, dark);
        tex.SetPixel(11, 9, dark); tex.SetPixel(10, 8, dark);

        // Feet
        for (int x = 2; x <= 5; x++) tex.SetPixel(x, 0, black);
        for (int x = 10; x <= 13; x++) tex.SetPixel(x, 0, black);
        for (int x = 2; x <= 5; x++) tex.SetPixel(x, 1, black);
        for (int x = 10; x <= 13; x++) tex.SetPixel(x, 1, black);

        tex.Apply();
        SaveTexture(tex, "Enemy_Goomba.png");
    }

    // ========================================================================
    // 8. KOOPA TROOPA & SHELL (16x16)
    // ========================================================================
    private static void CreateKoopa()
    {
        Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        Color green = new Color(0.2f, 0.75f, 0.2f);
        Color yellowSkin = new Color(1f, 0.88f, 0.2f);
        Color white = Color.white;
        Color black = Color.black;
        Color clear = new Color(0, 0, 0, 0);

        for (int x = 0; x < 16; x++)
            for (int y = 0; y < 16; y++)
                tex.SetPixel(x, y, clear);

        // Head
        for (int x = 3; x <= 8; x++)
            for (int y = 10; y <= 14; y++)
                tex.SetPixel(x, y, yellowSkin);

        // Eye
        tex.SetPixel(5, 12, black);

        // Shell
        for (int x = 6; x <= 13; x++)
            for (int y = 4; y <= 11; y++)
                tex.SetPixel(x, y, green);

        // Shell white border
        for (int x = 6; x <= 13; x++)
            tex.SetPixel(x, 4, white);

        // Feet
        for (int x = 7; x <= 9; x++) { tex.SetPixel(x, 1, yellowSkin); tex.SetPixel(x, 0, yellowSkin); }
        for (int x = 11; x <= 13; x++) { tex.SetPixel(x, 1, yellowSkin); tex.SetPixel(x, 0, yellowSkin); }

        tex.Apply();
        SaveTexture(tex, "Enemy_Koopa.png");
    }

    private static void CreateKoopaShell()
    {
        Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        Color green = new Color(0.18f, 0.78f, 0.22f);
        Color shadow = new Color(0.1f, 0.45f, 0.12f);
        Color white = Color.white;
        Color dark = Color.black;
        Color clear = new Color(0, 0, 0, 0);

        for (int x = 0; x < 16; x++)
            for (int y = 0; y < 16; y++)
                tex.SetPixel(x, y, clear);

        // Dome shell
        for (int y = 2; y <= 12; y++)
        {
            int minX = (y >= 10) ? 4 : (y >= 6) ? 2 : 1;
            int maxX = 15 - minX;
            for (int x = minX; x <= maxX; x++)
                tex.SetPixel(x, y, green);
        }

        // White rim
        for (int x = 1; x <= 14; x++)
        {
            tex.SetPixel(x, 2, white);
            tex.SetPixel(x, 3, white);
        }

        // Hexagon lines
        tex.SetPixel(7, 8, shadow); tex.SetPixel(8, 8, shadow);
        tex.SetPixel(7, 7, white); tex.SetPixel(8, 7, white);

        tex.Apply();
        SaveTexture(tex, "Enemy_KoopaShell.png");
    }

    // ========================================================================
    // 9. WARP PIPES (Pipe Top & Pipe Body 32x16)
    // ========================================================================
    private static void CreatePipeTop()
    {
        Texture2D tex = new Texture2D(32, 16, TextureFormat.RGBA32, false);
        Color pipeGreen = new Color(0.15f, 0.75f, 0.15f);
        Color lightGreen = new Color(0.55f, 0.95f, 0.4f);
        Color darkGreen = new Color(0.08f, 0.42f, 0.08f);
        Color black = Color.black;

        for (int x = 0; x < 32; x++)
            for (int y = 0; y < 16; y++)
                tex.SetPixel(x, y, pipeGreen);

        // Highlight stripe
        for (int y = 1; y < 15; y++)
        {
            tex.SetPixel(3, y, lightGreen);
            tex.SetPixel(4, y, lightGreen);
            tex.SetPixel(5, y, lightGreen);
            tex.SetPixel(28, y, darkGreen);
            tex.SetPixel(29, y, darkGreen);
            tex.SetPixel(30, y, darkGreen);
        }

        // Outline
        for (int x = 0; x < 32; x++)
        {
            tex.SetPixel(x, 0, black);
            tex.SetPixel(x, 15, black);
        }
        for (int y = 0; y < 16; y++)
        {
            tex.SetPixel(0, y, black);
            tex.SetPixel(31, y, black);
        }

        tex.Apply();
        SaveTexture(tex, "Pipe_Top.png");
    }

    private static void CreatePipeBody()
    {
        Texture2D tex = new Texture2D(32, 16, TextureFormat.RGBA32, false);
        Color pipeGreen = new Color(0.15f, 0.75f, 0.15f);
        Color lightGreen = new Color(0.55f, 0.95f, 0.4f);
        Color darkGreen = new Color(0.08f, 0.42f, 0.08f);
        Color black = Color.black;
        Color clear = new Color(0, 0, 0, 0);

        // Clear side indentations (Pipe body is narrower than Pipe Top by 2px on each side)
        for (int x = 0; x < 32; x++)
            for (int y = 0; y < 16; y++)
            {
                if (x < 2 || x > 29) tex.SetPixel(x, y, clear);
                else tex.SetPixel(x, y, pipeGreen);
            }

        for (int y = 0; y < 16; y++)
        {
            tex.SetPixel(2, y, black);
            tex.SetPixel(5, y, lightGreen);
            tex.SetPixel(6, y, lightGreen);
            tex.SetPixel(27, y, darkGreen);
            tex.SetPixel(28, y, darkGreen);
            tex.SetPixel(29, y, black);
        }

        tex.Apply();
        SaveTexture(tex, "Pipe_Body.png");
    }

    // ========================================================================
    // 10. FLAGPOLE & CASTLE & FIREBALL
    // ========================================================================
    private static void CreateFlagpole()
    {
        Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        Color pole = new Color(0.85f, 0.85f, 0.85f);
        Color greenFlag = new Color(0.1f, 0.8f, 0.2f);
        Color clear = new Color(0, 0, 0, 0);

        for (int x = 0; x < 16; x++)
            for (int y = 0; y < 16; y++)
                tex.SetPixel(x, y, clear);

        // Vertical pole in middle
        for (int y = 0; y < 16; y++)
        {
            tex.SetPixel(2, y, pole);
            tex.SetPixel(3, y, pole);
        }

        // Triangular flag
        for (int y = 8; y <= 14; y++)
        {
            int flagWidth = (14 - y) * 2 + 1;
            for (int x = 4; x < 4 + flagWidth && x < 16; x++)
                tex.SetPixel(x, y, greenFlag);
        }

        tex.Apply();
        SaveTexture(tex, "Flagpole_Flag.png");
    }

    private static void CreateCastle()
    {
        Texture2D tex = new Texture2D(32, 32, TextureFormat.RGBA32, false);
        Color brick = new Color(0.45f, 0.5f, 0.6f);
        Color darkDoor = new Color(0.1f, 0.12f, 0.15f);
        Color white = Color.white;
        Color clear = new Color(0, 0, 0, 0);

        for (int x = 0; x < 32; x++)
            for (int y = 0; y < 32; y++)
                tex.SetPixel(x, y, clear);

        // Castle Base
        for (int x = 4; x < 28; x++)
            for (int y = 0; y < 24; y++)
                tex.SetPixel(x, y, brick);

        // Battlements (Thành răng cưa)
        for (int x = 4; x < 28; x++)
        {
            if (x % 4 < 2)
            {
                for (int y = 24; y < 28; y++)
                    tex.SetPixel(x, y, brick);
            }
        }

        // Door arch
        for (int x = 12; x <= 19; x++)
            for (int y = 0; y < 12; y++)
                tex.SetPixel(x, y, darkDoor);

        tex.Apply();
        SaveTexture(tex, "Castle.png");
    }

    private static void CreateFireball()
    {
        Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        Color red = new Color(0.95f, 0.2f, 0.1f);
        Color yellow = new Color(1f, 0.85f, 0.15f);
        Color white = Color.white;
        Color clear = new Color(0, 0, 0, 0);

        for (int x = 0; x < 16; x++)
            for (int y = 0; y < 16; y++)
                tex.SetPixel(x, y, clear);

        for (int x = 5; x <= 10; x++)
            for (int y = 5; y <= 10; y++)
                tex.SetPixel(x, y, red);

        for (int x = 6; x <= 9; x++)
            for (int y = 6; y <= 9; y++)
                tex.SetPixel(x, y, yellow);

        tex.SetPixel(7, 8, white);
        tex.SetPixel(8, 8, white);

        tex.Apply();
        SaveTexture(tex, "Fireball.png");
    }
}
#endif
