using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
namespace CongestionTaxCalculator
{
    public class Config
    {
        public List<string> TollFreeVehicles { get; set; }
        public Dictionary<string, List<TollFreeDate>> TollFreeDates { get; set; }
        public List<TollFee> TollFees { get; set; }
        public int MaxDailyFee { get; set; }
        public int MaxInterval { get; set; }

        public static Config LoadConfig(string path)
        {
            var configJson = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Config>(configJson);
        }
    }
}
