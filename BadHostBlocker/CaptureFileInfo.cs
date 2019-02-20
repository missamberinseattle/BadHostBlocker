using System;

namespace BadHostBlocker
{
    public class CaptureFileInfo
    {
        public CaptureFileInfo()
        {
            SetOn = DateTime.MinValue;
        }

        public DateTime SetOn { get; set;  }
        public string Path { get; set; }
    }
}