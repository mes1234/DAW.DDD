using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.ValueObjects;
using DAW.Repositories.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.Repositories.States;
internal static class StateExtensions
{
    public static EventAtLocation<SoundEvent> ToModel(this EventAtLocationState<SoundEventState> state)
    {
        var locationModel = Location.Create(state!.Location!.Start);
        var soundEvent = SoundEvent.Create(state!.Event!.Velocity, state.Event.Pitch, state.Event.Length, state.Event.Offset);
        return EventAtLocation<SoundEvent>.Create(locationModel, soundEvent);
    }

    public static EventAtLocation<Clip> ToModel(this EventAtLocationState<ClipState> state)
    {
        var locationModel = Location.Create(state!.Location!.Start);
        var clip = state.Event!.ToModel();
        return EventAtLocation<Clip>.Create(locationModel, clip);
    }

    public static Clip ToModel(this ClipState state)
    {
        return Clip.Create(state.Id, state.Sounds.Select(x => x.ToModel()).ToList(), state.Length, NullNotificationPublisher.Instance);
    }

    public static Track ToModel(this TrackState state, IEnumerable<Clip> clips)
    {
        var clipsAtLocations = new List<EventAtLocation<Clip>>();

        // Still hacky but better
        foreach (var clipState in state.Clips)
        {
            clipsAtLocations.Add(
                EventAtLocation<Clip>.Create(
                    Location.Create(clipState.Location!.Start, true),
                    clips.First(x => x.Id == clipState.Event)));
        }

        return Track.Create(state.Id, clipsAtLocations, state.SourceId, NullNotificationPublisher.Instance);
    }
}
