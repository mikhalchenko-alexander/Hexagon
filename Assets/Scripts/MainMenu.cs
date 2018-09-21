using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [SerializeField] private GameObject _modePanel;
    [SerializeField] private GameObject _boardPanel;
    
    public void QuitGame() {
        Application.Quit();
    }

    public void SetPvPMode() {
        SetMode(GameMode.PvP);
    }
    
    public void SetPvCMode() {
        SetMode(GameMode.PvC);
       
    }

    public void SetMode(GameMode gameMode) {
        Settings.GameMode = gameMode;
        _boardPanel.SetActive(true);
        _modePanel.SetActive(false);
    }

    public void BackToModeSelection() {
        _boardPanel.SetActive(false);
        _modePanel.SetActive(true);
    }
    
    public void LoadHexagonalBoard() {
        SceneManager.LoadScene("HexagonBoard");
    }
    
    public void LoadRectangleBoard() {
        SceneManager.LoadScene("RectangleBoard");
    }

}