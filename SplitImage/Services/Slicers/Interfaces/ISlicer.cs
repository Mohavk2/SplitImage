using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitImage.Services.Slicers.Interfaces
{
    public interface ISlicer
    {
        public Bitmap[,] Slice(Bitmap target);
        public string[] SupportedImageFormats { get; }
    }
}
