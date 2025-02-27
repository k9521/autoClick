using System;
using System.Drawing;


namespace autoClicker
{
    [Serializable]
    public class ClickParameters
    {
        public Point? Start { get; set; }
        public Point? End { get; set; }
        public bool Slide { get; set; }
        public int RNDPoint { get; set; }
        public int WaitDurationTime { get; set; }
        public int WaitAfterTime { get; set; }
        public int WaitRNDAfterTime { get; set; }
        public int WaitRNDDurationTime { get; set; }
    }
}
