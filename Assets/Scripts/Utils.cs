using UnityEngine;

internal static class Utils
{
    public static bool IsInsideMapBounds(Vector2 point)
    {
        return point.x is >= SmashTanksConstants.MapBounds.MinX and <= SmashTanksConstants.MapBounds.MaxX &&
               point.y is >= SmashTanksConstants.MapBounds.MinY and <= SmashTanksConstants.MapBounds.MaxY;
    }
    
    public static string SnakeFromTitle(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var result = new System.Text.StringBuilder();
        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsWhiteSpace(c) && i > 0)
            {
                result.Append('_');
                continue;
            }
            result.Append(char.ToLower(c));
        }
        return result.ToString();
    }
}