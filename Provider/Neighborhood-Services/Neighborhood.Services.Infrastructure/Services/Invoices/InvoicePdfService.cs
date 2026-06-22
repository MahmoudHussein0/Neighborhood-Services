using Neighborhood.Services.Application.Invoices.DTOs;
using Neighborhood.Services.Application.Invoices.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Neighborhood.Services.Infrastructure.Services.Invoices
{
    public class InvoicePdfService : IInvoicePdfService
    {
        public byte[] GenerateInvoicePdf(InvoiceResponseDto invoice)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial).FontColor(Colors.Black));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(x => ComposeContent(x, invoice));
                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(28).SemiBold().FontColor(Colors.Blue.Darken2);

            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("NEIGHBORHOOD SERVICES").Style(titleStyle);
                    column.Item().Text("Your Trusted Local Professionals").FontSize(10).FontColor(Colors.Grey.Medium);
                });

                row.ConstantItem(100).AlignRight().Text("INVOICE").FontSize(24).SemiBold().FontColor(Colors.Grey.Lighten1);
            });
        }

        private void ComposeContent(IContainer container, InvoiceResponseDto invoice)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(20);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Component(new AddressComponent("Billed To", invoice.CustomerName ?? $"Customer #{invoice.CustomerId}"));
                    row.RelativeItem().Component(new AddressComponent("Service Provider", invoice.TechnicianName ?? $"Technician #{invoice.TechnicianId}"));
                    row.RelativeItem().Component(new InvoiceDetailsComponent(invoice));
                });

                column.Item().PaddingTop(15).Element(x => ComposeTable(x, invoice));

                var totalPrice = invoice.TotalAmount;
                column.Item().AlignRight().PaddingTop(15).Text($"Total Paid: EGP {totalPrice:N2}").FontSize(18).SemiBold().FontColor(Colors.Blue.Darken3);
            });
        }

        private void ComposeTable(IContainer container, InvoiceResponseDto invoice)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Description").SemiBold();
                    header.Cell().Element(CellStyle).AlignRight().Text("Base Price").SemiBold();
                    header.Cell().Element(CellStyle).AlignRight().Text("Discount/Tax").SemiBold();

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                // Base Booking Cost
                table.Cell().Element(CellStyle).Text($"Quoted Service Price (Booking #{invoice.BookingId})");
                table.Cell().Element(CellStyle).AlignRight().Text($"EGP {invoice.BaseAmount:N2}");
                table.Cell().Element(CellStyle).AlignRight().Text("-");

                // Promo Code Line
                if (invoice.DiscountAmount > 0)
                {
                    var promoLabel = string.IsNullOrWhiteSpace(invoice.PromoCodeApplied) ? "Discount Applied" : $"Promo Code: {invoice.PromoCodeApplied}";
                    table.Cell().Element(CellStyle).Text(promoLabel).FontColor(Colors.Green.Darken2);
                    table.Cell().Element(CellStyle).AlignRight().Text("-");
                    table.Cell().Element(CellStyle).AlignRight().Text($"- EGP {invoice.DiscountAmount:N2}").FontColor(Colors.Green.Darken2);
                }

                // Tax Line
                if (invoice.Tax > 0)
                {
                    table.Cell().Element(CellStyle).Text("Taxes & Fees");
                    table.Cell().Element(CellStyle).AlignRight().Text("-");
                    table.Cell().Element(CellStyle).AlignRight().Text($"+ EGP {invoice.Tax:N2}");
                }

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(8);
                }
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(x =>
            {
                x.Span("Thank you for using Neighborhood Services! • ");
                x.Span("Generated Automatically at " + DateTime.UtcNow.ToString("dd MMM yyyy, hh:mm tt")).FontColor(Colors.Grey.Medium);
            });
        }
    }

    public class AddressComponent : IComponent
    {
        private string Title { get; }
        private string Details { get; }

        public AddressComponent(string title, string details)
        {
            Title = title;
            Details = details;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(4);
                column.Item().Text(Title).SemiBold().FontColor(Colors.Grey.Darken3);
                column.Item().Text(Details).FontSize(11).FontColor(Colors.Black);
            });
        }
    }

    public class InvoiceDetailsComponent : IComponent
    {
        private InvoiceResponseDto Invoice { get; }

        public InvoiceDetailsComponent(InvoiceResponseDto invoice)
        {
            Invoice = invoice;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(4);
                column.Item().Text($"Invoice #{Invoice.Id}").SemiBold().FontColor(Colors.Grey.Darken3);
                column.Item().Text($"Status: {Invoice.Status}").FontSize(11).FontColor(Colors.Black);
                column.Item().Text($"Issued: {Invoice.IssuedAt:dd MMM yyyy, hh:mm tt}").FontSize(11).FontColor(Colors.Black);
            });
        }
    }
}
