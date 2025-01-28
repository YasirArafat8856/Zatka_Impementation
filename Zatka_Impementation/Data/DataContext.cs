using Microsoft.EntityFrameworkCore;
using Zatka_Impementation_Testing.Entities;

namespace Zatka_Impementation_Testing.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<EInvoice> EInvoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
    }
}
