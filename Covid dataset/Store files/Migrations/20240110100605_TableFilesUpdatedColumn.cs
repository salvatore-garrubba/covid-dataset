using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store_files.Migrations
{
    /// <inheritdoc />
    public partial class TableFilesUpdatedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "File",
                table: "Files",
                newName: "FileBytes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileBytes",
                table: "Files",
                newName: "File");
        }
    }
}
