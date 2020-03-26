namespace Camera_NET
{
    using DirectShowLib;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class CameraChoice : IDisposable
    {
        protected List<DsDevice> m_pCapDevices = new List<DsDevice>();

        public void Dispose()
        {
            foreach (DsDevice device in this.m_pCapDevices)
            {
                device.Dispose();
            }
            this.m_pCapDevices.Clear();
        }

        public DsDevice GetCameraByName(string name)
        {
            return this.GetCameraByName(name, 0);
        }

        public DsDevice GetCameraByName(string camera_name, int index_in_same_names)
        {
            if (string.IsNullOrEmpty(camera_name) || (index_in_same_names < 0))
            {
                return null;
            }
            int num = 0;
            DsDevice device = null;
            foreach (DsDevice device2 in this.m_pCapDevices)
            {
                if (string.Compare(device2.Name, camera_name, true) == 0)
                {
                    num++;
                    if (device == null)
                    {
                        device = device2;
                    }
                }
                if ((num - 1) == index_in_same_names)
                {
                    return device2;
                }
            }
            return device;
        }

        public int GetCameraIndexInDevices(DsDevice cam)
        {
            int num3;
            try
            {
                this.UpdateDeviceList();
                int num = -1;
                if (cam == null)
                {
                    return -1;
                }
                for (int i = 0; i < this.m_pCapDevices.Count; i++)
                {
                    if (string.Compare(cam.DevicePath, this.m_pCapDevices[i].DevicePath) == 0)
                    {
                        num = i;
                        break;
                    }
                }
                num3 = num;
            }
            catch
            {
                throw;
            }
            return num3;
        }

        public bool GetNameByCamera(DsDevice camera, out string camera_name, out int index_in_same_names)
        {
            this.UpdateDeviceList();
            int num = 0;
            foreach (DsDevice device in this.m_pCapDevices)
            {
                if (string.Compare(device.DevicePath, camera.DevicePath) == 0)
                {
                    index_in_same_names = num;
                    camera_name = device.Name;
                    return true;
                }
                if (string.Compare(device.Name, camera.Name, true) == 0)
                {
                    num++;
                }
            }
            camera_name = string.Empty;
            index_in_same_names = 0;
            return false;
        }

        public void UpdateDeviceList()
        {
            this.m_pCapDevices = new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));
        }

        public List<DsDevice> Devices
        {
            get
            {
                return this.m_pCapDevices;
            }
        }
    }
}

