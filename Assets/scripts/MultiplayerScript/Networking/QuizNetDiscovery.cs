using Mirror;
using Mirror.Discovery;
using System;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizNetDiscovery 
    : NetworkDiscoveryBase<ServerRequest, QuizServerResponse>
{

    public string roomName = "Room";
    public bool isGameStarted = false;

    protected override ServerRequest GetRequest()
    {
        return new ServerRequest();
    }



protected override QuizServerResponse ProcessRequest(
    ServerRequest request,
    IPEndPoint endpoint)
{

    Debug.Log("QUIZ DISCOVERY: ProcessRequest was called!");

    Debug.Log(
        $"DISCOVERY REQUEST: " +
        $"Server connections = {NetworkServer.connections.Count}"
    );

    string hostName =
        PlayerPrefs.GetString("UserName", "Player");

        // =========================================
    // CHECK GAME STATE
    // =========================================

    string currentScene =
        SceneManager.GetActiveScene().name;

    // bool isInGame =
    //     currentScene == "Multi_Indu" ||
    //     currentScene == "Multi_Synonym" ||
    //     currentScene == "Multi_Antonym";

    return new QuizServerResponse
    {
        serverId = ServerId,

        uri = transport.ServerUri(),

        roomName = hostName + "'s Room",

        gameMode =
            NetworkRoomSettings.Instance.GetGameMode(),

        difficulty =
            NetworkRoomSettings.Instance.GetDifficulty(),

        timeMode =
            NetworkRoomSettings.Instance.GetTimeMode(),

        currentPlayers =
            NetworkServer.connections.Count,

        maxPlayers = 4,


        isInGame =
            isGameStarted
    };
}


    protected override void ProcessResponse(
        QuizServerResponse response,
        IPEndPoint endpoint)
    {

        response.uri =
            new System.UriBuilder(response.uri)
            {
                Host = endpoint.Address.ToString()
            }.Uri;


        OnServerFound.Invoke(response);
    }

}