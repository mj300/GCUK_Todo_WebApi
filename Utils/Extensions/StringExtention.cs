using System;

namespace todo_webApi.Utils.Extensions
{
   public static class StringExtention
   {
      public static bool EqualCurrentCultureIgnoreCase(this string orgValue, string compareWithValue) =>
  string.Equals(orgValue, compareWithValue, StringComparison.CurrentCultureIgnoreCase);

   }
}
