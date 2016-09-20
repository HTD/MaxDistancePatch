using System;
using System.Globalization;

namespace Woof.Core.FileSystem {

    /// <summary>
    /// Dynamic size type allows presenting byte sizes in dynamic units.
    /// </summary>
    public class SizeString {

        private readonly long InBytes;
        private readonly IFormatProvider FormatProvider = CultureInfo.CurrentCulture;

        public SizeString(int size, IFormatProvider formatProvider = null) {
            InBytes = size;
            if (formatProvider != null) FormatProvider = formatProvider;
        }

        public SizeString(long size, IFormatProvider formatProvider = null) {
            InBytes = size;
            if (formatProvider != null) FormatProvider = formatProvider;
        }

        private static readonly string[] Units = new [] { "B", "KB", "MB", "GB", "TB", "PB" };
        private static readonly int UnitsLength = Units.Length;
        private const double K = 1024d; // kibi

        public override string ToString() {
            double dynamic = InBytes;
            int i;
            for (i = 0; i < UnitsLength && dynamic > K; i++) dynamic /= K;
            string unit = Units[i];
            dynamic = Math.Round(dynamic, 1);
            return dynamic.ToString(FormatProvider) + ' ' + unit;
        }

        public static implicit operator string(SizeString s) => s.ToString();

        public static implicit operator long(SizeString s) => s.InBytes;

        public static implicit operator int(SizeString s) => (int)s.InBytes;

        public static implicit operator SizeString(long s) => new SizeString(s);

        public static implicit operator SizeString(int s) => new SizeString(s);

    }

}