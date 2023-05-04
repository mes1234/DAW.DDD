using DAW.DDD.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.DDD.Domain.Primitives;
internal interface IPlayable
{
    IReadOnlyCollection<EventAtLocation<IReadOnlyCollection<SoundEvent>>> GetPlayableEvents(Location offset);
}
