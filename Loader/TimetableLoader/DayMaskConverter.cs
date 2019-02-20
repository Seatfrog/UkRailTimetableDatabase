using System;

namespace TimetableLoader
{
    public static class DayMaskConverter
    {       
        public static object Convert(string mask)
        {
            if (string.IsNullOrWhiteSpace(mask))
                return DBNull.Value;

            byte val = 0;
            for (var i = 0; i < 7; i++)
            {
                val += (byte) (mask[i].Equals('1') ? 1 << i  : 0);
            }
           
            return val;
        }

        public static object ConvertBankHoliday(string bankHolidayFlag, bool isCancelOrDelete)
        {
            return isCancelOrDelete && string.IsNullOrWhiteSpace(bankHolidayFlag) ?
                (object) DBNull.Value : 
                bankHolidayFlag;
        }
    }
}