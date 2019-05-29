namespace obiektowe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix_user : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Orders", name: "Author_sub", newName: "AuthorSub");
            RenameIndex(table: "dbo.Orders", name: "IX_Author_sub", newName: "IX_AuthorSub");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Orders", name: "IX_AuthorSub", newName: "IX_Author_sub");
            RenameColumn(table: "dbo.Orders", name: "AuthorSub", newName: "Author_sub");
        }
    }
}
