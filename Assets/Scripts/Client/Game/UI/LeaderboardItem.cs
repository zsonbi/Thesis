using TMPro;
using UnityEngine;
using Utility;

namespace Game
{
    namespace UI
    {
        /// <summary>
        /// A single leaderboard item
        /// </summary>
        public class LeaderboardItem : ThreadSafeMonoBehaviour
        {
            /// <summary>
            /// Reference to the player name's text
            /// </summary>
            [SerializeField]
            private TMP_Text playerNameText;

            /// <summary>
            /// Reference to the score text
            /// </summary>
            [SerializeField]
            private TMP_Text scoreText;

            /// <summary>
            /// Initializes the item
            /// </summary>
            /// <param name="playerName">The achieved player's name</param>
            /// <param name="score">The achieved score</param>
            public void Init(string playerName, int score)
            {
                this.playerNameText.text = playerName;
                this.scoreText.text = score.ToString();
            }
        }
    }
}