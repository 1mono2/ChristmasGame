using System;
using UnityEngine;
using UnityEngine.Events;
using GoogleMobileAds.Api;
using MoNo.Utility;


public class InterstitialAds : SingletonMonoBehaviour<InterstitialAds>
{
    protected override bool DontDestroy => true;


#if UNITY_ANDROID
    [SerializeField] string adUnitIdAndroid = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
    [SerializeField] string adUnitIdiOS = "ca-app-pub-3940256099942544/4411468910";
#endif

    // Ads
    private InterstitialAd interstitialAd;

    [System.Serializable]
    public class Provider : UnityEvent { }
    public Provider OnAdLoaded;
    public Provider OnAdFailedToLoad;
    public Provider OnAdOpening;
    public Provider OnAdClosed;

    const string SAVE_PURCHASING_AD_FLAG = "PurchasingAdFlag"; // 1:purshased 0: NOT purchased

    void Start()
    {
        if (PlayerPrefs.GetInt(SAVE_PURCHASING_AD_FLAG) == 1)
        {
            Destroy(this);
            return;
        }


        CreateAndLoadRewardAd();
    }

    private void CreateAndLoadRewardAd()
    {
        // Initiralize instance
        if (this.interstitialAd != null)
        {
            this.interstitialAd.Destroy();
        }

#if UNITY_ANDROID
    string adUnitId = adUnitIdAndroid;
#elif UNITY_IPHONE
        string adUnitId = adUnitIdiOS;
#else
    string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.interstitialAd = new InterstitialAd(adUnitId);


        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitialAd.LoadAd(request);


        AddHandle();

    }

    public void ShowIfLoaded()
    {
        if (this.interstitialAd.IsLoaded())
        {
            this.interstitialAd.Show();
        }
        else
        {
            Debug.Log("don't loaded");
        }

    }

    private void AddHandle()
    {
        this.interstitialAd.OnAdLoaded += HandleOnAdLoaded;
        this.interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        this.interstitialAd.OnAdOpening += HandleOnAdOpening;
        this.interstitialAd.OnAdClosed += HandleOnAdClosed;

    }



    private void HandleOnAdLoaded(object sender, EventArgs args)
    {
        OnAdLoaded.Invoke();
        Debug.Log("Interstitial ad has successed to load ad.");
    }


    private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        OnAdFailedToLoad.Invoke();
        Debug.Log("Rewarded interstitial ad has failed to load ad.");
    }

    private void HandleOnAdOpening(object sender, EventArgs args)
    {
        OnAdOpening.Invoke();
        Debug.Log("Rewarded interstitial ad has opened.");

    }

    private void HandleOnAdClosed(object sender, EventArgs args)
    {
        OnAdClosed.Invoke();
        Debug.Log("Rewarded interstitial ad has closed");

        // When close Ads, create next one;
        Dispose();
        CreateAndLoadRewardAd();
    }




    public void Dispose()
    {
        this.interstitialAd.OnAdLoaded -= HandleOnAdLoaded;
        this.interstitialAd.OnAdFailedToLoad -= HandleOnAdFailedToLoad;
        this.interstitialAd.OnAdOpening -= HandleOnAdOpening;
        this.interstitialAd.OnAdClosed -= HandleOnAdClosed;

    }

    void OnDestroy()
    {
        if (this.interstitialAd != null)
        {
            Dispose();
        }
    }

}
