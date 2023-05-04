using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.DDD.Domain.Primitives;
public interface IEntity
{
    public Guid Id { get; }
}
