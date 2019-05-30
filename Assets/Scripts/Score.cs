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

    public void Reset()
    {
        text.text = "0";
    }

    public void UpdateScore(int score)
    {
        text.text = score.ToString();
    }
}
