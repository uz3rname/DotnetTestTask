using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetTestTask.Migrations
{
    /// <inheritdoc />
    public partial class Triggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(File.ReadAllText("./Sql/CreateTriggers.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(File.ReadAllText("./Sql/RemoveTriggers.sql"));
        }
    }
}
