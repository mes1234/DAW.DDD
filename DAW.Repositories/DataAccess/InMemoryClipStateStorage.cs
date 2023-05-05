using DAW.Repositories.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.Repositories.DataAccess;

/// <summary>
/// Represents simple in memory storage of data
/// </summary>
internal class InMemoryClipStateStorage : IModelStateProvider<ClipState>
{
    // Implement
    public Task<ClipState?> TryGet(Guid id)
    {
        throw new NotImplementedException();
    }
}
