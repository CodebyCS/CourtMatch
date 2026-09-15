using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Contracts.Game
{
    /// <summary>
    /// Represents the result of a court occupancy check performed by the Game API.
    /// </summary>
    public class CourtOccupiedResponse
    {
        /// <summary>Gets or sets whether the court is occupied at the requested time.</summary>
        public bool IsOccupied { get; set; }
    }
}