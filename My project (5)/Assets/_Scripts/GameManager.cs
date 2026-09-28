using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("--- ĐIỂM SỐ & GIAO DIỆN UI ---")]
    [Tooltip("Text hiển thị điểm số (TextMeshPro)")]
    public TextMeshProUGUI scoreText;
    private int score = 0;

    [Header("--- BẢNG THÔNG BÁO ---")]
    [Tooltip("Panel hiển thị khi Game Over")]
    public GameObject gameOverPanel;
    
    [Tooltip("Panel hiển thị khi chiến thắng (You Win)")]
    public GameObject winPanel;

    private bool isGameEnded = false;

    private void Awake()
    {
        // Mô hình Singleton: Đảm bảo chỉ có duy nhất 1 GameManager hoạt động
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Đặt lại thời gian bình thường khi bắt đầu màn chơi
        Time.timeScale = 1f;
        isGameEnded = false;

        // Cập nhật điểm số ban đầu
        UpdateScoreUI();

        // Ẩn các bảng thông báo
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    // Cộng điểm khi nhặt coin
    public void AddScore(int amount)
    {
        if (isGameEnded) return;

        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    // Kích hoạt trạng thái Game Over khi chạm bẫy hoặc quái
    public void GameOver()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        Debug.Log("<color=red><b>[Game Over]</b> Người chơi đã thua!</color>");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Tạm dừng mọi chuyển động vật lý trong game
        Time.timeScale = 0f;
    }

    // Kích hoạt trạng thái chiến thắng khi nhặt được chìa khóa (Key)
    public void WinGame()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        Debug.Log("<color=yellow><b>[You Win]</b> Chúc mừng! Bạn đã chiến thắng!</color>");

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    // Nút Play Again: Chơi lại màn hiện tại
    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Nút Main Menu: Quay về Menu chính
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        // Nếu có scene MainMenu thì load, nếu chưa có thì load lại scene hiện tại
        if (Application.CanStreamedLevelBeLoaded("MainMenu"))
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
