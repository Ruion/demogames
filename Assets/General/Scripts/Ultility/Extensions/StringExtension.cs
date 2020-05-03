using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class StringExtensions
{
    public static string FirstCharToUpper(this string s)
    {
        if (string.IsNullOrEmpty(s))
            throw new ArgumentException("There is no first letter");

        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }

    public static bool ContainsAny(this string input, IEnumerable<string> containsKeywords, StringComparison comparisonType)
    {
        return containsKeywords.Any(keyword => input.IndexOf(keyword, comparisonType) >= 0);
    }

    public static string GetNumbers(string input)
    {
        return new string(input.Where(c => char.IsDigit(c)).ToArray());
    }

    public static string RemoveCharacters(string value, string[] charsToRemove)
    {
        string newValue = value;

        foreach (var c in charsToRemove)
        {
            newValue = newValue.Replace(c, string.Empty);
        }

        return newValue;
    }

    public static string RemoveAlphabets(string oldString)
    {
        var newValue = Regex.Replace(oldString, @"[A-Za-z]+", "");

        return newValue;
    }
}