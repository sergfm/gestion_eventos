using System;
using System.Collections.Generic;
using gestion_eventos.Models;
using Microsoft.EntityFrameworkCore;

namespace gestion_eventos.Data
{
    public class GestionEventosContext : DbContext
    {
        public GestionEventosContext(DbContextOptions<GestionEventosContext> options) : base(options)
        {
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Snack> Snacks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // La configuración vendrá desde Program.cs y appsettings.json.
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__8B69261CD51B8BDE");

                entity.HasOne(d => d.Event).WithMany(p => p.Attendances)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK__Attendanc__Event__52593CB8");

                entity.HasOne(d => d.Person).WithMany(p => p.Attendances)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK__Attendanc__Perso__5165187F");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.EventId).HasName("PK__Events__7944C8101966D779");

                entity.Property(e => e.Date).HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Location).HasMaxLength(100);
                entity.Property(e => e.Title).HasMaxLength(100);

                entity.HasMany(e => e.Schedules)
                    .WithOne(s => s.Event)
                    .HasForeignKey(s => s.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Tickets)
                    .WithOne(t => t.Event)
                    .HasForeignKey(t => t.EventId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.HasKey(s => s.ScheduleId);
                entity.Property(s => s.ActivityDescription).HasMaxLength(200).IsRequired();
                entity.HasOne(s => s.Event)
                    .WithMany(e => e.Schedules)
                    .HasForeignKey(s => s.EventId);
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.PersonId).HasName("PK__People__AA2FFBE5204C0919");

                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.FullName).HasMaxLength(100);
            });

            modelBuilder.Entity<Snack>(entity =>
            {
                entity.HasKey(e => e.SnackId).HasName("PK__Snacks__320A85CBF00320E5");

                entity.Property(e => e.ProvidedOn).HasColumnType("datetime");
                entity.Property(e => e.SnackType).HasMaxLength(100);

                entity.HasOne(d => d.Event).WithMany(p => p.Snacks)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK__Snacks__EventId__5629CD9C");

                entity.HasOne(d => d.Person).WithMany(p => p.Snacks)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK__Snacks__PersonId__5535A963");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.TicketId).HasName("PK__Tickets__712CC607F1AB71ED");

                entity.Property(e => e.PurchaseDate).HasColumnType("datetime");

                entity.HasOne(d => d.Event).WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Person).WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK__Tickets__PersonI__4D94879B");
            });
        }
    }
}
