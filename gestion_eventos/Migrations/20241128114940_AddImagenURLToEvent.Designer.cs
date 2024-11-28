﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using gestion_eventos.Data;

#nullable disable

namespace gestion_eventos.Migrations
{
    [DbContext(typeof(GestionEventosContext))]
    [Migration("20241128114940_AddImagenURLToEvent")]
    partial class AddImagenURLToEvent
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("gestion_eventos.Models.Attendance", b =>
                {
                    b.Property<int>("AttendanceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AttendanceId"));

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<bool>("IsPresent")
                        .HasColumnType("bit");

                    b.Property<int>("PersonId")
                        .HasColumnType("int");

                    b.HasKey("AttendanceId")
                        .HasName("PK__Attendan__8B69261CD51B8BDE");

                    b.HasIndex("EventId");

                    b.HasIndex("PersonId");

                    b.ToTable("Attendances");
                });

            modelBuilder.Entity("gestion_eventos.Models.Event", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EventId"));

                    b.Property<int>("AvailableTickets")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("ImagenURL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("EventId")
                        .HasName("PK__Events__7944C8101966D779");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("gestion_eventos.Models.Person", b =>
                {
                    b.Property<int>("PersonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PersonId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("PersonId")
                        .HasName("PK__People__AA2FFBE5204C0919");

                    b.ToTable("People");
                });

            modelBuilder.Entity("gestion_eventos.Models.Schedule", b =>
                {
                    b.Property<int>("ScheduleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ScheduleId"));

                    b.Property<string>("ActivityDescription")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("ScheduleId");

                    b.HasIndex("EventId");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("gestion_eventos.Models.ShoppingCart", b =>
                {
                    b.Property<int>("CartId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CartId"));

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<decimal>("PricePerTicket")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TicketsQuantity")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CartId");

                    b.HasIndex("EventId");

                    b.ToTable("ShoppingCart");
                });

            modelBuilder.Entity("gestion_eventos.Models.Snack", b =>
                {
                    b.Property<int>("SnackId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SnackId"));

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<int>("PersonId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ProvidedOn")
                        .HasColumnType("datetime");

                    b.Property<string>("SnackType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("SnackId")
                        .HasName("PK__Snacks__320A85CBF00320E5");

                    b.HasIndex("EventId");

                    b.HasIndex("PersonId");

                    b.ToTable("Snacks");
                });

            modelBuilder.Entity("gestion_eventos.Models.Ticket", b =>
                {
                    b.Property<int>("TicketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TicketId"));

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<int>("PersonId")
                        .HasColumnType("int");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("datetime");

                    b.HasKey("TicketId")
                        .HasName("PK__Tickets__712CC607F1AB71ED");

                    b.HasIndex("EventId");

                    b.HasIndex("PersonId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("gestion_eventos.Models.TicketPurchase", b =>
                {
                    b.Property<int>("PurchaseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PurchaseId"));

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TicketsBought")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PurchaseId");

                    b.HasIndex("EventId");

                    b.ToTable("TicketPurchases");
                });

            modelBuilder.Entity("gestion_eventos.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("UserId")
                        .HasName("PK_Users");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("gestion_eventos.Models.Attendance", b =>
                {
                    b.HasOne("gestion_eventos.Models.Event", "Event")
                        .WithMany("Attendances")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Attendanc__Event__52593CB8");

                    b.HasOne("gestion_eventos.Models.Person", "Person")
                        .WithMany("Attendances")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Attendanc__Perso__5165187F");

                    b.Navigation("Event");

                    b.Navigation("Person");
                });

            modelBuilder.Entity("gestion_eventos.Models.Schedule", b =>
                {
                    b.HasOne("gestion_eventos.Models.Event", "Event")
                        .WithMany("Schedules")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");
                });

            modelBuilder.Entity("gestion_eventos.Models.ShoppingCart", b =>
                {
                    b.HasOne("gestion_eventos.Models.Event", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");
                });

            modelBuilder.Entity("gestion_eventos.Models.Snack", b =>
                {
                    b.HasOne("gestion_eventos.Models.Event", "Event")
                        .WithMany("Snacks")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Snacks__EventId__5629CD9C");

                    b.HasOne("gestion_eventos.Models.Person", "Person")
                        .WithMany("Snacks")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Snacks__PersonId__5535A963");

                    b.Navigation("Event");

                    b.Navigation("Person");
                });

            modelBuilder.Entity("gestion_eventos.Models.Ticket", b =>
                {
                    b.HasOne("gestion_eventos.Models.Event", "Event")
                        .WithMany("Tickets")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("gestion_eventos.Models.Person", "Person")
                        .WithMany("Tickets")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Tickets__PersonI__4D94879B");

                    b.Navigation("Event");

                    b.Navigation("Person");
                });

            modelBuilder.Entity("gestion_eventos.Models.TicketPurchase", b =>
                {
                    b.HasOne("gestion_eventos.Models.Event", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");
                });

            modelBuilder.Entity("gestion_eventos.Models.Event", b =>
                {
                    b.Navigation("Attendances");

                    b.Navigation("Schedules");

                    b.Navigation("Snacks");

                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("gestion_eventos.Models.Person", b =>
                {
                    b.Navigation("Attendances");

                    b.Navigation("Snacks");

                    b.Navigation("Tickets");
                });
#pragma warning restore 612, 618
        }
    }
}
