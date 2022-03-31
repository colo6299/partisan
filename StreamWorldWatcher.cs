using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

public class StreamWorldWatcher : MonoBehaviour
{

    public PlayerPartisan player; //set to player in editor...?
    //private string serverIP = "192.168.0.85";
    private string serverIP = "52.53.239.39";
    private int listenOnPort = 12112;
    private int sendToPort = 12112;//12322;
    private int tcpPort = 13113;
    public static string uID = "";

    public bool running = false;
    public bool start = false;
    private bool autostart = false;
    private bool tcpConnected = false;
    private bool udpConnected = false;

    public GameObject prefabPuppet;

    private Vector3 playerPos;
    private float playerAzimuth;

    private UdpClient udpClient;
    private TcpClient tcpClient;
    private NetworkStream tcpStream;
    private static double netTimeOffset;

    public Dictionary<string, PuppetPartisan> puppets;

    public Transform puppetHolder;

    private string stagedPuppetUID;
    private string stagedMessage;

    private static Queue<string> tcpCommands;

    private static float realTime = 0;

    private float lastTcpPacket = 2;
    private float tcpTimeout = 10;

    private float lastUdpPacket = 2;
    private float udpTimeout = 20;
    public bool debugSetConnected = false;


    void OnApplicationQuit()
    {

    }
    private void Awake()
    {
        Application.runInBackground = true;
        tcpCommands = new Queue<string>();
    }
    private void Start()
    {
        uID = Guid.NewGuid().ToString();
        IpConfiguration();
    }
    private void Update()
    {
        realTime = Time.realtimeSinceStartup;
        if (start) { start = false; running = true; StreamsInitialze(); }
        if ((Time.time > 2) & (autostart == false) & !debugSetConnected) { start = true; autostart = true; }
        if ((udpConnected == false) & (tcpConnected == true))
        {
            
        }
        if (udpConnected & tcpConnected)
        {
            player.SetConnect(true);
        }
        if ((realTime - lastTcpPacket > tcpTimeout) & tcpConnected) { Debug.LogError("TCP timeout."); DisconnectAll(); }
        if ((realTime - lastUdpPacket > udpTimeout) & udpConnected) { Debug.LogError("UDP timeout."); DisconnectAll(); }
        if (debugSetConnected)
        {
            player.SetConnect(true);
        }
        if (Input.GetButton("Disconnect"))
        {
            DisconnectAll();
            return;
        }
        Destager();
    }

    public void DisconnectAll()
    {
        tcpConnected = false;
        udpConnected = false;
        tcpClient.GetStream().Flush();
        tcpClient.GetStream().Close();
        tcpClient.Close();
        udpClient.Close();
        player.SetConnect(false);
    }

    private void IpConfiguration()
    {/*
        try
        {
            Debug.Log("Old ip: " + serverIP + serverIP.Length);
            string path = Application.dataPath + "/network.txt";
            //Read the text from directly from the test.txt file
            StreamReader reader = new StreamReader(path);
            string[] netConfig = reader.ReadToEnd().Split('\n');
            serverIP = netConfig[0].Trim();
            //4listenOnPort = int.Parse(netConfig[1].Trim());
            //sendToPort = int.Parse(netConfig[2].Trim());
            Debug.Log("New ip: " + serverIP + serverIP.Length);
            Debug.Log("Network config files loaded.");
        }
        catch
        {
            Debug.Log("Default network config loaded.");
            serverIP = "192.168.0.223";
            listenOnPort = 12321;
            sendToPort = 12321;//12322;
}*/
    }

    private void Destager()
    {

        if (stagedPuppetUID == null)
        {
            //do nothing
        }
        else if (puppets.ContainsKey(stagedPuppetUID) == false)
        {
            GameObject puppet = Instantiate(prefabPuppet, puppetHolder);
            puppets.Add(stagedPuppetUID, puppet.GetComponent<PuppetPartisan>());
            puppets[stagedPuppetUID].PuppetUdpStream(stagedMessage);
            Debug.Log("New partisan puppet spawned.");
        }
    }

    public static double GetNetOffset()
    {
        return netTimeOffset;
    }

    public static double GetServerTime()
    {
        return netTimeOffset + realTime;
    }

    public static void AddTcpCommand(string command)
    {
        tcpCommands.Enqueue(command);
    }

    private void FixedUpdate()
    {
        playerPos = player.transform.position;
        playerAzimuth = player.transform.rotation.eulerAngles.y;
    }


    void StreamsInitialze()
    {
        puppets = new Dictionary<string, PuppetPartisan>();
        UDPInitialize();
        TCPInitialize();
    }

    void TCPInitialize()
    {
        try
        {
            Thread TcpSendRecieveThread = new Thread(() => TcpSendListenLoop());
            TcpSendRecieveThread.Start();

        }
        catch
        {
            Debug.LogError("yowch but tcp");
        }
    }

    private void TcpSendListenLoop()
    {


        IPAddress remote = IPAddress.Parse(serverIP);

        //Debug.Log("TCP attempting to connect...");
        TcpClient playerClient = new TcpClient();
        playerClient.Connect(new IPEndPoint(remote, tcpPort));
        tcpClient = playerClient;
        //Debug.Log("TCP client created...");
        NetworkStream stream = tcpClient.GetStream();
        tcpStream = stream;

        //Debug.Log("Connection established: " + playerClient.GetHashCode());
        string handshakeMessage = uID + " Sync" + " 0000";


        byte[] data = Encoding.UTF8.GetBytes(NetPrefix(handshakeMessage));
        stream.Write(data, 0, data.Length);
        //Debug.Log("TCP data successfully written.");

        data = new byte[1024];
        string[] responses;
        string inboundCommand;
        bool initConnect = true;

        while (running)
        {
            lastTcpPacket = realTime;
            //Debug.Log("TCP data read.");
            responses = RecieveNetString(stream).Trim().Split(',');
            if (initConnect) { tcpConnected = true; initConnect = false; }
            foreach (string response in responses)
            {
                string[] decoded_response = response.Trim().Split(' ');
                if (decoded_response.Length < 2)
                {
                    //Debug.Log("Empty command filtered");
                    continue;
                }
                inboundCommand = decoded_response[1];
                Debug.Log("Command is: " + inboundCommand);
                if (inboundCommand == "collisions") { Debug.Log("collision command recieved"); }
                if (inboundCommand == "kills") { Debug.Log("kills command recieved"); }
                if (inboundCommand == "fireEvent") { Debug.Log("fireEvents command recieved"); }
                if (inboundCommand == "selfMissileAck") { Debug.Log("selfMissileAck command recieved"); }
                if (inboundCommand == "serverTime") { TcpTimeManager(decoded_response); }
                if (inboundCommand == "missileData") { TcpMissileManager(decoded_response); }
                if (inboundCommand == "serverMissileStates") { TcpMiscManager(decoded_response); }
                if (inboundCommand == "killfeed") { TcpKillfeedManager(decoded_response); }
                if (inboundCommand == "projectileNak") { TcpProjectileNakManager(decoded_response); }
            }
            if (tcpCommands.Count != 0)
            {
                byte[] dqCommandData = Encoding.UTF8.GetBytes(NetPrefix(tcpCommands.Dequeue()));
                stream.Write(dqCommandData, 0, dqCommandData.Length);
            }
            else
            {

                byte[] dqCommandData = Encoding.UTF8.GetBytes(NetPrefix(uID + " Pass 0"));
                stream.Write(dqCommandData, 0, dqCommandData.Length);
            }
        }
    }

    private string RecieveNetString(NetworkStream stream)
    {
        string netString = "";
        bool msg_complete = false;
        byte[] data = new byte[1024];
        int bytes;
        while (msg_complete != true)
        {
            bytes = stream.Read(data, 0, data.Length);
            netString += Encoding.UTF8.GetString(data, 0, bytes);
            msg_complete = PrefixCheck(netString);
        }
        return PrefixTrim(netString);
    }

    private string NetPrefix(string netTask)
    {
        //8 zeroes
        netTask = (netTask.Length + 100000000).ToString() + netTask;
        return netTask;
    }

    private bool PrefixCheck(string netString)
    {
        string prefix = netString.Substring(1, 8);
        int targetLength = int.Parse(prefix) + 9;
        if (netString.Length >= targetLength) { return true; }
        else { return false; }
    }

    private string PrefixTrim(string netString)
    {
        return netString.Substring(9, netString.Length - 9);
    }

    // 0: UID
    // 1: CMD
    // 2: DATA

    private void TcpTimeManager(string[] timeData)
    {
        netTimeOffset = double.Parse(timeData[2]) - realTime;

        Debug.Log("Set time to: " + timeData[2]);
    }

    private void TcpProjectileNakManager(string[] nakProjectiles)
    {

    }

    private void TcpKillfeedManager(string[] killfeed)
    {
        //   UID command ;killerID:missileID:KilledID;
        string[] killerMissiles = killfeed[2].Trim().Split(';');
        foreach (string missileKills in killerMissiles)
        {
            if(missileKills.Length < 2) { continue; }
            string[] killsData = missileKills.Split(':');
            string killerPlayerUID = killsData[0]; //player who got the kills below
            string killerMissileUID = killsData[1]; //missile above player shot to kill players below
            for (int i=2; i<killsData.Length; i++)  //players killed by above
            {
                /////
            }
            MissileDirector.director.DetonateMissile(killerMissileUID);
        }
    }

    private void TcpCollisionManager(string[] collisions)
    {

    }

    private void TcpKillsManager(string[] kills)
    {

    }

    private void TcpMissileManager(string[] missileReport)
    {
        string reportString = "";
        int i = 0;
        foreach(string datum in missileReport)
        {
            if(i < 2) { i++; continue; }
            reportString += datum + " ";
        }
        player.fireControl.QueueMissile(reportString);
    }

    private void TcpMiscManager(string[] data)
    {
        Vector3[] vectors = new Vector3[(data.Length - 2) / 2];
        for (int i=2; i<data.Length-2; i+=2)
        {
            float x = float.Parse(data[i]);
            float z = float.Parse(data[i + 1]);
            vectors[i / 2] = new Vector3(x, 0, z);
        }
        ServerDataViewer.FeedVectors(vectors);
    }

    void SendMissile(TcpClient client)
    {

    }

    void UDPInitialize()
    {
        

        try
        {
            UdpClient playerClient = new UdpClient(listenOnPort);
            udpClient = playerClient;

            Debug.Log("Connection established: " + playerClient.GetHashCode());

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            Thread UdpRecieveThread = new Thread(() => UdpListenLoop(playerClient, sender));
            UdpRecieveThread.Start();

            Thread UdpSendThread = new Thread(() => UdpSendLoop(playerClient));
            UdpSendThread.Start();
        }
        catch
        {
            Debug.LogError("yowch");
        }

    }
    private void UdpSendLoop(UdpClient playerClient)
    {
        Debug.Log("UDP player outbound running...");
        bool initialConnect = true;
        
        while (running)
        {
            //8 for player uID
            //8 for player x
            //8 for player z
            //8 for player azimuth
            //8 bytes for ... team... yesss....
            //
            //...maybe
            
            string dataString = string.Format("{0} {1} {2} {3} {4}",
                uID, 
                playerPos.x, 
                playerPos.z, 
                playerAzimuth,
                player.fireControl.team
                );
            //Debug.Log(dataString);
            //dataString = "test";
            byte[] byteMessage = Encoding.UTF8.GetBytes(dataString);
            playerClient.Send(byteMessage, Encoding.UTF8.GetByteCount(dataString), serverIP, sendToPort);
            if (initialConnect) { udpConnected = true; initialConnect = false; }
            Thread.Sleep(10);
        }
    }

    public void UdpListenLoop(UdpClient playerClient, IPEndPoint sender)
    {
        Debug.Log("UDP world listener started...");
        while (running)
        {
            lastUdpPacket = realTime;
            byte[] recievedBytes = playerClient.Receive(ref sender);
            string decodedMessage = Encoding.UTF8.GetString(recievedBytes);
            string puppetUID = decodedMessage.Split(' ')[0];

            if (puppetUID == uID)
            {
                Debug.Log("Self-data ignored.");
            }
            else if (puppets.ContainsKey(puppetUID))
            {
                Debug.Log("Puppet Updated: " + puppetUID);
                puppets[puppetUID].PuppetUdpStream(decodedMessage);
            }
            else
            {
                Debug.Log("New partisan puppet spawning, UID: " + puppetUID);

                stagedPuppetUID = puppetUID;
                stagedMessage = decodedMessage;
            }
        }
    }
}
