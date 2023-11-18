using congestion.calculator;
using System;

public class CongestionTaxCalculator
{
    private readonly CongestionTaxRule _congestionTaxRule;

    public CongestionTaxCalculator(CongestionTaxRule congestionTaxRule)
    {
        _congestionTaxRule = congestionTaxRule;
    }

    public int GetTax(Vehicle vehicle, DateTime[] dates)
    {
        int totalFee = 0;

        if (dates == null || dates.Length == 0)
        {
            return totalFee;
        }

        Array.Sort(dates);

        DateTime intervalStart = dates[0];

        foreach (DateTime date in dates)
        {
            int nextFee = _congestionTaxRule.GetTollFee(date, vehicle);
            int tempFee = _congestionTaxRule.GetTollFee(intervalStart, vehicle);

            long minutes = (date - intervalStart).Minutes;

            if (minutes <= 60)
            {
                if (totalFee > 0) totalFee -= tempFee;
                if (nextFee >= tempFee) tempFee = nextFee;
                totalFee += tempFee;
            }
            else
            {
                totalFee += nextFee;
            }

            intervalStart = date;
        }

        return Math.Min(totalFee, 60);
    }
}

public class CongestionTaxRule
{
    public bool IsTollFreeDate(DateTime date)
    {
        int year = date.Year;
        int month = date.Month;
        int day = date.Day;

        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;

        if (year == 2013)
        {
            if (month == 1 && day == 1 ||
                month == 3 && (day == 28 || day == 29) ||
                month == 4 && (day == 1 || day == 30) ||
                month == 5 && (day == 1 || day == 8 || day == 9) ||
                month == 6 && (day == 5 || day == 6 || day == 21) ||
                month == 7 ||
                month == 11 && day == 1 ||
                month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsTollFreeVehicleType(string vehicleType)
    {
        Enum.TryParse<TollFreeVehicles>(vehicleType, out var parsedType);
        return parsedType != TollFreeVehicles.None;
    }

    public int GetTollFee(DateTime date, Vehicle vehicle)
    {
        if (IsTollFreeDate(date) || IsTollFreeVehicleType(vehicle?.GetVehicleType() ?? ""))
            return 0;

        int hour = date.Hour;
        int minute = date.Minute;

        if (hour == 6 && minute >= 0 && minute <= 29) return 8;
        else if (hour == 6 && minute >= 30 && minute <= 59) return 13;
        else if (hour == 7 && minute >= 0 && minute <= 59) return 18;
        else if (hour == 8 && minute >= 0 && minute <= 29) return 13;
        else if (hour >= 8 && hour <= 14 && minute >= 30 && minute <= 59) return 8;
        else if (hour == 15 && minute >= 0 && minute <= 29) return 13;
        else if (hour == 15 && minute >= 0 || hour == 16 && minute <= 59) return 18;
        else if (hour == 17 && minute >= 0 && minute <= 59) return 13;
        else if (hour == 18 && minute >= 0 && minute <= 29) return 8;
        else return 0;
    }

    private enum TollFreeVehicles
    {
        None = -1,
        Motorcycle = 0,
        Tractor = 1,
        Emergency = 2,
        Diplomat = 3,
        Foreign = 4,
        Military = 5
    }
}

public class CongestionTaxRulesProvider
{
    public CongestionTaxRule GetCongestionTaxRuleForCity(string cityName)
    {
        // Simulate reading rules from an external source based on the city name
        // For simplicity, let's assume there is only one set of rules for all cities for now.
        return new CongestionTaxRule();
    }
}
