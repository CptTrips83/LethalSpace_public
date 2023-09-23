using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdvertisingController : MonoBehaviour
{
    public string gameId = "3460819";
    public string placementId = "LevelBanner";
    public bool testMode = false;

    void Start()
    {
        Advertisement.Initialize(gameId, testMode);
        StartCoroutine(ShowBannerWhenReady());
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(placementId))
        {
            yield return new WaitForSeconds(0.5f);
        }
        

        
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);        

        Advertisement.Banner.Show(placementId);
    }
}
