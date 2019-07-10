using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TaskManager.Migrations
{
    public partial class smareterconnect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SomeDatas",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    data1 = table.Column<string>(nullable: true),
                    data2 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SomeDatas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_role = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserApps",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    date_app = table.Column<DateTime>(type: "datetime", nullable: false),
                    date_java = table.Column<DateTime>(type: "datetime", nullable: true),
                    email = table.Column<string>(maxLength: 100, nullable: false),
                    emailconfirmed = table.Column<bool>(nullable: false),
                    first_name = table.Column<string>(maxLength: 100, nullable: false),
                    group_status = table.Column<bool>(nullable: true),
                    id_java = table.Column<long>(nullable: true),
                    id_role = table.Column<int>(nullable: false),
                    last_name = table.Column<string>(maxLength: 100, nullable: false),
                    upassword = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserApps", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserApps_UserRoles_id_role",
                        column: x => x.id_role,
                        principalTable: "UserRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoardTasks",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    background_image = table.Column<string>(nullable: true),
                    content_task = table.Column<string>(nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime", nullable: false),
                    date_finished = table.Column<DateTime>(type: "datetime", nullable: false),
                    id_owner = table.Column<long>(nullable: false),
                    name_task = table.Column<string>(maxLength: 100, nullable: false),
                    task_created = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardTasks", x => x.id);
                    table.ForeignKey(
                        name: "FK_BoardTasks_UserApps_id_owner",
                        column: x => x.id_owner,
                        principalTable: "UserApps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StageTasks",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    background_image = table.Column<string>(nullable: true),
                    content_stage = table.Column<string>(nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime", nullable: false),
                    date_finished = table.Column<DateTime>(type: "datetime", nullable: false),
                    id_owner = table.Column<long>(nullable: false),
                    name_stage = table.Column<string>(maxLength: 100, nullable: true),
                    stage_created = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageTasks", x => x.id);
                    table.ForeignKey(
                        name: "FK_StageTasks_BoardTasks_id_owner",
                        column: x => x.id_owner,
                        principalTable: "BoardTasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NodeStages",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    path_file = table.Column<string>(nullable: true),
                    content_node = table.Column<string>(nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime", nullable: false),
                    date_finished = table.Column<DateTime>(type: "datetime", nullable: false),
                    id_owner = table.Column<long>(nullable: false),
                    node_created = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeStages", x => x.id);
                    table.ForeignKey(
                        name: "FK_NodeStages_StageTasks_id_owner",
                        column: x => x.id_owner,
                        principalTable: "StageTasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardTasks_id_owner",
                table: "BoardTasks",
                column: "id_owner");

            migrationBuilder.CreateIndex(
                name: "IX_NodeStages_id_owner",
                table: "NodeStages",
                column: "id_owner");

            migrationBuilder.CreateIndex(
                name: "IX_StageTasks_id_owner",
                table: "StageTasks",
                column: "id_owner");

            migrationBuilder.CreateIndex(
                name: "IX_UserApps_id_role",
                table: "UserApps",
                column: "id_role");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NodeStages");

            migrationBuilder.DropTable(
                name: "SomeDatas");

            migrationBuilder.DropTable(
                name: "StageTasks");

            migrationBuilder.DropTable(
                name: "BoardTasks");

            migrationBuilder.DropTable(
                name: "UserApps");

            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}
