using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class NetworkManager : Singleton<NetworkManager>
{
    public int Port = 9999;
    public bool IsExit = false;

    private string _serverIP = "0.tcp.ap.ngrok.io";
    private bool _isReceiving = false;
    private bool _isConnect = false;
    private Socket _sender;
    private Thread _receiveThread;
    private Queue<Action> _mainThreadActions = new();

    private void Update()
    {
        lock (_mainThreadActions)
        {
            while (_mainThreadActions.Count > 0)
            {
                _mainThreadActions.Dequeue().Invoke();
            }
        }
    }

    IEnumerator CheckConnect()
    {
        yield return new WaitForSeconds(0.5f);

        if (!_isConnect)
        {
            lock (_mainThreadActions)
            {
                _mainThreadActions.Enqueue(() =>
                {
                    UIManager.Instance.NotifyScreenActive(true);
                });
            }
        }
    }

    public void ConnectToServer()
    {
        byte[] bytes = new byte[1024];
        IPHostEntry host = Dns.GetHostEntry(_serverIP);
        IPAddress ipAddress = host.AddressList[0];

        try
        {
            //IPAddress.Parse("127.0.0.1")
            IPEndPoint remoteEP = new(ipAddress, Port);
            _sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _sender.Connect(remoteEP);
                Debug.Log("Socket connected to " + _sender.RemoteEndPoint.ToString());

                _isReceiving = true;
                _receiveThread = new Thread(new ThreadStart(ReceiveMessages))
                {
                    IsBackground = true
                };
                _receiveThread.Start();

                StartCoroutine(CheckConnect());
            }
            catch (ArgumentNullException ane)
            {
                Debug.Log("ArgumentNullException : " + ane.ToString());
            }
            catch (SocketException se)
            {
                Debug.Log("SocketException : " + se.ToString());
                UIManager.Instance.NotifyScreenActive(true);
            }
            catch (Exception e)
            {
                Debug.Log("Unexpected exception : " + e.ToString());
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private bool IsSocketConnected(Socket socket)
    {
        try
        {
            return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
        }
        catch (SocketException) { return false; }
    }

    public void SendMessageToServer(string message)
    {
        try
        {
            if (_sender == null || !_sender.Connected || !IsSocketConnected(_sender))
            {
                Debug.Log("Socket is not connected.");
                return;
            }

            byte[] msg = Encoding.ASCII.GetBytes(message);
            int bytesSent = _sender.Send(msg);
        }
        catch (ArgumentNullException ane)
        {
            Debug.Log("ArgumentNullException : " + ane.ToString());
        }
        catch (SocketException se)
        {
            Debug.Log("SocketException : " + se.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Unexpected exception : " + e.ToString());
        }
    }

    private void ReceiveMessages()
    {
        try
        {
            if (_sender == null || !_sender.Connected || !IsSocketConnected(_sender))
            {
                Debug.Log("Socket is not connected.");
                return;
            }

            while (_isReceiving)
            {
                if (!_sender.Connected || !IsSocketConnected(_sender))
                {
                    Debug.Log("Socket Disconnected.");
                    _isReceiving = false;
                    break;
                }

                byte[] bytes = new byte[1024];
                int bytesRec = _sender.Receive(bytes);
                if (bytesRec > 0)
                {
                    string str = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    List<string> listMessage = SplitMessage(str);

                    foreach (var message in listMessage)
                    {
                        if (!string.IsNullOrEmpty(message))
                        {
                            if (message == "x" || message == "o")
                            {
                                lock (_mainThreadActions)
                                {
                                    _mainThreadActions.Enqueue(() =>
                                    {
                                        ClientControl.Instance.Symbol = message;
                                    });
                                }
                            }
                            else if (message.Equals("Connected to server"))
                            {
                                lock (_mainThreadActions)
                                {
                                    _mainThreadActions.Enqueue(() =>
                                    {
                                        _isConnect = true;
                                        UIManager.Instance.NotifyScreenActive(false);
                                    });
                                }
                            }
                            else if (message.Contains("SPAWN"))
                            {
                                (int row, int column) = GetIndex(message);
                                lock (_mainThreadActions)
                                {
                                    _mainThreadActions.Enqueue(() =>
                                    {
                                        ClientControl.Instance.Move(row, column);
                                    });
                                }
                            }
                            else if (message.Equals("OPPONENT DISCONNECTED"))
                            {
                                lock (_mainThreadActions)
                                {
                                    _mainThreadActions.Enqueue(() =>
                                    {
                                        ConnectToServer();

                                        GameManager.Instance.ResetPlayer();
                                        GameModeControl.Instance.PlayOnline();
                                        UIManager.Instance.LoadingScreenActive(true);
                                    });
                                }
                            }
                            else if (message.Equals("EXIT"))
                            {
                                lock (_mainThreadActions)
                                {
                                    _mainThreadActions.Enqueue(() =>
                                    {
                                        IsExit = true;
                                    });
                                }
                            }
                            else if (message.Equals("REPLAY REQUEST")
                                    || message.Equals("WAITING"))
                            {
                                lock (_mainThreadActions)
                                {
                                    _mainThreadActions.Enqueue(() =>
                                    {
                                        GameModeControl.Instance.PlayOnline();
                                        UIManager.Instance.LoadingScreenActive(true);
                                    });
                                }
                            }
                            else if (message.Equals("GAME STARTED")
                                    || message.Equals("GAME RESTARTED"))
                            {
                                lock (_mainThreadActions)
                                {
                                    _mainThreadActions.Enqueue(() =>
                                    {
                                        if (!message.Equals("GAME RESTARTED"))
                                        {
                                            IsExit = false;

                                            GameManager.Instance.ResetPlayer();
                                            GameModeControl.Instance.PlayOnline();
                                        }

                                        UIManager.Instance.LoadingScreenActive(false);
                                    });
                                }
                            }

                            Debug.Log("Message received: " + message);
                        }
                    }
                }
                else
                {
                    _isReceiving = false;
                }
            }
        }
        catch (SocketException)
        {
            return;
        }
        catch (Exception e)
        {
            Debug.Log("Unexpected exception : " + e.ToString());
        }
    }

    public List<string> SplitMessage(string input)
    {
        List<string> result = new();
        Regex regex = new(@"\[(.*?)\]");
        MatchCollection matches = regex.Matches(input);

        foreach (Match match in matches)
        {
            result.Add(match.Groups[1].Value);
        }

        return result;
    }

    public (int row, int column) GetIndex(string input)
    {
        Regex regex = new(@"SPAWN\s*\((\d+),\s*(\d+)\)");
        Match match = regex.Match(input);

        if (match.Success)
        {
            int row = int.Parse(match.Groups[1].Value);
            int column = int.Parse(match.Groups[2].Value);

            return (row, column);
        }

        throw new ArgumentException("Input string is not in the correct format.");
    }

    private void OnApplicationQuit()
    {
        ClientControl.Instance.CloseApp();

        DisconnectFromSever();

        lock (_mainThreadActions)
        {
            _mainThreadActions.Enqueue(() =>
            {
                UIManager.Instance.NotifyScreenActive(true);
            });
        }
    }

    public void DisconnectFromSever()
    {
        _isReceiving = false;
        if (_sender != null)
        {
            try
            {
                _sender.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException se)
            {
                Debug.Log("SocketException during shutdown: " + se.ToString());
            }
            _sender.Close();
            _sender = null;
            Debug.Log("Disconnected From Server.");
        }

        if (_receiveThread != null && _receiveThread.IsAlive)
        {
            _receiveThread.Join();
        }
    }
}