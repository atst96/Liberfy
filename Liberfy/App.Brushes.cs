using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Liberfy
{
    partial class App
    {
        public static class Brushes
        {
            public static Brush Retweet = GetResource<Brush>("Brush.Retweet");
            public static Brush Favorite = GetResource<Brush>("Brush.Favorite");
            public static Brush RetweetFavorite = GetResource<Brush>("Brush.RetweetFavorite");
        }
    }
}
