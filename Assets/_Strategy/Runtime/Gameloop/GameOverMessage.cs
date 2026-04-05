using _Strategy.Runtime.Board;
using Runtime.Messaging;

namespace _Strategy.Runtime.Gameloop
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