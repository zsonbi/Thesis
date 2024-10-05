using Config;
using System;
using System.Collections.Generic;
using Thesis_backend.Data_Structures;
using TMPro;
using UnityEngine;
using User;

public class LeaderboardWindow : MonoBehaviour
{
    [SerializeField]
    private GameObject scoreParent;

    [SerializeField]
    private TMP_Dropdown filterTypeDropdown;

    [SerializeField]
    private ModalWindow ModalWindow;

    [SerializeField]
    private GameObject LeaderboardItemPrefab;

    public void Show()
    {
        this.gameObject.SetActive(true);
        LoadScores();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void LoadScores()
    {
        DateTime filterSince = DateTime.Now;
        switch ((LeaderboardFilterType)filterTypeDropdown.value)
        {
            case LeaderboardFilterType.AllTime:
                filterSince = DateTime.MinValue;
                break;

            case LeaderboardFilterType.ThisYear:
                filterSince = DateTime.UtcNow.AddYears(-1);
                break;

            case LeaderboardFilterType.ThisMonth:
                filterSince = DateTime.UtcNow.AddMonths(-1);
                break;

            case LeaderboardFilterType.ThisWeek:
                filterSince = DateTime.UtcNow.AddDays(-7);
                break;

            case LeaderboardFilterType.Today:
                filterSince = DateTime.UtcNow.AddDays(-1);
                break;

            default:
                Debug.LogError("No such leaderboard filter type is known");
                break;
        }

        CoroutineRunner.RunCoroutine(Server.SendGetRequest<List<GameScore>>(ServerConfig.PATH_FOR_GET_GAME_SCORES(filterSince), LoadedScores, onFailedAction: ShowRequestFail));
    }

    private void ShowRequestFail(string content)
    {
        ModalWindow.Show("Shop error", content);
    }

    private void LoadedScores(List<GameScore> gameScores)
    {
        if (scoreParent == null || this.scoreParent.transform == null)
        {
            // Exit or handle the case when the Score parent is destroyed
            Debug.LogWarning("Score parent has been destroyed or is missing.");
            return;
        }

        //Delete the previous ones
        for (int i = 0; i < this.scoreParent.transform.childCount; i++)
        {
            Destroy(this.scoreParent.transform.GetChild(i).gameObject);
        }
        this.scoreParent.transform.DetachChildren();

        foreach (var item in gameScores)
        {
            LeaderboardItem leaderboardItem = Instantiate(LeaderboardItemPrefab, scoreParent.transform).GetComponent<LeaderboardItem>();
            leaderboardItem.Init(item.OwnerName, item.Score);
        }
    }
}