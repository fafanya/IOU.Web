using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Fund.Data;

namespace Fund.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("Fund.Models.UBill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Amount");

                    b.Property<int>("UEventId");

                    b.Property<int>("UMemberId");

                    b.HasKey("Id");

                    b.HasIndex("UEventId");

                    b.HasIndex("UMemberId");

                    b.ToTable("UBills");
                });

            modelBuilder.Entity("Fund.Models.UDebt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Amount");

                    b.Property<int>("DebtorId");

                    b.Property<int>("LenderId");

                    b.Property<string>("Name");

                    b.Property<int>("UGroupId");

                    b.HasKey("Id");

                    b.HasIndex("DebtorId");

                    b.HasIndex("LenderId");

                    b.HasIndex("UGroupId");

                    b.ToTable("UDebts");
                });

            modelBuilder.Entity("Fund.Models.UEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("UEventTypeId");

                    b.Property<int?>("UGroupId");

                    b.HasKey("Id");

                    b.HasIndex("UEventTypeId");

                    b.HasIndex("UGroupId");

                    b.ToTable("UEvents");
                });

            modelBuilder.Entity("Fund.Models.UEventType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("UEventTypes");
                });

            modelBuilder.Entity("Fund.Models.UGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.Property<string>("UUserId");

                    b.HasKey("Id");

                    b.HasIndex("UUserId");

                    b.ToTable("UGroups");
                });

            modelBuilder.Entity("Fund.Models.UMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int?>("UEventId");

                    b.Property<int?>("UGroupId");

                    b.Property<string>("UUserId");

                    b.HasKey("Id");

                    b.HasIndex("UEventId");

                    b.HasIndex("UGroupId");

                    b.HasIndex("UUserId");

                    b.ToTable("UMembers");
                });

            modelBuilder.Entity("Fund.Models.UPayment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Amount");

                    b.Property<int>("UEventId");

                    b.Property<int>("UMemberId");

                    b.HasKey("Id");

                    b.HasIndex("UEventId");

                    b.HasIndex("UMemberId");

                    b.ToTable("UPayments");
                });

            modelBuilder.Entity("Fund.Models.UUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Fund.Models.UBill", b =>
                {
                    b.HasOne("Fund.Models.UEvent", "UEvent")
                        .WithMany("UBills")
                        .HasForeignKey("UEventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Fund.Models.UMember", "UMember")
                        .WithMany("UBills")
                        .HasForeignKey("UMemberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Fund.Models.UDebt", b =>
                {
                    b.HasOne("Fund.Models.UMember", "Debtor")
                        .WithMany("DebtorUDebts")
                        .HasForeignKey("DebtorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Fund.Models.UMember", "Lender")
                        .WithMany("LenderUDebts")
                        .HasForeignKey("LenderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Fund.Models.UGroup", "UGroup")
                        .WithMany("UDebts")
                        .HasForeignKey("UGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Fund.Models.UEvent", b =>
                {
                    b.HasOne("Fund.Models.UEventType", "UEventType")
                        .WithMany("UEvents")
                        .HasForeignKey("UEventTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Fund.Models.UGroup", "UGroup")
                        .WithMany("UEvents")
                        .HasForeignKey("UGroupId");
                });

            modelBuilder.Entity("Fund.Models.UGroup", b =>
                {
                    b.HasOne("Fund.Models.UUser", "UUser")
                        .WithMany("UGroups")
                        .HasForeignKey("UUserId");
                });

            modelBuilder.Entity("Fund.Models.UMember", b =>
                {
                    b.HasOne("Fund.Models.UEvent")
                        .WithMany("UMembers")
                        .HasForeignKey("UEventId");

                    b.HasOne("Fund.Models.UGroup", "UGroup")
                        .WithMany("UMembers")
                        .HasForeignKey("UGroupId");

                    b.HasOne("Fund.Models.UUser", "UUser")
                        .WithMany("UMembers")
                        .HasForeignKey("UUserId");
                });

            modelBuilder.Entity("Fund.Models.UPayment", b =>
                {
                    b.HasOne("Fund.Models.UEvent", "UEvent")
                        .WithMany("UPayments")
                        .HasForeignKey("UEventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Fund.Models.UMember", "UMember")
                        .WithMany("UPayments")
                        .HasForeignKey("UMemberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Fund.Models.UUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Fund.Models.UUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Fund.Models.UUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
