using System.Collections.Generic;
using System.Text;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class EnemyFormation : Formation
    {
        public Dictionary<VehicleType, int> TypesCount
        {
            get
            {
                Dictionary<VehicleType, int> dictionary = new Dictionary<VehicleType, int>
                {
                    {VehicleType.Fighter, 0},
                    {VehicleType.Helicopter, 0},
                    {VehicleType.Arrv, 0},
                    {VehicleType.Ifv, 0},
                    {VehicleType.Tank, 0}
                };
                foreach (var vehicle in Vehicles.Values)
                {
                    dictionary[vehicle.Type]++;
                }

                return dictionary;
            }
        }

        public override void Update(IEnumerable<VehicleUpdate> updates = null)
        {
            base.Update(updates);
        }

        private string TypesToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (var kv in TypesCount)
            {
                if (kv.Value > 0)
                    result.Append($" {kv.Key}:{kv.Value}");
            }
            return result.ToString();
        }

        public override string ToString()
        {
            return base.ToString() +TypesToString();
        }
    }
}