using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace ExifInfo.Helpers
{
    public class SystemHelper
    {
        public static Boolean Windows10Build10240 => ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 1, 0);
        public static Boolean Windows10Build10586 => ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 2, 0);
        public static Boolean Windows10Build14393 => ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3, 0);
        public static Boolean Windows10Build15063 => ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4, 0);
        public static Boolean Windows10Build16299 => ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5, 0);

        public static string GetAppVersion()
        {
            string temp = "2017.6.18";
            Windows.ApplicationModel.Package package = Windows.ApplicationModel.Package.Current;
            ushort u1 = package.Id.Version.Major;
            ushort u2 = package.Id.Version.Minor;
            ushort u3 = package.Id.Version.Build;
            ushort u4 = package.Id.Version.Revision;
            temp = $"{u1}.{u2}.{u3}";
            return temp;
        }

        public static string GetSysInfo()
        {
            Windows.System.Profile.AnalyticsVersionInfo analyticsVersion = Windows.System.Profile.AnalyticsInfo.VersionInfo;
            string strSysInfo = "";
            strSysInfo += "系统名称：" + analyticsVersion.DeviceFamily + "----------";
            ulong v = ulong.Parse(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = (v & 0x000000000000FFFFL);
            string temp = $"{v1}.{v2}.{v3}.{v4}";
            strSysInfo += "系统版本：" + temp + "----------";
            Windows.ApplicationModel.Package package = Windows.ApplicationModel.Package.Current;
            ushort u1 = package.Id.Version.Major;
            ushort u2 = package.Id.Version.Minor;
            ushort u3 = package.Id.Version.Build;
            ushort u4 = package.Id.Version.Revision;
            temp = $"{u1}.{u2}.{u3}.{u4}";
            strSysInfo += "软件包版本：" + temp + "----------";
            temp = package.Id.Architecture.ToString();
            strSysInfo += "系统架构：" + temp + "----------";
            temp = package.DisplayName;
            EasClientDeviceInformation eas = new EasClientDeviceInformation();
            temp = eas.SystemManufacturer;
            strSysInfo += "系统制造商：" + temp + "----------";
            return strSysInfo;
        }

        public static string GetDeviceInfo()
        {
            string strInfo = "设备名称：{0}----------设备标识符：{1}----------设备操作系统：{2}----------设备固件版本号：{3}----------设备硬件版本号：{4}----------设备制造商：{5}----------设备系统产品：{6}----------设备SKU：{7}---------- ";
            EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();
            string body = string.Format(strInfo, deviceInfo.FriendlyName,
                deviceInfo.Id,
                deviceInfo.OperatingSystem,
                deviceInfo.SystemFirmwareVersion,
                deviceInfo.SystemHardwareVersion,
                deviceInfo.SystemManufacturer,
                deviceInfo.SystemProductName,
                deviceInfo.SystemSku);
            return body;
        }
    }
}
