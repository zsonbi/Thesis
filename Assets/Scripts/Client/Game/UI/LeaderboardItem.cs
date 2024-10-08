using TMPro;
using UnityEngine;

namespace Game
{
    namespace UI
    {
        public class LeaderboardItem : ThreadSafeMonoBehaviour
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
    }
}