using SplitImage.Services.Slicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitImage.Services.Slicers.Structures
{
    public struct SettingStatus
    {
        public ISlicer? slicer;
        public bool isSettingCompleted;
    }
}
