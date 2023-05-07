namespace PGK.Application.Common.Date
{
    public enum Month
    {
        NotSet = 0,
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public static class MonthExtensions
    {
        public static string GetNameValue(this Month month)
        {
            return month switch
            {
                Month.January => "Январь",
                Month.February => "Февраль",
                Month.March => "Март",
                Month.April => "Апрель",
                Month.May => "Май",
                Month.June => "Июнь",
                Month.July => "Июль",
                Month.August => "Август",
                Month.September => "Сентябрь",
                Month.October => "Октябрь",
                Month.November => "Ноябрь",
                Month.December => "Декабрь",
                _ => "",
            };
        }
    }
}
