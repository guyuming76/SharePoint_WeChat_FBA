using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Sockets;
using System.Net;
using Sharepoint.FormsBasedAuthentication;
using System.Threading;

//http://lenashane.com/article/20151030-1030.html

/// <summary>
///WakeOnLine 的摘要说明
/// </summary>
public class WakeOnLine
{
    public WakeOnLine()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }
    private IPEndPoint point;
    private UdpClient client = new UdpClient();
    /**
     * 唤醒远程机器方法
     * @param
     * mac 要唤醒的机器的MAC
     * IP
     * port udp消息发送端口
     *
     * 摘要：唤醒方法为网卡提供的魔术封包功能，即以广播模式发送6个FF加上16遍目标MAC地址的字节数组
     **/
    public int wakeUp(string mac, int port, string ip)
    {
        byte[] magicBytes = getMagicPacket(mac);
        point = new IPEndPoint(GetOrCheckIP(ip), port);//广播模式:255.255.255.255
        try
        {
            return client.Send(magicBytes, magicBytes.Length, point);
        }
        catch (SocketException ex)
        {
            MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.Message);
            MyFBADiagnosticsService.Local.WriteTrace(0, MyFBADiagnosticsService.FBADiagnosticsCategory.Weixin, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, ex.StackTrace);
        }
        return -100;
    }

    /// <summary>
    /// 字符串转16进制字节数组
    /// </summary>
    /// <param name="hexString"></param>
    /// <returns></returns>
    private byte[] strToHexByte(string hexString)
    {
        hexString = hexString.Replace(" ", "");
        if ((hexString.Length % 2) != 0)
            hexString += " ";
        byte[] returnBytes = new byte[hexString.Length / 2];
        for (int i = 0; i < returnBytes.Length; i++)
            returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
        return returnBytes;
    }

    /// <summary>
    /// 拼装MAC魔术封包
    /// </summary>
    /// <param name="hexString"></param>
    /// <returns></returns>
    private byte[] getMagicPacket(string macString)
    {
        byte[] returnBytes = new byte[102];
        string commandString = "FFFFFFFFFFFF";
        for (int i = 0; i < 6; i++)
            returnBytes[i] = Convert.ToByte(commandString.Substring(i * 2, 2), 16);
        byte[] macBytes = strToHexByte(macString);
        for (int i = 6; i < 102; i++)
        {
            returnBytes[i] = macBytes[i % 6];
        }
        return returnBytes;
    }

    /// <summary>
    /// 域名解析
    /// </summary>
    /// <param name="hexString"></param>
    /// <returns></returns>
    public IPAddress GetOrCheckIP(string HostOrIP)
    {
        IPAddress IPA;
        if (!IPAddress.TryParse(HostOrIP, out IPA))
        {
            //IPHostEntry host = Dns.GetHostByName(HostOrIP);
            IPHostEntry host = Dns.GetHostEntry(HostOrIP);
            IPA = host.AddressList[0];
        }
        return IPA;
    }

    public void WOLMyDEV()
    {
        string Mac = "5404A69CD977"; //MAC地址，注意此处没有中间的“-”
        string Ip = "192.168.100.98"; //IP或域名
        //string SubnetMask = "255.255.255.255"; //默认
        int Port = 7; //默认端口或计算机在路由器中的映射端口
        //WakeOnLine WOL = new WakeOnLine();
        int i1 = wakeUp(Mac, Port, "192.168.100.255");
        int i2 = wakeUp(Mac, Port, "255.255.255.255");
        Thread.Sleep(500);
        int i3 = wakeUp(Mac, Port, "192.168.100.255");
        int i4 = wakeUp(Mac, Port, "255.255.255.255");
        Thread.Sleep(500);
        int i5 = wakeUp(Mac, Port, "192.168.100.255");
        int i6 = wakeUp(Mac, Port, "255.255.255.255");
    }

}