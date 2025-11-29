using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Core.Entities;

namespace SubscriptionManager.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // DbSet'ы для всех наших таблиц
    public DbSet<User> Users => Set<User>();
    public DbSet<Place> Places => Set<Place>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SubscriptionItem> SubscriptionItems => Set<SubscriptionItem>();
    public DbSet<History> Histories => Set<History>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Конфигурация для Subscription
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(s => s.Id);

            // Связь с User
            entity.HasOne(s => s.User)
                  .WithMany(u => u.Subscriptions)
                  .HasForeignKey(s => s.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Связь с Application
            entity.HasOne(s => s.Application)
                  .WithMany(a => a.Subscriptions)
                  .HasForeignKey(s => s.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Конфигурация для SubscriptionItem
        modelBuilder.Entity<SubscriptionItem>(entity =>
        {
            entity.HasKey(si => si.Id);

            // Связь с Subscription
            entity.HasOne(si => si.Subscription)
                  .WithMany(s => s.SubscriptionItems)
                  .HasForeignKey(si => si.SubscriptionId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Связь с Place
            entity.HasOne(si => si.Place)
                  .WithMany(p => p.SubscriptionItems)
                  .HasForeignKey(si => si.PlaceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Конфигурация для History
        modelBuilder.Entity<History>(entity =>
        {
            entity.HasKey(h => h.Id);

            // Связь с Subscription
            entity.HasOne(h => h.Subscription)
                  .WithMany(s => s.Histories)
                  .HasForeignKey(h => h.SubscriptionId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Связь с Place
            entity.HasOne(h => h.Place)
                  .WithMany(p => p.Histories)
                  .HasForeignKey(h => h.PlaceId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Связь с Application
            entity.HasOne(h => h.Application)
                  .WithMany(a => a.Histories)
                  .HasForeignKey(h => h.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Конфигурация для Invoice
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(i => i.Id);

            // Связь с User
            entity.HasOne(i => i.User)
                  .WithMany(u => u.Invoices)
                  .HasForeignKey(i => i.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Конфигурация для InvoiceItem
        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(ii => ii.Id);

            // Связь с Invoice
            entity.HasOne(ii => ii.Invoice)
                  .WithMany(i => i.InvoiceItems)
                  .HasForeignKey(ii => ii.InvoiceId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Связь с Application
            entity.HasOne(ii => ii.Application)
                  .WithMany(a => a.InvoiceItems)
                  .HasForeignKey(ii => ii.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Связь с Place
            entity.HasOne(ii => ii.Place)
                  .WithMany(p => p.InvoiceItems)
                  .HasForeignKey(ii => ii.PlaceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}