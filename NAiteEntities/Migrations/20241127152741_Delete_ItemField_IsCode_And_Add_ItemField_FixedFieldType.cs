using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NAiteEntities.Migrations
{
    /// <inheritdoc />
    public partial class Delete_ItemField_IsCode_And_Add_ItemField_FixedFieldType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCode",
                table: "ItemFields");

            migrationBuilder.AddColumn<string>(
                name: "FixedFieldType",
                table: "ItemFields",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FixedFieldType",
                table: "ItemFields");

            migrationBuilder.AddColumn<bool>(
                name: "IsCode",
                table: "ItemFields",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
