using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    private Button startButton = null;

    private void Awake()
    {
        Assert.IsNotNull(startButton);
    }

    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(StartGame);
    }

    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
