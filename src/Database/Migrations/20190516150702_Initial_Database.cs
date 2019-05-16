using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class Initial_Database : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_LOG_WORK",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Message = table.Column<string>(nullable: true),
                    Level = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Exception = table.Column<string>(nullable: true),
                    LogEvent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_LOG_WORK", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBL_SYS_CONFIG",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    CREATED_USER = table.Column<string>(maxLength: 2048, nullable: true),
                    CREATED_DT = table.Column<DateTime>(nullable: true),
                    LAST_UDT_USER = table.Column<string>(maxLength: 2048, nullable: true),
                    LAST_UDT_DT = table.Column<DateTime>(nullable: true),
                    KEY = table.Column<string>(maxLength: 2048, nullable: true),
                    VALUE = table.Column<string>(maxLength: 2048, nullable: true),
                    VALUE_UNIT = table.Column<string>(maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_SYS_CONFIG", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TBL_USER",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    CREATED_USER = table.Column<string>(maxLength: 2048, nullable: true),
                    CREATED_DT = table.Column<DateTime>(nullable: true),
                    LAST_UDT_USER = table.Column<string>(maxLength: 2048, nullable: true),
                    LAST_UDT_DT = table.Column<DateTime>(nullable: true),
                    LOGIN_FAILED_NR = table.Column<int>(nullable: true),
                    TOKEN = table.Column<string>(maxLength: 2048, nullable: true),
                    SUBCRISE_TOKEN = table.Column<string>(maxLength: 2048, nullable: true),
                    TOKEN_EXPIRED_DT = table.Column<DateTime>(nullable: true),
                    LOGIN_TM = table.Column<DateTime>(nullable: true),
                    PASSWORD = table.Column<string>(maxLength: 1024, nullable: false),
                    PASSWORD_LAST_UDT = table.Column<DateTime>(nullable: true),
                    USERNAME = table.Column<string>(maxLength: 2048, nullable: false),
                    CODE = table.Column<string>(maxLength: 128, nullable: false),
                    FULL_NAME = table.Column<string>(maxLength: 2048, nullable: false),
                    USER_TYP = table.Column<string>(maxLength: 2048, nullable: false),
                    STATUS = table.Column<string>(maxLength: 2048, nullable: false),
                    ADDRESS = table.Column<string>(maxLength: 2048, nullable: false),
                    EMAIL = table.Column<string>(maxLength: 2048, nullable: true),
                    PHONE = table.Column<string>(maxLength: 128, nullable: false),
                    START_DT = table.Column<DateTime>(nullable: true),
                    EXPIRED_DT = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_USER", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_LOG_WORK");

            migrationBuilder.DropTable(
                name: "TBL_SYS_CONFIG");

            migrationBuilder.DropTable(
                name: "TBL_USER");
        }
    }
}
