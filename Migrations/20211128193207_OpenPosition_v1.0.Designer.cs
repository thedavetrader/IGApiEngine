﻿// <auto-generated />
using System;
using IGApi.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IGApi.Migrations
{
    [DbContext(typeof(IGApiDbContext))]
    [Migration("20211128193207_OpenPosition_v1.0")]
    partial class OpenPosition_v10
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("IGApiEngine.Model.Account", b =>
                {
                    b.Property<string>("AccountId")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)")
                        .HasColumnName("account_id");

                    b.Property<string>("AccountAlias")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)")
                        .HasColumnName("account_alias");

                    b.Property<string>("AccountName")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)")
                        .HasColumnName("account_name");

                    b.Property<string>("AccountType")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)")
                        .HasColumnName("account_type");

                    b.Property<decimal?>("AmountDue")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)")
                        .HasColumnName("amount_due");

                    b.Property<DateTime>("ApiLastUpdate")
                        .HasColumnType("datetime2")
                        .HasColumnName("api_last_update");

                    b.Property<decimal?>("AvailableCash")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)")
                        .HasColumnName("available_cash");

                    b.Property<decimal?>("Balance")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)")
                        .HasColumnName("balance");

                    b.Property<bool?>("CanTransferFrom")
                        .HasColumnType("bit")
                        .HasColumnName("can_transfer_from");

                    b.Property<bool?>("CanTransferTo")
                        .HasColumnType("bit")
                        .HasColumnName("can_transfer_to");

                    b.Property<string>("Currency")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)")
                        .HasColumnName("currency");

                    b.Property<decimal?>("Deposit")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)")
                        .HasColumnName("deposit");

                    b.Property<decimal?>("Equity")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)")
                        .HasColumnName("equity");

                    b.Property<decimal?>("EquityUsed")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)")
                        .HasColumnName("equity_used");

                    b.Property<bool?>("Preferred")
                        .HasColumnType("bit")
                        .HasColumnName("preferred");

                    b.Property<decimal?>("ProfitAndLoss")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)")
                        .HasColumnName("profit_and_loss");

                    b.Property<string>("Status")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)")
                        .HasColumnName("status");

                    b.Property<decimal?>("UsedMargin")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)")
                        .HasColumnName("used_margin");

                    b.HasKey("AccountId");

                    b.ToTable("account");
                });

            modelBuilder.Entity("IGApiEngine.Model.IGRestRequestQueue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("ExecuteAsap")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("execute_asap");

                    b.Property<bool>("IsRecurrent")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("is_recurrent");

                    b.Property<string>("Parameters")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)")
                        .HasColumnName("parameter");

                    b.Property<string>("RestRequest")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)")
                        .HasColumnName("rest_request");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("timestamp")
                        .HasDefaultValueSql("getutcdate()");

                    b.HasKey("Id");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("Id"), false);

                    b.HasIndex("ExecuteAsap", "Timestamp");

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex("ExecuteAsap", "Timestamp"), false);

                    b.ToTable("rest_request_queue");

                    SqlServerEntityTypeBuilderExtensions.IsMemoryOptimized(b);

                    b.HasCheckConstraint("CHK_ig_rest_api_queue_rest_request", "rest_request in ('GetAccountDetails', 'GetOpenPositions', 'CreatePosition')");
                });

            modelBuilder.Entity("IGApiEngine.Model.OpenPosition", b =>
                {
                    b.Property<string>("AccountId")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)")
                        .HasColumnName("account_id");

                    b.Property<string>("DealId")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)")
                        .HasColumnName("deal_id");

                    b.Property<decimal?>("ContractSize")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)")
                        .HasColumnName("contract_size");

                    b.Property<bool>("ControlledRisk")
                        .HasColumnType("bit")
                        .HasColumnName("controlled_risk");

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("created_date_utc");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)")
                        .HasColumnName("currency");

                    b.Property<string>("Direction")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)")
                        .HasColumnName("direction");

                    b.Property<decimal?>("Level")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)")
                        .HasColumnName("level");

                    b.Property<decimal?>("LimitLevel")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)")
                        .HasColumnName("limit_level");

                    b.Property<decimal?>("Size")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)")
                        .HasColumnName("size");

                    b.Property<decimal?>("StopLevel")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)")
                        .HasColumnName("stop_level");

                    b.Property<decimal?>("TrailingStep")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)")
                        .HasColumnName("trailing_step");

                    b.Property<decimal?>("TrailingStopDistance")
                        .HasPrecision(38, 19)
                        .HasColumnType("decimal(38,19)")
                        .HasColumnName("trailing_stop_distance");

                    b.HasKey("AccountId", "DealId");

                    b.ToTable("open_position");
                });
#pragma warning restore 612, 618
        }
    }
}
