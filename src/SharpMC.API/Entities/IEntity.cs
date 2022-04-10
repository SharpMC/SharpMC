using System;
using SharpMC.API.Utils;

namespace SharpMC.API.Entities
{
    public interface IEntity : ICustomizable
    {
        Guid UniqueId { get; set; }

        string Name { get; set; }

        double Health { get; set; }

        ILocation Location { get; }
    }
}