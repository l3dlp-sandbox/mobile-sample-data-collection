﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Carto.Core;
using Carto.Utils;

namespace data.collection.Droid
{
    [Activity(Label = "CHOOSE LOCATION")]
    public class LocationChoiceActivity : BaseActivity, ILocationListener, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        public LocationChoiceView ContentView { get; set; }

        public LocationClient LocationClient { get; set; }

        LocationManager manager;

        LocationChoiceListener Listener { get; set; }

        string[] Permissions
        {
            get
            {
                return new string[] {
                    Android.Manifest.Permission.AccessCoarseLocation,
                    Android.Manifest.Permission.AccessFineLocation
                };
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ContentView = new LocationChoiceView(this);
            SetContentView(ContentView);

			LocationClient = new LocationClient(ContentView.MapView);

			if (IsMarshmallow)
			{
				RequestPermissions(Permissions);
			}
			else
			{
				OnPermissionsGranted();
			}

			Listener = new LocationChoiceListener(ContentView.MapView);
            var bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.icon_pin_red);
            Listener.Bitmap = BitmapUtils.CreateBitmapFromAndroidBitmap(bitmap);

			Listener.QueryPoints(DeviceId);
			string text = "QUERYING POINTS...";
			ContentView.Banner.SetLoadingText(text, false);
		}

        protected override void OnResume()
        {
            base.OnResume();

			LocationClient.AttachIgnoreListener();

			ContentView.MapView.MapEventListener = Listener;

			Listener.PinAdded += OnPinAdded;
			Listener.QueryFailed += OnQueryFailed;
			Listener.PointsAdded += OnPointsAdded;

			ContentView.Done.Clicked += OnDoneClick;
        }

        protected override void OnPause()
        {
            base.OnPause();

			LocationClient.DetachIgnoreListener();

			ContentView.MapView.MapEventListener = null;

			Listener.PinAdded -= OnPinAdded;
			Listener.QueryFailed -= OnQueryFailed;
			Listener.PointsAdded -= OnPointsAdded;

			ContentView.Done.Clicked -= OnDoneClick;
        }

        void OnPinAdded(object sender, EventArgs e)
        {
            MapPos position = Listener.MarkerPosition;
            position = Listener.Projection.ToLatLong(position.X, position.Y);

            LocationClient.MarkerLatitude = position.X;
            LocationClient.MarkerLongitude = position.Y;

            RunOnUiThread(delegate
            {
                ContentView.Done.Show();
            });
        }

		void OnQueryFailed(object sender, EventArgs e)
		{
			string text = "CLICK ON THE MAP TO SPECIFY A LOCATION";

            RunOnUiThread(delegate
			{
				ContentView.Banner.SetInformationText(text, false);
			});
		}

		void OnPointsAdded(object sender, EventArgs e)
		{
			var syncedColor = LocationChoiceListener.SyncedLocations.ToNativeColor();
			var mySyncedColor = LocationChoiceListener.MySyncedLocations.ToNativeColor();
			var unsyncedColor = LocationChoiceListener.UnsyncedLocations.ToNativeColor();

			string text = "CLICK ON THE MAP TO SPECIFY A LOCATION";

            RunOnUiThread(delegate
			{
				ContentView.Banner.SetInformationText(text, false);
				//ContentView.Legend.Update(syncedColor, mySyncedColor, unsyncedColor);
			});
		}
		
        void OnDoneClick(object sender, EventArgs e)
		{
            OnBackPressed();
		}

		public override void OnPermissionsGranted()
        {
            manager = (LocationManager)GetSystemService(LocationService);

            foreach (string provider in manager.GetProviders(true))
            {
                manager.RequestLocationUpdates(provider, 1000, 50, this);
            }
        }

        public override void OnPermissionsDenied()
        {
            // TODO
        }

        void RequestLocationPermission()
        {
            string fine = Android.Manifest.Permission.AccessFineLocation;
            string coarse = Android.Manifest.Permission.AccessCoarseLocation;
            ActivityCompat.RequestPermissions(this, new string[] { fine, coarse }, RequestCode);
        }

        public void OnLocationChanged(Location location)
        {
            LocationFound(location);
        }

        public void OnProviderDisabled(string provider)
        {
            Alert("Location provider disabled, bro!");
        }

        public void OnProviderEnabled(string provider)
        {
            Alert("Location provider enabled... scanning for location");
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            Console.WriteLine("OnStatusChanged");
        }

        void LocationFound(Location location)
        {
            LocationClient.Latitude = location.Latitude;
            LocationClient.Longitude = location.Longitude;
            LocationClient.Accuracy = location.Accuracy;

            LocationClient.Update();
        }
    }
}