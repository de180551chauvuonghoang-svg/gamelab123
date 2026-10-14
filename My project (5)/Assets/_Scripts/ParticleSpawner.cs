using UnityEngine;

/// <summary>
/// Hệ thống sinh hiệu ứng Particle hoàn toàn bằng code — không cần prefab.
/// Gọi các hàm static từ bất kỳ script nào: ParticleSpawner.SpawnLandingDust(pos);
/// </summary>
public static class ParticleSpawner
{
    // ========================================================================
    // 1. BỤI KHI TIẾP ĐẤT (Landing Dust)
    // ========================================================================
    public static void SpawnLandingDust(Vector3 position, float intensity = 1f)
    {
        GameObject go = new GameObject("LandingDust");
        go.transform.position = position;
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.playOnAwake = false;
        main.startLifetime = 0.35f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.8f * intensity, 2.2f * intensity);
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.18f);
        main.startColor = new Color(0.82f, 0.75f, 0.6f, 0.6f);
        main.maxParticles = 12;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0.4f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        int burstCount = Mathf.Clamp((int)(6 * intensity), 4, 12);
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)burstCount) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.25f;
        shape.arc = 180f;
        shape.rotation = new Vector3(-90f, 0f, 0f);

        var colorOverLife = ps.colorOverLifetime;
        colorOverLife.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.82f, 0.75f, 0.6f), 0f), new GradientColorKey(new Color(0.7f, 0.65f, 0.5f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.7f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLife.color = grad;

        var sizeOverLife = ps.sizeOverLifetime;
        sizeOverLife.enabled = true;
        sizeOverLife.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 1, 1, 0.3f));

        SetupRenderer(go);
        ps.Play();
        Object.Destroy(go, 1f);
    }

    // ========================================================================
    // 2. BỤI KHI NHẢY (Jump Puff)
    // ========================================================================
    public static void SpawnJumpDust(Vector3 position)
    {
        GameObject go = new GameObject("JumpDust");
        go.transform.position = position;
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.playOnAwake = false;
        main.startLifetime = 0.25f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.12f);
        main.startColor = new Color(0.88f, 0.83f, 0.7f, 0.5f);
        main.maxParticles = 6;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.1f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 4) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.15f;
        shape.arc = 120f;
        shape.rotation = new Vector3(-90f, 0f, 0f);

        var colorOverLife = ps.colorOverLifetime;
        colorOverLife.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.85f, 0.8f, 0.65f), 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.5f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLife.color = grad;

        SetupRenderer(go);
        ps.Play();
        Object.Destroy(go, 0.8f);
    }

    // ========================================================================
    // 3. VÀNG NỔ KHI NHẶT COIN (Coin Burst)
    // ========================================================================
    public static void SpawnCoinBurst(Vector3 position)
    {
        GameObject go = new GameObject("CoinBurst");
        go.transform.position = position;
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.playOnAwake = false;
        main.startLifetime = 0.5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 3.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.14f);
        main.startColor = new Color(1f, 0.85f, 0.15f, 1f);
        main.maxParticles = 16;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0.5f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 10) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.1f;

        var colorOverLife = ps.colorOverLifetime;
        colorOverLife.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.9f, 0.3f), 0f), new GradientColorKey(new Color(1f, 0.6f, 0f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLife.color = grad;

        SetupRenderer(go);
        ps.Play();
        Object.Destroy(go, 1.2f);
    }

    // ========================================================================
    // 4. NỔ ĐỎ KHI QUÁI CHẾT (Enemy Death)
    // ========================================================================
    public static void SpawnEnemyDeath(Vector3 position)
    {
        GameObject go = new GameObject("EnemyDeath");
        go.transform.position = position;
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.playOnAwake = false;
        main.startLifetime = 0.6f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.22f);
        main.startColor = new Color(1f, 0.35f, 0.15f, 0.9f);
        main.maxParticles = 14;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0.6f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 10) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.15f;

        var colorOverLife = ps.colorOverLifetime;
        colorOverLife.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.5f, 0.2f), 0f), new GradientColorKey(new Color(0.5f, 0.1f, 0.05f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.9f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLife.color = grad;

        SetupRenderer(go);
        ps.Play();
        Object.Destroy(go, 1.5f);
    }

    // ========================================================================
    // 5. TỎA SÁNG KHI KÍCH HOẠT CHECKPOINT (Checkpoint Sparkle)
    // ========================================================================
    public static void SpawnCheckpointSparkle(Vector3 position)
    {
        GameObject go = new GameObject("CheckpointSparkle");
        go.transform.position = position;
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.playOnAwake = false;
        main.startLifetime = 0.8f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.12f);
        main.startColor = new Color(0.3f, 1f, 0.5f, 0.9f);
        main.maxParticles = 20;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.3f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 14) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.3f;

        var colorOverLife = ps.colorOverLifetime;
        colorOverLife.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.4f, 1f, 0.6f), 0f), new GradientColorKey(new Color(0.9f, 1f, 0.95f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLife.color = grad;

        SetupRenderer(go);
        ps.Play();
        Object.Destroy(go, 1.5f);
    }

    // ========================================================================
    // 6. SAO NHỎ KHI COMBO (Combo Stars)
    // ========================================================================
    public static void SpawnComboStars(Vector3 position, int comboLevel)
    {
        GameObject go = new GameObject("ComboStars");
        go.transform.position = position + Vector3.up * 0.5f;
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.playOnAwake = false;
        main.startLifetime = 0.7f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.15f);
        // Màu vàng sáng -> cam -> đỏ theo combo level
        float hue = Mathf.Clamp01(1f - comboLevel * 0.15f) * 0.15f;
        main.startColor = Color.HSVToRGB(hue, 1f, 1f);
        main.maxParticles = 10;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.5f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        int count = Mathf.Clamp(4 + comboLevel * 2, 4, 16);
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)count) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.2f;

        var colorOverLife = ps.colorOverLifetime;
        colorOverLife.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.HSVToRGB(hue, 1f, 1f), 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLife.color = grad;

        SetupRenderer(go);
        ps.Play();
        Object.Destroy(go, 1.2f);
    }

    // ========================================================================
    // SETUP RENDERER CHUNG
    // ========================================================================
    private static void SetupRenderer(GameObject go)
    {
        var renderer = go.GetComponent<ParticleSystemRenderer>();
        if (renderer == null) return;

        // Tìm shader particle phù hợp URP, fallback dần dần
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit")
                     ?? Shader.Find("Particles/Standard Unlit")
                     ?? Shader.Find("Sprites/Default");

        if (shader != null)
        {
            Material mat = new Material(shader);
            // Dùng texture trắng để tô màu bằng startColor
            mat.mainTexture = Texture2D.whiteTexture;
            renderer.material = mat;
        }

        renderer.renderMode = ParticleSystemRenderMode.Billboard;
    }
}
