using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.UI;

public class ServiceManager : MonoBehaviour
{       
    public Text authStatusText;

    private void Start()
    {
        Authentication();
    }

    public void CallBackAuthentication(bool result)
    {
        authStatusText.text = "";

        if (result == true)
        {
            //authStatusText.text = "Google Play Games: " + Social.localUser.userName;
        }
        else
        {
            authStatusText.text = "";
        }
    }

    private void CallBackReportScore(bool result)
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI();
    }

    public void ShowLeaderboard()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
        else
        {
            Debug.Log("Cannot show leaderboard: not authenticated");
        }
    }


    public void SendScore(int score, string HighscoreId = GPGSIds.leaderboard_highscore)
    {
        PlayGamesPlatform.Instance.Authenticate((bool success) =>
        {
            if (success)
            {
                PlayGamesPlatform.Instance.ReportScore(score, HighscoreId, CallBackReportScore);
            }
        });
    }

    // Start is called before the first frame update
    private void Authentication()
    {
        PlayGamesClientConfiguration config = new
            PlayGamesClientConfiguration.Builder()
            .Build();

        // Enable debugging output (recommended)
        PlayGamesPlatform.DebugLogEnabled = true;

        // Initialize and activate the platform
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        PlayGamesPlatform.Instance.Authenticate(CallBackAuthentication, true);
        /*
        if (PlayGamesPlatform.Instance.GetUserId() == "0")
        {
            PlayGamesPlatform.Instance.Authenticate(CallBackAuthentication, false);
        }
        */        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
