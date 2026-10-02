using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("--- TÊN SCENE CHƠI GAME ---")]
    [Tooltip("Tên scene màn chơi chính")]
    public string gameSceneName = "SampleScene";

    // 1. Chức năng bắt đầu chơi game (Play)
    public void PlayGame()
    {
        Time.timeScale = 1f;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayButtonClickSFX();
        }

        // Tải scene màn chơi chính
        if (Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            // Hoặc tải scene thứ 2 trong Build Settings (index 1)
            SceneManager.LoadScene(1);
        }
    }

    // 2. Chức năng thoát game (Quit)
    public void QuitGame()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayButtonClickSFX();
        }

        Debug.Log("<color=yellow><b>[Main Menu]</b> Đang thoát trò chơi...</color>");

        // Thoát ứng dụng khi chạy bản build độc lập (.exe)
        Application.Quit();

#if UNITY_EDITOR
        // Dừng chế độ Play trong Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
