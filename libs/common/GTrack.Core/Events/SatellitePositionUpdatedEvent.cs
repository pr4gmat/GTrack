using GTrack.Core.Models;

namespace GTrack.Core.Events;

public class SatellitePositionUpdatedEvent : PubSubEvent<SatelliteObservationResult> { }