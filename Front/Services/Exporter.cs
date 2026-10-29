using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Front.Services
{
    /// <summary>Builds the Excel and PDF exports of the list pages (same headers and rows for both).</summary>
    public static class Exporter
    {
        public const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public static byte[] ToExcel(string sheetName, string[] headers, IEnumerable<object?[]> rows)
        {
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add(sheetName);

            for (int column = 0; column < headers.Length; column++)
            {
                sheet.Cells[1, column + 1].Value = headers[column];
            }

            int row = 2;
            foreach (var values in rows)
            {
                for (int column = 0; column < values.Length; column++)
                {
                    sheet.Cells[row, column + 1].Value = values[column];
                }

                row++;
            }

            sheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        public static byte[] ToPdf(string title, string[] headers, IEnumerable<object?[]> rows)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Text(title).SemiBold().FontSize(16).AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (var _ in headers)
                            {
                                columns.RelativeColumn();
                            }
                        });

                        table.Header(header =>
                        {
                            foreach (var text in headers)
                            {
                                header.Cell().Text(text).SemiBold();
                            }
                        });

                        foreach (var values in rows)
                        {
                            foreach (var value in values)
                            {
                                table.Cell().Text(Format(value));
                            }
                        }
                    });
                });
            }).GeneratePdf();
        }

        private static string Format(object? value) => value switch
        {
            null => "-",
            decimal amount => amount.ToString("N2"),
            DateTime date => date.ToShortDateString(),
            _ => value.ToString() ?? "-"
        };
    }
}
