using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using GTrack.Core.Models;
using GTrack.Core.Services;
using Microsoft.Extensions.Logging;

namespace GTrack.Infrastructure.Services;

public class ObserverLocationService : IObserverLocationService
{
    private readonly ILogger<ObserverLocationService> _logger;
    private readonly string _filePath;

    public ObserverLocationService(ILogger<ObserverLocationService> logger)
    {
        _logger = logger;
        _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "observer_locations.json");
    }

    public async Task<ObservableCollection<ObserverLocation>> LoadAsync()
    {
        try
        {
            if (!File.Exists(_filePath))
                return new ObservableCollection<ObserverLocation>();

            var json = await File.ReadAllTextAsync(_filePath);
            var list = JsonSerializer.Deserialize<List<ObserverLocation>>(json) ?? new List<ObserverLocation>();
            return new ObservableCollection<ObserverLocation>(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ObserverLocationService] Ошибка при загрузке точек наблюдения");
            return new ObservableCollection<ObserverLocation>();
        }
    }

    public async Task SaveAsync(ObservableCollection<ObserverLocation> locations)
    {
        try
        {
            var json = JsonSerializer.Serialize(locations, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ObserverLocationService] Ошибка при сохранении точек наблюдения");
        }
    }
}