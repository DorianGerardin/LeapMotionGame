namespace PowerUps
{
    public interface IPowerUp
    {
        Player Player { get; set; }

        void Execute();
    }
}
