using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using Avalonia.Media;
using System.Drawing;

static class Extensions
{
    public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable
    {
        List<T> sorted = collection.OrderBy(x => x).ToList();
        for (int i = 0; i < sorted.Count(); i++)
            collection.Move(collection.IndexOf(sorted[i]), i);
    }

    public static string GetOrdinalSuffix(this int num)
    {
        string number = num.ToString();
        if (number.EndsWith("11")) return "th";
        if (number.EndsWith("12")) return "th";
        if (number.EndsWith("13")) return "th";
        if (number.EndsWith("1")) return "st";
        if (number.EndsWith("2")) return "nd";
        if (number.EndsWith("3")) return "rd";
        return "th";
    }

    public static string ToHex(this Avalonia.Media.Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

    public static bool IsInstalledFont(this string name)
    {
        using (Font fontTester = new Font(name, 14))
        {
            return fontTester.Name == name;
        }
    }
}