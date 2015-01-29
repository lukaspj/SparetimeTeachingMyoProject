using System;

namespace SparetimeTeachingLibrary
{
   public static class MathHelper
   {
      public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
      {
         if (val.CompareTo(min) < 0) return min;
         if (val.CompareTo(max) > 0) return max;
         return val;
      }

      public static int Clamp(int value, int min, int max)
      {
         return (value < min) ? min : (value > max) ? max : value;
      }
   }
}
