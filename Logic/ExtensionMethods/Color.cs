using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Falconwing.Logic.ExtensionMethods
{
    public static partial class DummyContainer
    {
        public static int GetLuminance (this Color color)
        {
            return (int) (color.R * 0.2126 + color.G * 0.7152 + color.B * 0.0722);
        }
    }
}
