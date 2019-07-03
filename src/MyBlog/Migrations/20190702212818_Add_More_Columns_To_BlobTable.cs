using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyBlog.Migrations
{
  public partial class Add_More_Columns_To_BlobTable : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<DateTime>(
          name: "CreatedDate",
          table: "Blobs",
          nullable: false,
          defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

      migrationBuilder.AddColumn<string>(
          name: "Name",
          table: "Blobs",
          nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "CreatedDate",
          table: "Blobs");

      migrationBuilder.DropColumn(
          name: "Name",
          table: "Blobs");
    }
  }
}
