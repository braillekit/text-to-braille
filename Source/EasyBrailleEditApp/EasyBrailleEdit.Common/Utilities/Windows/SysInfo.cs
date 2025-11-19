using System.Net;

namespace EasyBrailleEdit.Common.Utilities.Windows;

/// <summary>
/// 系統資訊。
/// </summary>
public sealed class SysInfo
{
    private SysInfo()
    {
    }

    /// <summary>
    /// 傳回本機的所有 IP 位址。
    /// </summary>
    /// <returns></returns>
    public static string[] GetIPAddresses()
    {
        string hostName = Dns.GetHostName();
        IPHostEntry entry = Dns.GetHostEntry(hostName);
        string[] ipAddresses = new string[entry.AddressList.Length];

        for (int i = 0; i < entry.AddressList.Length; i++)
        {
            ipAddresses[i] = entry.AddressList[i].ToString();
        }
        return ipAddresses;
    }

    /// <summary>
    /// 傳回目前使用的 IP 位址。
    /// </summary>
    /// <returns></returns>
    public static string GetIPAddress()
    {
        string[] ipAddresses = GetIPAddresses();
        if (ipAddresses.Length > 0)
            return ipAddresses[0];
        return "";
    }

    /// <summary>
    /// 取得 DNS host name（跟 System.Environment.MachineName 相同作用?）。
    /// </summary>
    /// <returns></returns>
    public static string GetDnsHostName()
    {
        return Dns.GetHostName();
/*
        string result = "";
        ManagementObjectSearcher mngSearcher;
        ManagementObjectCollection mngObjects;
        PropertyData propData;

        string qry = "SELECT * from Win32_NetworkAdapterConfiguration WHERE IPEnabled=true";
        mngSearcher = new ManagementObjectSearcher(qry);
        mngObjects = mngSearcher.Get();

        foreach (ManagementObject mngObj in mngObjects)
        {

            propData = mngObj.Properties["DNSHostName"];
            if (propData != null && propData.IsLocal)
            {
                result = propData.Value.ToString();
                break;
            }
        }
        return result;
*/
    }

    /// <summary>
    /// 從 IP address 反查 DNS 主機名稱。
    /// </summary>
    /// <param name="ipAddr"></param>
    /// <returns></returns>
    public static string GetDnsHostName(string ipAddr)
    {
        IPHostEntry entry = Dns.GetHostEntry(ipAddr);            
        return entry.HostName;
    }

    /// <summary>
    /// 檢查是否有連接網路的能力。
    /// </summary>
    /// <returns></returns>
    public static bool IsNetworkConnected()
    {
        try 
        {
            System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry("www.google.com");
            return true;
        }
        catch 
        {
            return false;
        }
    }
}