using TMPro;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text playerNameText;

    [SerializeField]
    private TMP_Text scoreText;

    public void Init(string playerName, int score)
    {
        this.playerNameText.text = playerName;
        this.scoreText.text = score.ToString();
    }
}