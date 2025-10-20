using System.Collections.ObjectModel;
using GTrack.Core.Models;

namespace GTrack.Core.Services;

public interface IObserverLocationService
{
    Task<ObservableCollection<ObserverLocation>> LoadAsync();
    Task SaveAsync(ObservableCollection<ObserverLocation> locations);
}