namespace _Strategy.Runtime.Board
{
    public enum Team
    {
        White,
        Black,
        None
    }

    public static class TeamExtensions
    {
        public static Team GetOther(this Team team) => team switch
        {
            Team.White => Team.Black,
            Team.Black => Team.White,
            _ => Team.None
        };
    }
}