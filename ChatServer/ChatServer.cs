// <copyright file="ChatServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

using CS3500.Networking;
using System.Collections.Generic;
using System.Text;

namespace CS3500.Chatting;

/// <summary>
///   A simple ChatServer that handles clients separately and replies with a static message.
/// </summary>
public partial class ChatServer
{
    private static List<NetworkConnection> connections = new();

    /// <summary>
    ///   The main program.
    /// </summary>
    /// <param name="args"> ignored. </param>
    /// <returns> A Task. Not really used. </returns>
    private static void Main( string[] args )
    {
        Server.StartServer( HandleConnect, 11_000 );
        Console.Read(); // don't stop the program.
    }


    /// <summary>
    ///   <pre>
    ///     When a new connection is established, enter a loop that receives from and
    ///     replies to a client.
    ///   </pre>
    /// </summary>
    ///
    private static void HandleConnect( NetworkConnection connection )
    {
        // handle all messages until disconnect.
        try
        {
            connection.Send("Enter your username: ");
            string username = connection.ReadLine();
            connection.Send("You will be chatting as " + username);

            lock (connection)
            {
                connections.Add(connection);
            }

            while ( true )
            {
                var message = connection.ReadLine( );

                List<NetworkConnection> connectionsCopy;

                lock (connection)
                {
                    connectionsCopy = connections.ToList();
                }
                foreach (NetworkConnection c in connectionsCopy)
                {
                    c.Send($"{username}: {message}");
                }
            }
        }
        catch ( Exception )
        {
            // do anything necessary to handle a disconnected client in here
        }
    }
}