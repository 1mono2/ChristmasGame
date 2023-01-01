using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using MoNo.Utility;
using Cysharp.Threading.Tasks;
using Facebook.Unity;


namespace MoNo.Christmas
{
    public class LoadData : SingletonMonoBehaviour<LoadData> 
    {
        protected override bool DontDestroy => true;

        [SerializeField] bool _isShowAd = true;
        //[SerializeField] private BannerAdGameObject _bannerAdGameObject;
        BannerView bannerView;

        private bool adsTrackingEnabledOnFB = true;
        private bool adsTrackingEnabledOnGoogle = true;
        
        public bool isShowAd => _isShowAd;
        const string SAVE_STAGE_INDEX = "StageIndex";


        protected override void Awake()
        {
            base.Awake();
            
            RequestAds();

        }
        


        void RequestAds()
        {
            if (_isShowAd == false) return;
            
# if UNITY_IOS
	        
            AppTrackingTransparencyCheck att = new();
            UniTask.Void(async () =>
            {
	            await att.Check();
	            att.AppTransparencyTrackingRequestCompleted(OnAuthorized, OnDenied);
            });
#endif

	        // admob initialize
	        MobileAds.Initialize(initStatus =>
	        {
		        // Setting the same app key.
		        RequestConfiguration requestConfiguration = new RequestConfiguration.Builder()
			        .SetSameAppKeyEnabled(adsTrackingEnabledOnGoogle)
			        .build();
		        MobileAds.SetRequestConfiguration(requestConfiguration);
		        // AdMobからのコールバックはメインスレッドで呼び出される保証がないため、次のUpdate()で呼ばれるようにMobileAdsEventExecutorを使用
		        MobileAdsEventExecutor.ExecuteInUpdate(RequestBanner);
	        });
	        
	        // Initialize Facebook Audience Network
	        FB.Init(() =>
	        {
		        if (FB.IsInitialized)
		        {
			        FB.ActivateApp();
			        FB.Mobile.SetAdvertiserTrackingEnabled(adsTrackingEnabledOnFB);
		        }
		        else
		        {
			        Debug.Log("Failed to Initialize the Facebook SDK");
		        }
	        });
            
	        

        }

        void OnAuthorized()
        {
	        adsTrackingEnabledOnFB = true;
	        adsTrackingEnabledOnGoogle = true;
	        Debug.Log("OnAuthorized");
        }
        
        void OnDenied()
		{
			adsTrackingEnabledOnFB = false;
			adsTrackingEnabledOnGoogle = false;
	        Debug.Log("OnDenied");

		}
        
        private void RequestBanner()
        {
#if UNITY_ANDROID
            string adUnitId = "ca-app-pub-1767951001352951/3215520425";
#elif UNITY_IPHONE
	        string adUnitId = "ca-app-pub-1767951001352951/3898046705";
#else
            string adUnitId = "unexpected_platform";
#endif

	        // Create a 320x50 banner at the top of the screen.
	        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
	        // Create an empty ad request.
	        AdRequest request = new AdRequest.Builder().Build();

	        // Load the banner with the request.
	        this.bannerView.LoadAd(request);
	        bannerView.Show();
        }
        
        
        private void Start()
        {
            LoadScene();

        }

        public static void LoadScene()
        {
	        // Load stage index.
	        if (!PlayerPrefs.HasKey(SAVE_STAGE_INDEX))
	        {
		        SceneManager.LoadScene(1);
		        return;
	        }

	        int savedSceneNum = PlayerPrefs.GetInt(SAVE_STAGE_INDEX);
	        if (savedSceneNum < SceneManager.sceneCountInBuildSettings - 1)
	        {
		        SceneManager.LoadScene(savedSceneNum + 1);
	        }
	        else
	        {
		        SceneManager.LoadScene(1);
	        }
        }

    }
}
