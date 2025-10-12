namespace GTrack.Core.Models;

/// <summary>
/// Represents a Houston server application.
/// Stores basic information such as Id, Name, Address, Status, and connection state.
/// </summary>
///
/// <summary>
/// Представляет приложение сервера Houston.
/// Хранит базовую информацию: Id, Name, Address, Status и состояние подключения.
/// </summary>
public class HoustonApp
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Status { get; set; }
    public bool IsConnected { get; set; }
}