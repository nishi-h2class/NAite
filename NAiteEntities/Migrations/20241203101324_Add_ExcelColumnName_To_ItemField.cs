using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NAiteEntities.Migrations
{
    /// <inheritdoc />
    public partial class Add_ExcelColumnName_To_ItemField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExcelColumnName",
                table: "ItemFields",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcelColumnName",
                table: "ItemFields");
        }
    }
}
