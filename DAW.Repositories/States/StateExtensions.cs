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
    public static ICollection<EventAtLocation<SoundEvent>> ToModel(this ICollection<EventAtLocationState<SoundEventState>> soundStates)
    {
        var soundsModels = new List<EventAtLocation<SoundEvent>>();

        foreach (var sound in soundStates)
        {
            var locationModel = Location.Create(sound!.Location!.Start);
            var soundEvent = SoundEvent.Create(sound!.Event!.Velocity, sound.Event.Pitch, sound.Event.Length, sound.Event.Offset);
            soundsModels.Add(EventAtLocation<SoundEvent>.Create(locationModel, soundEvent));
        }

        return soundsModels;
    }

    public static Clip ToModel(this ClipState clipState)
    {
        return Clip.Create(clipState.Sounds.ToModel(), clipState.Length, clipState.SourceId, NullNotificationPublisher.Instance);
    }
}
