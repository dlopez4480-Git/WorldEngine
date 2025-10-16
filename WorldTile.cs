using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace testProgram
{
    public class WorldTile
    {
        //  Info Values
        public String locationID { get; set; }
        public String locationName { get; set; }
        public Coords coordinates { get; set; }


        //  Terrain Values
        public String landType { get; set; }     //  WATER, LAND, MOUNTAIN



        public WorldTile(string locationID, string locationName, Coords coordinates, string landType)
        {
            this.locationID = locationID;
            this.locationName = locationName;
            this.coordinates = coordinates;
            this.landType = landType;

        }
    }
}
