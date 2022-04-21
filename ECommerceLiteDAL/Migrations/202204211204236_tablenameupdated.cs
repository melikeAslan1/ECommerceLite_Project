namespace ECommerceLiteDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tablenameupdated : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Admin", newName: "Admins");
            RenameTable(name: "dbo.Order Details", newName: "OrderDetails");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.OrderDetails", newName: "Order Details");
            RenameTable(name: "dbo.Admins", newName: "Admin");
        }
    }
}
