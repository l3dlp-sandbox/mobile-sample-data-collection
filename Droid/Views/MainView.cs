﻿
using System;
using Android.Content;
using Carto.Layers;
using Carto.Ui;

namespace data.collection.Droid
{
    public class MainView : BannerView
    {
		public MapView MapView { get; private set; }

        public ActionButton Done { get; private set; }

        public Legend Legend { get; private set; }

        public SlideInPopup Popup { get; private set; }

		public MainView(Context context) : base(context)
        {
			MapView = new MapView(context);
			AddView(MapView);

            Done = new ActionButton(context, Resource.Drawable.icon_done);
            Done.SetBackground(Colors.AppleBlue);
            AddView(Done);

            Legend = new Legend(context);
            AddView(Legend);

            // Just use close icon for both images,
            // as back icon is not used in this application
            int close = Resource.Drawable.icon_close;
            Popup = new SlideInPopup(context, close, close);
            AddView(Popup);

            SetMainViewFrame();

			var layer = new CartoOnlineVectorTileLayer(CartoBaseMapStyle.CartoBasemapStyleVoyager);
			MapView.Layers.Add(layer);

            Banner.BringToFront();
            Popup.BringToFront();

            Done.Hide();
		}

		public override void LayoutSubviews()
		{
            base.LayoutSubviews();

			MapView.SetFrame(0, 0, Frame.W, Frame.H);

            int pad = (int)(15 * Density);

            int w = (int)(55 * Density);
			int h = w;
            int x = Frame.W - (w + pad);
            int y = Frame.H - (h + pad);

			Done.Frame = new CGRect(x, y, w, h);

            int legendPadding = (int)(5 * Density);

            w = (int)(220 * Density);
            h = (int)(100 * Density);
            x = Frame.W - (w + legendPadding);
            y = legendPadding;

			Legend.Frame = new CGRect(x, y, w, h);

            x = 0;
            y = 0;
            w = Frame.W;
            h = Frame.H;

            Popup.Frame = new CGRect(x, y, w, h);
		}

    }
}
