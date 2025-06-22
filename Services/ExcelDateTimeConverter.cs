using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;

namespace PhasePlayWeb.Services
{
    public class ExcelDateTimeConverter : DefaultTypeConverter
    {
        private static readonly DateTime OriginDate = new DateTime(1900, 1, 1);

        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            if (decimal.TryParse(text, out decimal days))
            {
                if (days % 1 == 0) // Check if the value is an integer
                {
                    return OriginDate.AddDays((int)days - 2); // Adjust for Excel's date system
                }

                else if (days.GetType() == typeof(DateTime)) // Check if days is of type DateTime
                {
                    return days;
                }
                else if (days.GetType() == typeof(TimeSpan))// Handle other decimal values
                {
                    return days;
                }
                else
                { 
                    return TimeSpan.Zero;
                }
            }

            throw new TypeConverterException(this, memberMapData, text, row.Context, "The conversion cannot be performed.");
        }
    }
}
