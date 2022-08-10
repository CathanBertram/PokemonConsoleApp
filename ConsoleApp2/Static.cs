﻿public class Static
{
    public static readonly char block = '█';
    public static readonly float healPercent = 1f;
    public static readonly int level = 100;
    public static readonly int tableWidth = 120;
    public static readonly int minStreak = 1;
    public static void PrintLine()
    {
        Console.WriteLine(new string('-', tableWidth));
    }

    public static void PrintRow(params string[] columns)
    {
        int width = (tableWidth - columns.Length) / columns.Length;
        string row = "|";

        foreach (string column in columns)
        {
            row += AlignCentre(column, width) + "|";
        }

        Console.WriteLine(row);
    }

    public static string AlignCentre(string text, int width)
    {
        text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

        if (string.IsNullOrEmpty(text))
        {
            return new string(' ', width);
        }
        else
        {
            return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
        }
    }
}

