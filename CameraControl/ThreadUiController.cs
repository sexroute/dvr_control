using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Diagnostics;
using System.IO;
/***
 * a class library for c# multithread ui op
 * author:zdl
 *
 * */

namespace EricZhao.UiThread
{
    public enum ListViewExtendedStyles
    {
        /// <summary>
        /// LVS_EX_GRIDLINES
        /// </summary>
        GridLines = 0x00000001,
        /// <summary>
        /// LVS_EX_SUBITEMIMAGES
        /// </summary>
        SubItemImages = 0x00000002,
        /// <summary>
        /// LVS_EX_CHECKBOXES
        /// </summary>
        CheckBoxes = 0x00000004,
        /// <summary>
        /// LVS_EX_TRACKSELECT
        /// </summary>
        TrackSelect = 0x00000008,
        /// <summary>
        /// LVS_EX_HEADERDRAGDROP
        /// </summary>
        HeaderDragDrop = 0x00000010,
        /// <summary>
        /// LVS_EX_FULLROWSELECT
        /// </summary>
        FullRowSelect = 0x00000020,
        /// <summary>
        /// LVS_EX_ONECLICKACTIVATE
        /// </summary>
        OneClickActivate = 0x00000040,
        /// <summary>
        /// LVS_EX_TWOCLICKACTIVATE
        /// </summary>
        TwoClickActivate = 0x00000080,
        /// <summary>
        /// LVS_EX_FLATSB
        /// </summary>
        FlatsB = 0x00000100,
        /// <summary>
        /// LVS_EX_REGIONAL
        /// </summary>
        Regional = 0x00000200,
        /// <summary>
        /// LVS_EX_INFOTIP
        /// </summary>
        InfoTip = 0x00000400,
        /// <summary>
        /// LVS_EX_UNDERLINEHOT
        /// </summary>
        UnderlineHot = 0x00000800,
        /// <summary>
        /// LVS_EX_UNDERLINECOLD
        /// </summary>
        UnderlineCold = 0x00001000,
        /// <summary>
        /// LVS_EX_MULTIWORKAREAS
        /// </summary>
        MultilWorkAreas = 0x00002000,
        /// <summary>
        /// LVS_EX_LABELTIP
        /// </summary>
        LabelTip = 0x00004000,
        /// <summary>
        /// LVS_EX_BORDERSELECT
        /// </summary>
        BorderSelect = 0x00008000,
        /// <summary>
        /// LVS_EX_DOUBLEBUFFER
        /// </summary>
        DoubleBuffer = 0x00010000,
        /// <summary>
        /// LVS_EX_HIDELABELS
        /// </summary>
        HideLabels = 0x00020000,
        /// <summary>
        /// LVS_EX_SINGLEROW
        /// </summary>
        SingleRow = 0x00040000,
        /// <summary>
        /// LVS_EX_SNAPTOGRID
        /// </summary>
        SnapToGrid = 0x00080000,
        /// <summary>
        /// LVS_EX_SIMPLESELECT
        /// </summary>
        SimpleSelect = 0x00100000
    }

    public enum ListViewMessages
    {
        First = 0x1000,
        SetExtendedStyle = (First + 54),
        GetExtendedStyle = (First + 55),
    }

    /// <summary>
    /// Contains helper methods to change extended styles on ListView, including enabling double buffering.
    /// Based on Giovanni Montrone's article on <see cref="http://www.codeproject.com/KB/list/listviewxp.aspx"/>
    /// </summary>
    public class ListViewHelper
    {
        private ListViewHelper()
        {
        }
       
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr handle, int messg, int wparam, int lparam);

        public static void SetExtendedStyle(Control control, ListViewExtendedStyles exStyle)
        {
            ListViewExtendedStyles styles;
            styles = (ListViewExtendedStyles)SendMessage(control.Handle, (int)ListViewMessages.GetExtendedStyle, 0, 0);
            styles |= exStyle;
            SendMessage(control.Handle, (int)ListViewMessages.SetExtendedStyle, 0, (int)styles);
        }

        public static void EnableDoubleBuffer(Control control)
        {
            ListViewExtendedStyles styles;
            // read current style
            styles = (ListViewExtendedStyles)SendMessage(control.Handle, (int)ListViewMessages.GetExtendedStyle, 0, 0);
            // enable double buffer and border select
            styles |= ListViewExtendedStyles.DoubleBuffer | ListViewExtendedStyles.BorderSelect;
            // write new style
            SendMessage(control.Handle, (int)ListViewMessages.SetExtendedStyle, 0, (int)styles);
        }
        public static void DisableDoubleBuffer(Control control)
        {
            ListViewExtendedStyles styles;
            // read current style
            styles = (ListViewExtendedStyles)SendMessage(control.Handle, (int)ListViewMessages.GetExtendedStyle, 0, 0);
            // disable double buffer and border select
            styles -= styles & ListViewExtendedStyles.DoubleBuffer;
            styles -= styles & ListViewExtendedStyles.BorderSelect;
            // write new style
            SendMessage(control.Handle, (int)ListViewMessages.SetExtendedStyle, 0, (int)styles);
        }


    }
   public class ThreadUiController
    {
       [DllImport("user32.dll")]
       public static extern bool SetForegroundWindow(IntPtr hWnd);
        public static string getControlText(System.Windows.Forms.Control argsControl)
        {
            CallerParameter loParameter = new CallerParameter();

            loParameter.Control = argsControl;

            ControlCaller loControlInovker = new ControlCaller(loParameter.Control);

            loControlInovker.Invoke(new MethodCallCaller(_getControlText), loParameter);

            return loParameter.Out_value.ToString();
        }
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public extern static void ExitProcess(uint uExitCode);
       public static void ApplicationExit()
       {
           ExitProcess(1);
       }

        public static void EnableControl(System.Windows.Forms.Control argsControl)
        {
            CallerParameter loParameter = new CallerParameter();

            loParameter.Control = argsControl;

            ControlCaller loControlInovker = new ControlCaller(loParameter.Control);

            loControlInovker.Invoke(new MethodCallCaller(_enableControl), loParameter);

        }

        public static void DisableControl(System.Windows.Forms.Control argsControl)
        {
            CallerParameter loParameter = new CallerParameter();

            loParameter.Control = argsControl;

            ControlCaller loControlInovker = new ControlCaller(loParameter.Control);

            loControlInovker.Invoke(new MethodCallCaller(_disableControl), loParameter);

        }

        private static void _enableControl(CallerParameter args)
        {
            if (null != args)
            {
                if (null != args.Control)
                {
                    args.Control.Enabled = true;
                }
            }
        }

        private static void _disableControl(CallerParameter args)
        {
            if (null != args)
            {
                if (null != args.Control)
                {
                    args.Control.Enabled = false;
                }
            }
        }

        public delegate void CustomMethodCallInvoker(System.Windows.Forms.Control argsControl);

        public static void InvokeControlCustomMethod2(System.Windows.Forms.Control argsControl, CustomMethodCallInvoker aoCustomMethod)
        {
            CallerParameter loParameter = new CallerParameter();
            loParameter.Control = argsControl;
            loParameter.In_value = aoCustomMethod;
            ControlCaller loControlInovker = new ControlCaller(loParameter.Control);
            loControlInovker.Invoke(new MethodCallCaller(_InvokeControlCustomMethod2), loParameter);
        }


        public static void InvokeControlCustomMethod(System.Windows.Forms.Control argsControl,MethodCallCaller aoCustomMethod)
        {
            CallerParameter loParameter = new CallerParameter();

            loParameter.Control = argsControl;

            loParameter.In_value = aoCustomMethod;

            ControlCaller loControlInovker = new ControlCaller(loParameter.Control);

            loControlInovker.Invoke(new MethodCallCaller(_InvokeControlCustomMethod), loParameter);
        }

        public static void InvokeControlCustomMethod3(System.Windows.Forms.Control argsControl, MethodCallCaller aoCustomMethod,Object[] args)
        {
            CallerParameter loParameter = new CallerParameter();
            loParameter.Control = argsControl;
            loParameter.In_value = args;
            ControlCaller loControlInovker = new ControlCaller(loParameter.Control);
            loControlInovker.Invoke(aoCustomMethod, loParameter);
        }

        private static void _InvokeControlCustomMethod(CallerParameter args)
        {
            if (null != args)
            {
                if (null != args.Control)
                {
                    if (null!= args.In_value )
                    {
                        if (args.in_value is MethodCallCaller)
                        {
                            ((MethodCallCaller)args.In_value).Invoke(args);
                        }
                    }
                }
            }
        }

        private static void _InvokeControlCustomMethod2(CallerParameter args)
        {
            if (null != args)
            {
                if (null != args.Control)
                {
                    if (null != args.In_value)
                    {
                        if (args.in_value is CustomMethodCallInvoker)
                        {
                            ((CustomMethodCallInvoker)args.In_value).Invoke(args.Control);
                        }
                    }
                }
            }
        }


        public static void ShowControl(System.Windows.Forms.Control argsControl)
        {
            CallerParameter loParameter = new CallerParameter();

            loParameter.Control = argsControl;

            ControlCaller loControlInovker = new ControlCaller(loParameter.Control);

            loControlInovker.Invoke(new MethodCallCaller(_ShowControl), loParameter);  
        }

        public static void HideControl(System.Windows.Forms.Control argsControl)
        {
            CallerParameter loParameter = new CallerParameter();

            loParameter.Control = argsControl;

            ControlCaller loControlInovker = new ControlCaller(loParameter.Control);

            loControlInovker.Invoke(new MethodCallCaller(_HideControl), loParameter);  
        }

        public static void SetControlText(System.Windows.Forms.Control argsControl,String astrValue)
        {
            CallerParameter loParameter = new CallerParameter();

            loParameter.Control = argsControl;

            loParameter.In_value = astrValue;

            ControlCaller loControlInovker = new ControlCaller(loParameter.Control);

            loControlInovker.Invoke(new MethodCallCaller(_setControlText), loParameter);           

        }


        private static void _getControlText(CallerParameter args)
        {
            if (null != args)
            {
                if (null!= args.Control)
                {
                    args.Out_value = args.Control.Text;
                }
            }
        }

        private static void _setControlText(CallerParameter args)
        {
            if (null != args)
            {
                if (null != args.Control)
                {
                    if (null!= args.In_value)
                    {
                        args.Control.Text = args.in_value.ToString();
                    }
                    
                }
            }
        }

        private static void _HideControl(CallerParameter args)
        {
            if (null != args)
            {
                if (null != args.Control)
                {
                    args.Control.Hide();
                }
            }
        }

        private static void _ShowControl(CallerParameter args)
        {
            if (null != args)
            {
                if (null != args.Control)
                {
                    args.Control.Show();
                }
            }
        }

        public class CallerParameter
        {
            public Control control;
            public System.Windows.Forms.Control Control
            {
                get { return control; }
                set { control = value; }
            }
            public object in_value;
            public object In_value
            {
                get { return in_value; }
                set { in_value = value; }
            }
            public object out_value;
            public object Out_value
            {
                get { return out_value; }
                set { out_value = value; }
            }
        }
        public delegate void MethodCallCaller(CallerParameter args);   
        public class ControlCaller
        {

            private class MethodCall
            {
                public MethodCallCaller invoker;

                public CallerParameter args;

                public MethodCall(MethodCallCaller invoker, CallerParameter args)
                {
                    this.invoker = invoker;
                    this.args = args;
                }
            }

            private Control control;
            private Queue<MethodCall> argumentInvokeList = new Queue<MethodCall>();
             
            public ControlCaller(Control control)
            {
                this.control = control;
            }

          
            public void Invoke(MethodCallCaller invoker, CallerParameter args)
            {
                this.argumentInvokeList.Enqueue(new MethodCall(invoker, args));

                try
                {
                    control.Invoke(new EventHandler(ControlInvoke));
                }
                catch (System.Exception ex)
                {
                    string lstrErrormsg = ex.Message;
                    /*System.Diagnostics.Debug.Assert(false);*/
                    System.Diagnostics.Debug.WriteLine(lstrErrormsg);
                    return;
                }                
                
            }

            private void ControlInvoke(object sender, EventArgs e)
            {
                MethodCall methodCall = (MethodCall)argumentInvokeList.Dequeue();
                methodCall.invoker(methodCall.args);
            }


        }

        private static string m_strFeedName = null;

        private static FileInfo m_pFeedFileInfo = null;

        private static string m_strFeedLock = ("lock");

        public static void Feed()
        {

            lock (ThreadUiController.m_strFeedLock)
            {
                try
                {
                    if (ThreadUiController.m_strFeedName == null)
                    {
                        ThreadUiController.m_strFeedName = Path.GetFileName(Application.ExecutablePath);
                        int lnDotIndex = ThreadUiController.m_strFeedName.LastIndexOf('.');
                        if (lnDotIndex > 0)
                        {
                            ThreadUiController.m_strFeedName = ThreadUiController.m_strFeedName.Substring(0, lnDotIndex);
                        }
                        ThreadUiController.m_strFeedName = ThreadUiController.m_strFeedName + ".txt";
                        ThreadUiController.m_pFeedFileInfo = new FileInfo(ThreadUiController.m_strFeedName);
                    }
                    if (ThreadUiController.m_pFeedFileInfo != null)
                    {
                        try
                        {
                            if (!ThreadUiController.m_pFeedFileInfo.Exists)
                            {
                                FileStream lpFs = ThreadUiController.m_pFeedFileInfo.Create();

                                if (lpFs != null)
                                {
                                    lpFs.Close();
                                }
                            }
                            ThreadUiController.m_pFeedFileInfo.LastWriteTime = DateTime.Now;
                        }
                        catch (System.Exception ex)
                        {
                            Debug.Assert(false);
                            Debug.WriteLine(ex.Message);
                            ThreadUiController.log(ex.Message, LOG_LEVEL.FATAL);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.Assert(false);
                    Debug.WriteLine(ex.Message);
                    ThreadUiController.log(ex.Message, LOG_LEVEL.FATAL);
                }
            }
        }

        private static System.IO.StreamWriter m_logger = null;
        private static String m_strLogFileName = null;
        private static int m_nLogFileSizeLimit = 3*1024*1024; //3M
        public static void log(string astrlog, LOG_LEVEL aenumLogLevel)
        {
            
            try
            {
                ThreadUiController.InitLog();
                Configuration lpConf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (lpConf == null)
                {
                    return;
                }
                ConfigurationSection lpSection = lpConf.Sections["log_level"];
                ConfigSectionData lpData = null;
                if (!(lpSection is ConfigSectionData))
                {
                    lpData = new ConfigSectionData();
                    lpData.log_level = LOG_LEVEL.FATAL;
                    lpConf.Sections.Add("log_level", lpData);
                    lpConf.Save();
                }
                else
                {
                    lpData = (ConfigSectionData)lpSection;
                }
                LOG_LEVEL lenumLogEvel = lpData.log_level;
                if (lenumLogEvel > aenumLogLevel)
                {
                    return;
                }
	            if (ThreadUiController.m_logger == null || astrlog == null)
	            {
	                Debug.Assert(false);
	                return;
	            }
	            if (ThreadUiController.IsLogExceedLength())
	            {
	                if (!ThreadUiController.InitLog() || ThreadUiController.m_logger == null)
	                {
	                    Debug.Assert(false);
	                    return;
	                }
	            }
	            string lstrlogTxtFormat = "{0} | [{1}] | {2} \r\n";
	            StringBuilder loSb = new StringBuilder();
	            DateTime dateTime = DateTime.Now;
	            string lstrDataLog = dateTime.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
	            loSb.AppendFormat(lstrlogTxtFormat,lstrDataLog,aenumLogLevel, astrlog);            
	            ThreadUiController.m_logger.Write(loSb.ToString());
	            ThreadUiController.m_logger.Flush();
            }
            catch (System.Exception ex)
            {
                Debug.Assert(false);
                Debug.WriteLine(ex.Message);
            }
        }

        private static Boolean IsLogExceedLength()
        {
            if (ThreadUiController.m_strLogFileName == null)
            {
                Debug.Assert(false);
                return false;
            }
            FileInfo loFileInf = null;
            try
            {
                loFileInf = new FileInfo(ThreadUiController.m_strLogFileName);
            }
            catch (System.Exception ex)
            {
                Debug.Assert(false);
                Debug.WriteLine(ex.Message);
                return false;
            }

            if (loFileInf.Length >= ThreadUiController.m_nLogFileSizeLimit)
            {
                return true;
            }
            return false;
        }
        private static string m_strLogLocker = "LogLocker";
        public static Boolean InitLog()
        {
            lock (m_strLogLocker)
            {
                if (ThreadUiController.m_logger != null)
                {
                    try
                    {
                        ThreadUiController.m_logger.Flush();
                        ThreadUiController.m_logger.Close();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }//if (ThreadUiController.m_logger != null) 
                if (!Directory.Exists("log"))
                {
                    try
                    {
                        Directory.CreateDirectory("log");
                    }
                    catch (System.Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        Debug.Assert(false);
                    }
                }
                DateTime dateTime = DateTime.Now;
                string lstrCurrentFileName =Process.GetCurrentProcess().MainModule.ModuleName;
                string lstrDataLogFileName = lstrCurrentFileName + "_" + dateTime.ToString("yyyy'-'MM'-'dd");
                string lstrFinalLogFileName = "log/" + lstrDataLogFileName + ".log";

                int lnLoopCount = 0;
                while (true)
                {
                    lnLoopCount++;
                    FileInfo loFileInf = null;
                    try
                    {
                        loFileInf = new FileInfo(lstrFinalLogFileName);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.Assert(false);
                        Debug.WriteLine(ex.Message);
                        return false;
                    }
                    if (loFileInf.Exists && loFileInf.Length >= ThreadUiController.m_nLogFileSizeLimit)
                    {
                        lstrFinalLogFileName = "log/" + lstrDataLogFileName + "-" + lnLoopCount + ".log";
                        continue;
                    }
                    else
                    {
                        try
                        {
                            ThreadUiController.m_logger = new StreamWriter(loFileInf.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
                            ThreadUiController.m_strLogFileName = lstrFinalLogFileName;
                        }
                        catch (System.Exception ex)
                        {
                            ThreadUiController.m_logger = null;
                            Debug.WriteLine(ex.Message);
                            return false;
                        }
                        return true;
                    }//else                     
                }//while (true)                 
            }//lock (m_strLogLocker)

        }

         public enum LOG_LEVEL
        {        
            DEBUG = 0x00000001,
            INFO    = 0x00000002,
            WARN  = 0x00000004,
            ERROR = 0x00000008,
            FATAL   = 0x00000010,
            NONE  = 0x00000011,
        }

        class ConfigSectionData : ConfigurationSection
        {
            [ConfigurationProperty("log_level")]
            public LOG_LEVEL log_level
            {
                get
                {
                    LOG_LEVEL lnLevel = (LOG_LEVEL)this["log_level"];

                    return lnLevel;
                }
                set { this["log_level"] = value; }
            }

        }
    }//end of ThreadUiController





  
}
