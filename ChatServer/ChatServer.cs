// <copyright file="ChatServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Thomas Lu and Jakob Stabile </authors>
// <version> 03.27.2025 </version>

using CS3500.Networking;

namespace CS3500.Chatting;

/// <summary>
///   An upgraded ChatServer that handles clients separately and relays a message from a client
///   to all other connected clients, akin to a chatroom.
/// </summary>
public partial class ChatServer
{

    /// <summary>
    /// Storage for all NetworkConnections and the username of the connection
    /// </summary>
    private static Dictionary<NetworkConnection, string> connections = [];

    /// <summary>
    ///   The main program.
    /// </summary>
    /// <param name="args"> ignored. </param>
    /// <returns> A Task. Not really used. </returns>
    private static void Main(string[] args)
    {
        Server.StartServer(HandleConnect, 11_000);
        Console.Read(); // don't stop the program.
    }

    /// <summary>
    ///   <pre>
    ///     When a new connection is established, enter a loop that receives from and
    ///     replies to a client.
    ///   </pre>
    /// </summary>
    ///
    private static void HandleConnect(NetworkConnection connection)
    {
        // handle all messages until disconnect.
        try
        {
            connection.Send("Enter your username: ");
            string username = connection.ReadLine();
            connection.Send("You will be chatting as " + username);

            lock (connection)
            {
                connections.Add(connection, username);
            }

            while (true)
            {
                var message = connection.ReadLine();

                List<NetworkConnection> connectionsCopy;

                lock (connection)
                {
                    connectionsCopy = connections.Keys.ToList();
                }
                foreach (NetworkConnection c in connectionsCopy)
                {
                    c.Send($"{username}: {message}");
                }
            }
        }
        catch (Exception)
        {
            // do anything necessary to handle a disconnected client in here
            List<NetworkConnection> connectionsCopy;
            string username = connections[connection];
            lock (connection)
            {
                connections.Remove(connection);
                connectionsCopy = connections.Keys.ToList();
            }

            foreach (NetworkConnection c in connectionsCopy)
            {
                c.Send($"{username} left the chatroom.");
            }

        }
    }
}