namespace Camera_NET
{
    using DirectShowLib;
    using System;

    public class VideoInput
    {
        public readonly int Index;
        public static readonly PhysicalConnectorType PhysicalConnectorType_Default;
        public readonly PhysicalConnectorType Type;

        public VideoInput(int index, PhysicalConnectorType type)
        {
            this.Index = index;
            this.Type = type;
        }

        public VideoInput Clone()
        {
            return new VideoInput(this.Index, this.Type);
        }

        public static VideoInput Default
        {
            get
            {
                return new VideoInput(-1, PhysicalConnectorType_Default);
            }
        }
    }
}

