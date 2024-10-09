using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    namespace UI
    {
        /// <summary>
        /// Star handler
        /// </summary>
        public class StarHandler : ThreadSafeMonoBehaviour
        {
            /// <summary>
            /// Reference to the star image container
            /// </summary>
            [SerializeField]
            private List<Image> Stars;

            /// <summary>
            /// Full star sprite
            /// </summary>
            [SerializeField]
            private Sprite fullStarSprite;

            /// <summary>
            /// Empty star sprite
            /// </summary>
            [SerializeField]
            private Sprite emptyStarSprite;

            /// <summary>
            /// Update the display based on the difficulty
            /// </summary>
            /// <param name="difficulty">The current difficulty</param>
            public void ChangeDifficulty(int difficulty)
            {
                for (int i = 0; i < difficulty; i++)
                {
                    Stars[i].sprite = emptyStarSprite;
                }
                for (int i = difficulty; i < Stars.Count; i++)
                {
                    Stars[i].sprite = fullStarSprite;
                }
            }
        }
    }
}