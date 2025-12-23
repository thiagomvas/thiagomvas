using System;
using System.Linq;
using System.Text;

public static class Helpers
{
    // -------------------- PUBLIC API --------------------

    public static string DotLine(int width, params (string label, object value)[] items)
    {
        const string separator = " | ";

        int fixedLength =
            items.Sum(i => i.label.Length + 2 + i.value.ToString()!.Length) +
            separator.Length * (items.Length - 1);

        int totalDots = Math.Max(0, width - fixedLength);

        int dotsPerItem = items.Length > 0 ? totalDots / items.Length : 0;
        int remainder = items.Length > 0 ? totalDots % items.Length : 0;

        var parts = items.Select((item, index) =>
        {
            int dots = dotsPerItem + (index < remainder ? 1 : 0);
            return $"{item.label}: {new string('.', dots)}{item.value}";
        });

        string line = string.Join(separator, parts);

        return line.Length > width
            ? line[..width]
            : line.PadRight(width, '.');
    }

    public static string GetElapsedYMD(DateTime from, DateTime to)
    {
        if (from > to)
            (from, to) = (to, from);

        int years = 0;
        int months = 0;

        DateTime cursor = from;

        while (cursor.AddYears(1) <= to)
        {
            cursor = cursor.AddYears(1);
            years++;
        }

        while (cursor.AddMonths(1) <= to)
        {
            cursor = cursor.AddMonths(1);
            months++;
        }

        int days = (to - cursor).Days;

        return $"{years} years, {months} months, {days} days";
    }

    public static string GenerateSvg(
        string[] ascii,
        string[] lines,
        SvgTheme theme = SvgTheme.Dark)
    {
        const int padding = 24;
        const int lineHeight = 22;
        const int charWidth = 8;

        var t = GetTheme(theme);

        int asciiMaxChars = ascii.Length == 0 ? 0 : ascii.Max(l => l.Length);
        int asciiPixelWidth = asciiMaxChars * charWidth;

        int textStartX = padding + asciiPixelWidth + 32;

        int rightMaxChars = lines.Length == 0 ? 0 : lines.Max(l => l.Length);
        int rightPixelWidth = rightMaxChars * charWidth;

        int totalLines = Math.Max(ascii.Length, lines.Length);
        int width = textStartX + rightPixelWidth + padding + 16;
        int height = padding * 2 + totalLines * lineHeight;

        var sb = new StringBuilder();

        sb.AppendLine($@"
<svg xmlns='http://www.w3.org/2000/svg'
     width='{width}'
     height='{height}'
     font-family='Consolas, monospace'
     font-size='14'>
<style>
.bg {{ fill: {t.bg}; }}
.key {{ fill: {t.key}; }}
.value {{ fill: {t.value}; }}
.dots {{ fill: {t.dots}; }}
.sep {{ fill: {t.sep}; }}
.ascii {{ fill: {t.ascii}; }}
</style>

<rect class='bg' width='100%' height='100%' rx='12'/>
");

        int y = padding + 16;

        // ASCII (left)
        foreach (var line in ascii)
        {
            sb.AppendLine(
                $"<text x='{padding}' y='{y}' class='ascii'>{Escape(line)}</text>");
            y += lineHeight;
        }

        y = padding + 16;

        // Text (right)
        foreach (var line in lines)
        {
            sb.Append($"<text x='{textStartX}' y='{y}'>");
            RenderColoredLine(sb, line);
            sb.AppendLine("</text>");
            y += lineHeight;
        }

        sb.AppendLine("</svg>");
        return sb.ToString();
    }

    // -------------------- INTERNALS --------------------

    private static void RenderColoredLine(StringBuilder sb, string line)
    {
        var parts = line.Split(" | ", StringSplitOptions.None);

        for (int i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            int colon = part.IndexOf(':');

            if (colon > 0)
            {
                string key = part[..colon];
                string rest = part[(colon + 1)..];

                int dotStart = rest.TakeWhile(c => c == ' ' || c == '.').Count();
                string dots = rest[..dotStart];
                string value = rest[dotStart..];

                sb.Append($"<tspan class='key'>{Escape(key)}</tspan>");
                sb.Append($"<tspan class='sep'>:</tspan>");
                sb.Append($"<tspan class='dots'>{Escape(dots)}</tspan>");
                sb.Append($"<tspan class='value'>{Escape(value)}</tspan>");
            }
            else
            {
                sb.Append($"<tspan class='value'>{Escape(part)}</tspan>");
            }

            if (i < parts.Length - 1)
                sb.Append("<tspan class='sep'> | </tspan>");
        }
    }

    private static string Escape(string s) =>
        System.Security.SecurityElement.Escape(s) ?? "";

    private static (string bg, string key, string value, string dots, string sep, string ascii)
        GetTheme(SvgTheme theme)
    {
        return theme switch
        {
            SvgTheme.Light => (
                bg: "#f6f8fa",
                key: "#0969da",
                value: "#24292f",
                dots: "#d0d7de",
                sep: "#57606a",
                ascii: "#24292f"
            ),
            _ => ( // Dark
                bg: "#0d1117",
                key: "#79c0ff",
                value: "#c9d1d9",
                dots: "#484f58",
                sep: "#8b949e",
                ascii: "#c9d1d9"
            )
        };
    }
}

// -------------------- THEME ENUM --------------------

public enum SvgTheme
{
    Dark,
    Light
}
