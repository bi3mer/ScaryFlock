using UnityEngine.Assertions;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Score : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(text);
    }

    private void Start()
    {
        text.text = "0";
    }

    public void Reset()
    {
        text.text = "0";
    }

    public void UpdateScore()
    {
        text.text = GameManager.Instance.PreyCount.ToString();
    }
}
