namespace GTrack.Core.Models;

/// <summary>
/// Represents the status of a server with bindable properties.
/// </summary>
///
/// <summary>
/// Представляет состояние сервера с поддержкой привязки данных.
/// </summary>
public class ServerStatus : BindableBase
{
    public string Name { get; set; }

    private bool _status;
    public bool Status { get => _status; set => SetProperty(ref _status, value); }
}