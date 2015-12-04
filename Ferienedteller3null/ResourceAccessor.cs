using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ferienedteller3null
{
    internal static class ResourceAccessor
    {
        public static ImageSource Get(string respurcePath)
        {
            var uri = $"pack://application:,,,/{Assembly.GetExecutingAssembly().GetName().Name};component/{respurcePath}";
			var imgSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
            return imgSource;
        }
    }

    public class ImageSourceStore
    {
        public string Name { get; set; }
        public ImageSource ImageSource { get; set; }
    }
}
