using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Barker.Migrations
{
    public partial class UpdateBarkerPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarkerPost_AspNetUsers_AuthorId",
                table: "BarkerPost");

            migrationBuilder.DropIndex(
                name: "IX_BarkerPost_AuthorId",
                table: "BarkerPost");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "BarkerPost");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "BarkerPost",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BarkerPost",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BarkerPost_UserId",
                table: "BarkerPost",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BarkerPost_AspNetUsers_UserId",
                table: "BarkerPost",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarkerPost_AspNetUsers_UserId",
                table: "BarkerPost");

            migrationBuilder.DropIndex(
                name: "IX_BarkerPost_UserId",
                table: "BarkerPost");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "BarkerPost");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BarkerPost");

            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "BarkerPost",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BarkerPost_AuthorId",
                table: "BarkerPost",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_BarkerPost_AspNetUsers_AuthorId",
                table: "BarkerPost",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
