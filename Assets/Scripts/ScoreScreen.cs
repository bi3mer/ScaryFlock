using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ScoreScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text = null;

    [SerializeField]
    private Button backToMainMenu = null;

    [SerializeField]
    private Button restart = null;

    private void Awake()
    {
        Assert.IsNotNull(backToMainMenu);
        Assert.IsNotNull(restart);
        Assert.IsNotNull(text);
    }

    private void Start()
    {
        backToMainMenu.onClick.AddListener(LoadMainMenu);
        restart.onClick.AddListener(Restart);
    }

    private void OnDestroy()
    {
        backToMainMenu.onClick.RemoveListener(LoadMainMenu);
        restart.onClick.RemoveListener(Restart);
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void Restart()
    {
        SceneManager.LoadScene(1);
    }

    public void SetScore(float yourScore)
    {
        float highScore;
        if (PlayerPrefs.HasKey("highscore"))
        {
            highScore = PlayerPrefs.GetFloat("highscore");

            if (highScore < yourScore)
            {
                PlayerPrefs.SetFloat("highscore", yourScore);
                text.text = $"Your Score: {yourScore}\nOld High Score: {highScore}";
                return;
            }
        }
        else
        {
            PlayerPrefs.SetFloat("highscore", yourScore);
            highScore = yourScore;
        }

        text.text = $"Your Score: {yourScore}\nHigh Score: {highScore}";
    }
}
