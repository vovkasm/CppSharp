﻿using System;

namespace Cxxi.Passes
{
    class CheckFlagEnumsPass : TranslationUnitPass
    {
        static bool IsFlagEnum(Enumeration @enum)
        {
            // If the enumeration only has power of two values, assume it's
            // a flags enum.

            bool isFlags = true;
            bool hasBigRange = false;

            foreach (var item in @enum.Items)
            {
                if (item.Name.Length >= 1 && Char.IsDigit(item.Name[0]))
                    item.Name = String.Format("_{0}", item.Name);

                long value = item.Value;
                if (value >= 4)
                    hasBigRange = true;
                if (value <= 1 || value.IsPowerOfTwo())
                    continue;
                isFlags = false;
            }

            // Only apply this heuristic if there are enough values to have a
            // reasonable chance that it really is a bitfield.

            return isFlags && hasBigRange;
        }

        public override bool ProcessEnum(Enumeration @enum)
        {
            if (IsFlagEnum(@enum))
            {
                @enum.Modifiers |= Enumeration.EnumModifiers.Flags;
                return true;
            }

            return false;
        }
    }

    public static class CheckFlagEnumsExtensions
    {
        public static void CheckFlagEnums(this PassBuilder builder)
        {
            var pass = new CheckFlagEnumsPass();
            builder.AddPass(pass);
        }
    }
}
