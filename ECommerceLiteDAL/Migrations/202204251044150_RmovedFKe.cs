namespace ECommerceLiteDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RmovedFKe : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Categories", "BaseCategoryId", "dbo.Categories");
            DropIndex("dbo.Categories", new[] { "BaseCategoryId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Categories", "BaseCategoryId");
            AddForeignKey("dbo.Categories", "BaseCategoryId", "dbo.Categories", "Id");
        }
    }
}
