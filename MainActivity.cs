using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace SaqiRoyalPatented
{
    [Activity(
        Label = "SaqiRoyalPatented",
        Theme = "@style/AppTheme.NoActionBar",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : AppCompatActivity
    {
        private WebView _webView;
        private FrameLayout _customViewContainer;
        private View _customView;
        private WebChromeClient.ICustomViewCallback _customViewCallback;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _webView = FindViewById<WebView>(Resource.Id.webView);
            _customViewContainer = FindViewById<FrameLayout>(Resource.Id.customViewContainer);

            _webView.Settings.JavaScriptEnabled = true;
            _webView.Settings.DomStorageEnabled = true;
            _webView.Settings.AllowFileAccess = true;
            _webView.Settings.MediaPlaybackRequiresUserGesture = false;
            _webView.SetWebViewClient(new WebViewClient());
            _webView.SetWebChromeClient(new CustomWebChromeClient(this));

            if (savedInstanceState == null)
            {
                _webView.LoadUrl("https://www.saqiroyalpatented.com/");
            }
        }



        public override void OnBackPressed()
        {
            if (_customView != null)
            {
                ExitFullScreen();
            }
            else if (_webView.CanGoBack())
            {
                _webView.GoBack();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public void EnterFullScreen(View view, WebChromeClient.ICustomViewCallback callback)
        {
            _customView = view;
            _customViewCallback = callback;

            _customViewContainer.AddView(view);
            _customViewContainer.Visibility = ViewStates.Visible;
            _webView.Visibility = ViewStates.Gone;

            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                SystemUiFlags.Fullscreen |
                SystemUiFlags.HideNavigation |
                SystemUiFlags.ImmersiveSticky);
        }


        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            _webView.SaveState(outState);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            _webView.RestoreState(savedInstanceState);
        }
        public void ExitFullScreen()
        {
            _customViewContainer.RemoveView(_customView);
            _customView = null;
            _customViewContainer.Visibility = ViewStates.Gone;
            _webView.Visibility = ViewStates.Visible;

            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                SystemUiFlags.Visible);
            _customViewCallback.OnCustomViewHidden();
        }

        private class CustomWebChromeClient : WebChromeClient
        {
            private readonly MainActivity _activity;

            public CustomWebChromeClient(MainActivity activity)
            {
                _activity = activity;
            }

            public override void OnShowCustomView(View view, ICustomViewCallback callback)
            {
                _activity.EnterFullScreen(view, callback);
            }

            public override void OnHideCustomView()
            {
                _activity.ExitFullScreen();
            }
        }
    }
}
