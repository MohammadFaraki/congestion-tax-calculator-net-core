using CongestionTaxCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CongestionTaxCalculator.Services
{
    public class TaxCalculator : ITaxCalculator
    {
        private readonly Config _config;

        public TaxCalculator(Config config)
        {
            _config = config;
        }

        /**
        * Calculate the total toll fee for one day
        *
        * @param vehicle - the vehicle
        * @param dates   - date and time of all passes on one day
        * @return - the total congestion tax for that day
        */
        public int GetTax(Vehicle vehicle, DateTime[] dates)
        {
            int totalFee = 0;
            var underOneHourPasses = new List<(int fee, DateTime time)>();

            foreach (var date in dates)
            {
                int currentFee = GetTollFee(date, vehicle);
                underOneHourPasses.Add((currentFee, date));

                var diff = underOneHourPasses.Last().time - underOneHourPasses.First().time;
                double minutes = diff.TotalMinutes;

                if (minutes >= _config.MaxInterval)
                {
                    // Remove the last item which isn't within 60 min with other list elements
                    underOneHourPasses.RemoveAt(underOneHourPasses.Count - 1);

                    totalFee += underOneHourPasses.Max(f => f.fee);
                    underOneHourPasses = new List<(int fee, DateTime time)>();
                    underOneHourPasses.Add((currentFee, date));
                }
            }

            // Item(s) that remained in the list at the end of foreach loop
            totalFee += underOneHourPasses.Max(f => f.fee);
            if (totalFee > _config.MaxDailyFee) totalFee = _config.MaxDailyFee;

            return totalFee;
        }

        private bool IsTollFreeVehicle(Vehicle vehicle)
        {
            if (vehicle == null) return false;
            string vehicleType = vehicle.GetVehicleType();
            return _config.TollFreeVehicles.Contains(vehicleType);
        }

        private int GetTollFee(DateTime date, Vehicle vehicle)
        {
            if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;

            var time = date.TimeOfDay;
            foreach (var fee in _config.TollFees)
            {
                var startTime = TimeSpan.Parse(fee.Start);
                var endTime = TimeSpan.Parse(fee.End);
                if (time >= startTime && time <= endTime)
                {
                    return fee.Fee;
                }
            }
            return 0;
        }

        private bool IsTollFreeDate(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;

            if (_config.TollFreeDates.TryGetValue(date.Year.ToString(), out var months))
            {
                foreach (var month in months)
                {
                    if (month.Month == date.Month && month.Days.Contains(date.Day))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
