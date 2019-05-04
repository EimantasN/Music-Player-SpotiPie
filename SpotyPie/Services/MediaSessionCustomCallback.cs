using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media.Session;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SpotyPie.Services
{
    public class MediaSessionCustomCallback : MediaSession.Callback
    {
        public override bool OnMediaButtonEvent(Intent mediaButtonIntent)
        {
            return true;
        }
    }
}