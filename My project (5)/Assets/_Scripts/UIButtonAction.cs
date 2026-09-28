using UnityEngine;
using UnityEngine.UI;

public class UIButtonAction : MonoBehaviour
{
    public enum ActionType
    {
        PlayAgain,
        MainMenu
    }

    [Tooltip("Hành động khi nhấn nút")]
    public ActionType actionType;

    private void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        if (GameManager.instance == null) return;

        if (actionType == ActionType.PlayAgain)
        {
            GameManager.instance.PlayAgain();
        }
        else if (actionType == ActionType.MainMenu)
        {
            GameManager.instance.LoadMainMenu();
        }
    }
}
