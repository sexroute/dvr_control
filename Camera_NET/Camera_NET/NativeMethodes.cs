namespace Camera_NET
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    internal sealed class NativeMethodes
    {
        public const uint FILE_END = 2;

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern bool CloseHandle(IntPtr hObject);
        [DllImport("Kernel32.dll", EntryPoint="RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] int Length);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPTStr)] string filename, [MarshalAs(UnmanagedType.U4)] FileAccess access, [MarshalAs(UnmanagedType.U4)] FileShare share, IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes, IntPtr templateFile);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("oleaut32.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
        public static extern int OleCreatePropertyFrame(IntPtr hwndOwner, int x, int y, [MarshalAs(UnmanagedType.LPWStr)] string lpszCaption, int cObjects, [MarshalAs(UnmanagedType.Interface)] ref object ppUnk, int cPages, IntPtr lpPageClsID, int lcid, int dwReserved, IntPtr lpvReserved);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("kernel32.dll")]
        public static extern bool SetFilePointerEx(IntPtr hFile, long liDistanceToMove, IntPtr lpNewFilePointer, uint dwMoveMethod);
    }
}

