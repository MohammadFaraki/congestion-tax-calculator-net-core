using CongestionTaxCalculator.Models;
using System;

namespace CongestionTaxCalculator.Services
{
    public interface ITaxCalculator
    {
        int GetTax(Vehicle vehicle, DateTime[] dates);
    }
}