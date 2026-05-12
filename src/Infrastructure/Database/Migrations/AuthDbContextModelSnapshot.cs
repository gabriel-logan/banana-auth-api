using System;
using Banana.Auth.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Banana.Auth.Api.Infrastructure.Database.Migrations;

[DbContext(typeof(AuthDbContext))]
partial class AuthDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.0-preview.3.25171.6")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("Banana.Auth.Api.Modules.Users.User", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid");

            b.Property<DateTime>("CreatedAt")
                .HasColumnType("timestamp with time zone");

            b.Property<string>("Email")
                .IsRequired()
                .HasColumnType("text");

            b.Property<string>("PasswordHash")
                .IsRequired()
                .HasColumnType("text");

            b.Property<DateTime?>("RefreshTokenCreatedAt")
                .HasColumnType("timestamp with time zone");

            b.Property<DateTime?>("RefreshTokenExpiresAt")
                .HasColumnType("timestamp with time zone");

            b.Property<string>("RefreshTokenHash")
                .HasColumnType("text");

            b.Property<DateTime>("UpdatedAt")
                .HasColumnType("timestamp with time zone");

            b.HasKey("Id");

            b.HasIndex("Email")
                .IsUnique();

            b.ToTable("Users");
        });
#pragma warning restore 612, 618
    }
}
