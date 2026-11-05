using System.Globalization;
using System.Text;
using CampaignTracker.Api.DTOs;

namespace CampaignTracker.Api.Services;

/// <summary>
/// Writes expenses as RFC 4180 CSV. Dates and numbers are culture-invariant
/// (yyyy-MM-dd and "1234.50"), so the file looks the same on every server.
/// </summary>
public static class ExpenseCsvExporter
{
    private const string Header = "Date,Description,Category,Amount";

    public static string ToCsv(IEnumerable<ExpenseDto> expenses)
    {
        var csv = new StringBuilder();
        csv.Append(Header).Append("\r\n");

        foreach (var expense in expenses)
        {
            csv.Append(expense.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)).Append(',')
               .Append(Escape(expense.Description)).Append(',')
               .Append(Escape(expense.CategoryName)).Append(',')
               .Append(expense.Amount.ToString("0.00", CultureInfo.InvariantCulture))
               .Append("\r\n");
        }

        return csv.ToString();
    }

    /// <summary>
    /// Quotes a text field when needed and neutralizes values that spreadsheet apps
    /// would run as formulas (CSV injection), e.g. "=SUM(A1:A9)".
    /// </summary>
    public static string Escape(string value)
    {
        if (value.Length > 0 && "=+-@\t\r".Contains(value[0]))
        {
            value = "'" + value;
        }

        var needsQuotes = value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) >= 0;
        return needsQuotes
            ? "\"" + value.Replace("\"", "\"\"") + "\""
            : value;
    }
}
