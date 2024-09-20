using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarHandler : MonoBehaviour
{
    [SerializeField]
    private List<Image> Stars;

    [SerializeField]
    private Sprite fullStarSprite;

    [SerializeField]
    private Sprite emptyStarSprite;

    public void ChangeDifficulty(int difficulty)
    {
        for (int i = 0; i < difficulty; i++)
        {
            Stars[i].sprite = fullStarSprite;
        }
        for (int i = difficulty; i < Stars.Count; i++)
        {
            Stars[i].sprite = emptyStarSprite;
        }
    }
}