namespace ExplosivesDude
{
    public interface IUIOperationProvider
    {
        ////void AddImage(System.Windows.Controls.Image image);

        ////void RemoveImage(System.Windows.Controls.Image image);

        void StartCountdown(int seconds);

        void StopCountdown();

        void WriteConnectionStatus(string status, bool playerCount);

        void UpdateClientCount(int count);

        void UpdatePlayerCount(int count);

        void UpdateClientId(int id);
    }
}
