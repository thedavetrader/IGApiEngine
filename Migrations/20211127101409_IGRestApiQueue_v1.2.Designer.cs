﻿// <auto-generated />
using System;
using IGApi.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IGApiEngine.Migrations
{
    [DbContext(typeof(IGApiDbContext))]
    [Migration("20211127101409_IGRestApiQueue_v1.2")]
    partial class IGRestApiQueue_v12
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("IGApi.Model.IGApiAccountBalance", b =>
                {
                    b.Property<string>("AccountId")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<decimal?>("available")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)");

                    b.Property<decimal?>("balance")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)");

                    b.Property<decimal?>("deposit")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)");

                    b.Property<decimal?>("profitLoss")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)");

                    b.HasKey("AccountId");

                    b.ToTable("AccountBalance", (string)null);
                });

            modelBuilder.Entity("IGApi.Model.IGApiAccountDetails", b =>
                {
                    b.Property<string>("accountId")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("accountAlias")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("accountName")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("accountType")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<bool>("canTransferFrom")
                        .HasColumnType("bit");

                    b.Property<bool>("canTransferTo")
                        .HasColumnType("bit");

                    b.Property<string>("currency")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<bool>("preferred")
                        .HasColumnType("bit");

                    b.Property<string>("status")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.HasKey("accountId");

                    b.ToTable("AccountDetails", (string)null);
                });

            modelBuilder.Entity("IGApi.Model.IGApiStreamingAccountData", b =>
                {
                    b.Property<string>("AccountId")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<decimal?>("AmountDue")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)");

                    b.Property<decimal?>("AvailableCash")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)");

                    b.Property<decimal?>("Balance")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)");

                    b.Property<decimal?>("Deposit")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)");

                    b.Property<decimal?>("Equity")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)");

                    b.Property<decimal?>("EquityUsed")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)");

                    b.Property<decimal?>("ProfitAndLoss")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)");

                    b.Property<decimal?>("UsedMargin")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)");

                    b.HasKey("AccountId");

                    b.ToTable("StreamingAccountData", (string)null);
                });

            modelBuilder.Entity("IGApi.Model.IGRestApiQueue", b =>
                {
                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("RestRequest")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<bool>("ExecuteImmediately")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Parameters")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.HasKey("Timestamp", "RestRequest");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("Timestamp", "RestRequest"), false);

                    b.HasIndex("ExecuteImmediately", "Timestamp");

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex("ExecuteImmediately", "Timestamp"), false);

                    b.ToTable("IGRestApiQueue");

                    SqlServerEntityTypeBuilderExtensions.IsMemoryOptimized(b);
                });
#pragma warning restore 612, 618
        }
    }
}
