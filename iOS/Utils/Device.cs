﻿using System;
using UIKit;

namespace data.collection.iOS
{
    public class Device
    {
        public static string Id => UIDevice.CurrentDevice.IdentifierForVendor.ToString();

		public static nfloat StatusBarHeight
		{
			get { return Shared.StatusBarFrame.Height; }
		}

		public static nfloat TrueY0
        {
            get { return NavigationBarHeight + StatusBarHeight; }
        }

        public static UIApplication Shared
        {
            get { return UIApplication.SharedApplication; }
        }

        public static nfloat NavigationBarHeight
        {
            get
            {
                var appDelegate = (AppDelegate)Shared.Delegate;
                return appDelegate.Controller.NavigationBar.Frame.Height;
            }
        }

        public static bool IsLandscape
        {
            get
            {
                var orientation = UIDevice.CurrentDevice.Orientation;
                return orientation == UIDeviceOrientation.LandscapeLeft || orientation == UIDeviceOrientation.LandscapeRight;
            }
        }

        public static bool IsTablet
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad; }
        }

    }
}
