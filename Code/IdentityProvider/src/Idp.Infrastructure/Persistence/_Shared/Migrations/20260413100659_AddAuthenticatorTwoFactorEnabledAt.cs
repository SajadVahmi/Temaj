using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Idp.Infrastructure.Persistence._Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticatorTwoFactorEnabledAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AuthenticatorTwoFactorEnabledAt",
                table: "AspNetUsers",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthenticatorTwoFactorEnabledAt",
                table: "AspNetUsers");
        }
    }
}
