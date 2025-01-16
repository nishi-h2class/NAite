using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NAiteEntities.Migrations
{
    /// <inheritdoc />
    public partial class Add_IsCode_To_ItemField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCode",
                table: "ItemFields",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCode",
                table: "ItemFields");
        }
    }
}
