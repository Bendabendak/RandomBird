using Mirror;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Game
{
    public class HostMenuManager : SadJam.Component
    {
        public const float RETRY_CONNECTION_INTERVAL = 1;

        protected override void StartOnce()
        {
            base.StartOnce();

            StartCoroutine(RetryConnectionCoroutine());
        }

        private void Host()
        {
            if (Transport.active is PortTransport portTransport)
            {
                int port = 0;
                Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                try
                {
                    IPEndPoint localEP = new(IPAddress.Any, 0);
                    socket.Bind(localEP);
                    localEP = (IPEndPoint)socket.LocalEndPoint;
                    port = localEP.Port;
                }
                finally
                {
                    socket.Close();
                }

                if (port <= IPEndPoint.MinPort)
                {
                    Debug.LogWarning("Open port not found!", gameObject);
                }
                else
                {
                    portTransport.Port = (ushort)port;

                    Mirror.NetworkManager.singleton.StartHost();
                }
            }
            else
            {
                Mirror.NetworkManager.singleton.StartHost();
            }
        }

        private static WaitForSeconds _retryConnectionInterval = new(RETRY_CONNECTION_INTERVAL);
        private IEnumerator RetryConnectionCoroutine()
        {
            while (true)
            {
                if (NetworkServer.active || NetworkClient.active) 
                {
                    yield return null;
                    continue;
                }

                Host();

                yield return _retryConnectionInterval;
            }
        }
    }
}

