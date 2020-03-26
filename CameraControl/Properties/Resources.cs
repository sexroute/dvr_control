namespace CameraControl.Properties
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
    internal class Resources
    {
        private static CultureInfo resourceCulture;
        private static System.Resources.ResourceManager resourceMan;

        internal Resources()
        {
        }

        internal static Bitmap AliPay
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("AliPay", resourceCulture);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        internal static Bitmap haiou
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("haiou", resourceCulture);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (resourceMan == null)
                {
                    resourceMan = new System.Resources.ResourceManager("CameraControl.Properties.Resources", typeof(Resources).Assembly);
                }
                return resourceMan;
            }
        }

        internal static Bitmap WeiXinPay
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("WeiXinPay", resourceCulture);
            }
        }
    }
}

