using System;

namespace SpotyPie.Helpers
{
    public static class Caster
    {
        public static dynamic Cast(dynamic obj, Type castTo)
        {
            return Convert.ChangeType(obj, castTo);
        }
    }
}