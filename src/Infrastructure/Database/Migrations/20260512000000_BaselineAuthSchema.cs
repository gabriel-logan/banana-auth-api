using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Banana.Auth.Api.Infrastructure.Database.Migrations;

[DbContext(typeof(AuthDbContext))]
[Migration("20260512000000_BaselineAuthSchema")]
public partial class BaselineAuthSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE TABLE IF NOT EXISTS "Users" (
                "Id" uuid NOT NULL,
                "Email" text NOT NULL,
                "PasswordHash" text NOT NULL,
                "RefreshTokenHash" text NULL,
                "RefreshTokenExpiresAt" timestamp with time zone NULL,
                "RefreshTokenCreatedAt" timestamp with time zone NULL,
                "CreatedAt" timestamp with time zone NOT NULL,
                "UpdatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
                CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
            );
            """);

        migrationBuilder.Sql("""
            ALTER TABLE "Users"
            ADD COLUMN IF NOT EXISTS "RefreshTokenHash" text NULL;
            """);

        migrationBuilder.Sql("""
            ALTER TABLE "Users"
            ADD COLUMN IF NOT EXISTS "RefreshTokenExpiresAt" timestamp with time zone NULL;
            """);

        migrationBuilder.Sql("""
            ALTER TABLE "Users"
            ADD COLUMN IF NOT EXISTS "RefreshTokenCreatedAt" timestamp with time zone NULL;
            """);

        migrationBuilder.Sql("""
            ALTER TABLE "Users"
            ADD COLUMN IF NOT EXISTS "UpdatedAt" timestamp with time zone NULL;
            """);

        migrationBuilder.Sql("""
            UPDATE "Users"
            SET "UpdatedAt" = COALESCE("UpdatedAt", "CreatedAt", NOW())
            WHERE "UpdatedAt" IS NULL;
            """);

        migrationBuilder.Sql("""
            ALTER TABLE "Users"
            ALTER COLUMN "UpdatedAt" SET NOT NULL;
            """);

        migrationBuilder.Sql("""
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email"
            ON "Users" ("Email");
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DROP INDEX IF EXISTS "IX_Users_Email";
            """);

        migrationBuilder.Sql("""
            DROP TABLE IF EXISTS "Users";
            """);
    }
}
