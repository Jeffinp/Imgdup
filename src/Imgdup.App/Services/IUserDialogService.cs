namespace Imgdup.App.Services;

public interface IUserDialogService
{
    /// <summary>Asks the user to confirm a destructive action. Returns true if confirmed.</summary>
    bool Confirm(string message, string title);

    void Notify(string message, string title);
}
