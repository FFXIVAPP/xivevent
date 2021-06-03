// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileUtilities.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   FileUtilities.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.Utilities {
    using System;

    public static class FileUtilities {
        private static readonly string[] SizeSuffixes = {
            "bytes",
            "KB",
            "MB",
            "GB",
            "TB",
            "PB",
            "EB",
            "ZB",
            "YB",
        };

        public static string SizeSuffix(long value) {
            if (value < 0) {
                return $"-{SizeSuffix(-value)}";
            }

            if (value == 0) {
                return $"{0:n} bytes";
            }

            int sizeLevel = (int) Math.Log(value, 1024);
            decimal adjustedSize = (decimal) value / (1L << (sizeLevel * 10));

            if (Math.Round(adjustedSize, 0) >= 1000) {
                sizeLevel += 1;
                adjustedSize /= 1024;
            }

            return $"{adjustedSize:n} {SizeSuffixes[sizeLevel]}";
        }
    }
}