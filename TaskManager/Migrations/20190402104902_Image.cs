using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TaskManager.Migrations
{
    public partial class Image : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image_path",
                table: "UserApps",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "content_stage",
                table: "StageTasks",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_path",
                table: "UserApps");

            migrationBuilder.AlterColumn<string>(
                name: "content_stage",
                table: "StageTasks",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
