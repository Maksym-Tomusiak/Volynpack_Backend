using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageFittings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_overlay_url",
                table: "package_types");

            migrationBuilder.CreateTable(
                name: "package_fittings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    material_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fitting_image_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_package_fittings", x => x.id);
                    table.ForeignKey(
                        name: "fk_package_fittings_package_materials_material_id",
                        column: x => x.material_id,
                        principalTable: "package_materials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_package_fittings_package_types_type_id",
                        column: x => x.type_id,
                        principalTable: "package_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_package_fittings_material_id",
                table: "package_fittings",
                column: "material_id");

            migrationBuilder.CreateIndex(
                name: "ix_package_fittings_type_id",
                table: "package_fittings",
                column: "type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "package_fittings");

            migrationBuilder.AddColumn<string>(
                name: "image_overlay_url",
                table: "package_types",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
