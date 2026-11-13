using Mirror;
using System;

public struct QuizServerResponse : NetworkMessage
{
    public Uri uri;

    public long serverId;

    public string roomName;

    public string gameMode;

    public string difficulty;

    public string timeMode;

    public int currentPlayers;

    public int maxPlayers;

    public bool isInGame;
}