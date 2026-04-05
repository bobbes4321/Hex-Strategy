using Runtime.Messaging;

namespace Runtime.Chess
{
    public struct GameOverMessage : IMessage
    {
        public Team WinningTeam;

        public GameOverMessage(Team winningTeam)
        {
            WinningTeam = winningTeam;
        }
    }
}