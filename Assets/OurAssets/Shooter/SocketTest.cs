using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

#if NETFX_CORE
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Foundation;
using Windows.Storage.Streams;
using System.Threading.Tasks;
#endif

public class SocketTest : MonoBehaviour {


    public void Send()
    {
        StartStreamRPC("192.168.120.22", "27772");
    }

#if NETFX_CORE
    private DatagramSocket socket;
#endif
    private int _nextAction = -1;

    public void StartStreamRPC(string ip, string port)
    {
        //RuntimeExceptionsHandler.Instance.Log("start on " + port + "/" + ip);
        _nextAction = 10;
#if NETFX_CORE

        OpenUDPSocket(ip, port);
#endif
    }

    public void StopStreamRPC(string ip, string port)
    {
        _nextAction = 20;
#if NETFX_CORE
        OpenUDPSocket(ip, port);
#endif
    }

#if NETFX_CORE
    private async void OpenUDPSocket(string ip, string port)
    {
        socket = new DatagramSocket();
        IOutputStream outStream = await socket.GetOutputStreamAsync(new HostName(ip), port);
        StreamWriter writer = new StreamWriter(outStream.AsStreamForWrite());
        await writer.WriteLineAsync(_nextAction.ToString());
        await writer.FlushAsync();

        writer.Dispose();
        socket.Dispose();
        socket = null;
        _nextAction = -1;
    }
#endif

    // 10 - start streaming
    // 20 - stop streaming
#if NETFX_CORE
    private async void OnConnected(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
    {
        if(asyncStatus == AsyncStatus.Completed)
        {
            if (_nextAction == -1)
                return;

            using (DataWriter networkWriter = new DataWriter(socket.OutputStream))
            {
                networkWriter.WriteInt32(_nextAction);
                await networkWriter.StoreAsync();
                await networkWriter.FlushAsync();
            }

            socket.Dispose();
            socket = null;
            _nextAction = -1;
        }
        else
        {
            Debug.Log("Connect Error");
        }
    }
#endif
}
