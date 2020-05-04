using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnityEngine;

/// <summary>
/// This class is a data server for data messages being sent from remote clients.
/// Messages are separated into channels by their message type label.
/// More than one client can send data to the same channel simply by using the same channel id in the message.
/// It allows DataSubscriber objects to register to receive messages on any channel.
/// </summary>
public class DataServer : MonoBehaviour
{
    /// <summary>
    /// Objects that wish to subscribe to data channels must implement the DataSubscriber interface.
    /// </summary>
    public interface DataSubscriber
    {
        void OnReceiveMessage(float timestamp, string message);
    }

    /// <value> The dataserver will listen on the following ports for data sending clients to connect. </value>
    /// <remarks> The value of this should be set in the Unity Editor. You need one port per data-sending client. </remarks>
    public int[] listenPorts;

    /// <value> Whether to print debug statements. </value>
    public bool verbose = false;

    /// <value> A debug variable for how many messages the server has received across all channels. </value>
    public int line_count;

    /// <value> A mapping between the port number and the TcpListener object dedicated to that port. </value>
    private Dictionary<int, TcpListener> listeners = new Dictionary<int, TcpListener>();
    /// <value>
    /// A list of tuples, each representing the state of a stream with one of the data-sending clients.
    /// <list type="bullet">
    /// <item>The TcpClient is the object representing the client that connected to one of the ports.</item>
    /// <item>The NetworkStream is the receive stream opened up upon connection.</item>
    /// <item>The byte[] is the array representing the chunk of memory used as a buffer to store the received messages for client-stream.</item>
    /// <item>The ConcurrentQueue<string> is used to store the list of complete, but unparsed messages received.</item>
    /// <item>The StringBuilder is used to store the current, partially-received messages.</item>
    /// </list>
    /// </value>
    private List<Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder>> clientStreams =
        new List<Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder>>();

    /// <value>
    /// This is a list of parsed messages not yet broadcasted to the subscribers.
    /// <list type="bullet">
    /// <item>The first string in the tuple is the id of the channel.</item>
    /// <item>The float is the original timestamp of the data. It may be used to do real-time playback.</item>
    /// <item>The second string in the tuple is the actual unparsed contents of the message.</item>
    /// </list>
    /// </value>
    private ConcurrentQueue<Tuple<string, float, string>> parsedMessages = new ConcurrentQueue<Tuple<string, float, string>>();
    /// <value>
    /// This thread is responsible for taking received, unparsed messages and extracting 
    /// the channel id, timestamp, and raw contents and place them into the parsedMessages queue.
    /// </value>
    private Thread parseThread;

    /// <value> A mapping from channel to the list of DataSubscribers subscribed to the channel. </value>
    private Dictionary<string, List<DataSubscriber>> subscribers = new Dictionary<string, List<DataSubscriber>>();
    /// <value>
    /// The thread responsible for taking messages from the parsedMessages queue 
    /// and broadcasting them to all DataSubscribers to that channel.
    /// </value>
    private Thread subscriberThread;

    // Start is called before the first frame update
    void Start()
    {
        // Start message parsing thread in background.
        parseThread = new Thread(new ThreadStart(ProcessMessages));
        parseThread.IsBackground = true;
        parseThread.Start();

        // Start thread to broadcast message to subscribers in the background.
        subscriberThread = new Thread(new ThreadStart(DelegateMessages));
        subscriberThread.IsBackground = true;
        subscriberThread.Start();

        // Start listening for data-sending clients on specified ports.
        foreach (int port in listenPorts)
        {
            ListenOnPort(port);
        }
    }

    /// <summary>
    /// Register a DataSubscriber object to receive message to a specific channel.
    /// </summary>
    /// <param name="channel_id">The id of the channel to subscribe to.</param>
    /// <param name="sb">The DataSubscriber object to receive the messages.</param>
    public void RegisterDataSubscriber(string channel_id, DataSubscriber sb)
    {
        List<DataSubscriber> currSubscribers;
        if (!subscribers.TryGetValue(channel_id, out currSubscribers))
        {
            currSubscribers = new List<DataSubscriber>();
            subscribers.Add(channel_id, currSubscribers);
        }
        if (!currSubscribers.Contains(sb))
        {
            if (verbose)
            {
                Debug.Log("Registering listener for: " + channel_id);
            }
            currSubscribers.Add(sb);
        }
    }

    // Called when the application quits.
    private void OnApplicationQuit()
    {
        // Stop listening for new connections on those ports.
        foreach (KeyValuePair<int, TcpListener> curr in listeners)
        {
            curr.Value.Stop();
        }
        // Shutdown any existing client streams.
        lock (clientStreams)
        {
            foreach (Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder> curr in clientStreams)
            {
                curr.Item1.Close();
            }
            clientStreams = null;
        }
        // Stop all threads.
        parseThread.Abort();
        subscriberThread.Abort();
    }

    /// <summary>
    /// Helper function to open listen on a port and assign the callback function when a client connects.
    /// </summary>
    /// <param name="port">The port number to listen on.</param>
    private void ListenOnPort(int port)
    {
        if (port <= 0)
        {
            return;
        }
        TcpListener tcpListener = new TcpListener(IPAddress.Any, port);
        listeners.Add(port, tcpListener);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(ClientConnectCallback), tcpListener);
        if (verbose)
        {
            Debug.Log("Listening on " + port);
        }
    }

    /// <summary>
    /// The callback function for when a client establishes connection with the server.
    /// </summary>
    /// <param name="ar">The TcpListener that was listening on the port the client connected to.</param>
    private void ClientConnectCallback(IAsyncResult ar)
    {
        TcpListener tcpListener = (TcpListener)ar.AsyncState;
        // TODO [1]: Is it really necessary to restart the tcpListener every time a client connects?
        TcpClient client = tcpListener.EndAcceptTcpClient(ar);
        if (verbose)
        {
            Debug.Log("Client connected");
        }

        NetworkStream stream = client.GetStream();
        ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
        StringBuilder leftover = new StringBuilder();
        byte[] bytes = new byte[4096];
        Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder> state = 
            new Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder>(client, stream, bytes, messageQueue, leftover);
        lock (clientStreams)
        {
            clientStreams.Add(state);
        }

        // Start reading data arriving on this network stream.
        stream.BeginRead(bytes, 0, bytes.Length, new AsyncCallback(ReadMessages), state);

        // TODO [1]: Is it really necessary to restart the tcpListener every time a client connects?
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(ClientConnectCallback), tcpListener);
    }

    /// <summary>
    /// The callback function for when a network stream receives new data.
    /// </summary>
    /// <param name="ar">The Tuple state object associated with this stream.</param>
    private void ReadMessages(IAsyncResult ar)
    {
        Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder> state = 
            (Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder>)ar.AsyncState;
        NetworkStream stream = state.Item2;
        int length = stream.EndRead(ar);
        byte[] bytes = state.Item3;
        // Read incomming stream into byte arrary.				
        if (length != 0)
        {
            ConcurrentQueue<string> messageQueue = state.Item4;
            // Convert byte array to string message.
            string clientMessage = Encoding.ASCII.GetString(bytes, 0, length);
            messageQueue.Enqueue(clientMessage);
            length = stream.Read(bytes, 0, bytes.Length);
        }
        // TODO [2]: Is it really necessary to restart the a network stream's read fucntion each time?
        stream.BeginRead(bytes, 0, bytes.Length, new AsyncCallback(ReadMessages), state);
    }

    /// <value> The delimiter used to separate messsages in the network stream.</value>
    private static char[] charSeparators = new char[] { '\n' };

    /// <summary>
    /// Takes the raw data on the network stream and separates them into individual messages.
    /// Also parses the separated messages and enqueues them on the parsedMessages for broadcasting to subscribers.
    /// This is run on the parseThread thread.
    /// </summary>
    private void ProcessMessages()
    {
        string fragment;
        while (true)
        {
            int count;
            // Need locking in case new clients are accepted while the messages are being processed.
            lock (clientStreams)
            {
                count = clientStreams.Count;
            }
            
            for (int i = 0; i < count; i++)
            {
                Tuple<TcpClient, NetworkStream, byte[], ConcurrentQueue<string>, StringBuilder> curr;
                // Need locking in case new clients are accepted while the messages are being processed.
                lock (clientStreams)
                {
                    curr = clientStreams[i];
                }
                
                ConcurrentQueue<string> messageQueue = curr.Item4;
                StringBuilder leftover = curr.Item5;

                // Read out next received chunk and separate out messages.
                if (messageQueue.TryDequeue(out fragment))
                {
                    // Received chunk is appened to the end of the leftover fragment and this is now treated as the total unprocessed data.
                    string whole = leftover.Append(fragment).ToString();
                    if (whole.Length > 0)
                    {
                        string[] lines = whole.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                        leftover.Clear();
                        int end = lines.Length;
                        // Check if last line is a complete line. If not, store the leftover and don't process it.
                        if (whole[whole.Length - 1] != '\n')
                        {
                            leftover.Append(lines[lines.Length - 1]);
                            end--;
                        }
                        // Parse all complete messages, extracting channel id, timestamp, and contents.
                        for (int j = 0; j < end; j++)
                        {
                            string[] parts = lines[j].Split(':');
                            float timestamp;
                            // Check if the message is correct format (at least datatypes are correct).
                            if (parts.Length >= 3 && float.TryParse(parts[1], out timestamp))
                            {
                                parsedMessages.Enqueue(new Tuple<string, float, string>(parts[0], timestamp, parts[2]));
                                //Debug.Log("[" + timestamp + "]\t" + parts[0] + ": " + parts[2]);
                            }
                        }
                        line_count += end;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Distribute messages from each chennel to the DataSubscribers subscribed to them.
    /// </summary>
    private void DelegateMessages()
    {
        Tuple<string, float, string> currMessage;
        while (true)
        {
            if (parsedMessages.TryDequeue(out currMessage))
            {
                List<DataSubscriber> currSubscribers;
                if (subscribers.TryGetValue(currMessage.Item1, out currSubscribers))
                {
                    foreach (DataSubscriber sb in currSubscribers)
                    {
                        sb.OnReceiveMessage(currMessage.Item2, currMessage.Item3);
                    }
                }
            }
        }
    }
}
