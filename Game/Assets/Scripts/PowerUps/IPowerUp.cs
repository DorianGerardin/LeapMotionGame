namespace PowerUps
{
    public interface IPowerUp
    {
        Player Player { get; set; }

        string Label { get; set; }

        void Execute();
    }
}
