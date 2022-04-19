namespace ECommerceLiteDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductPicturesTableUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Order Details", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Products", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Categories", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductPictures", "Picture", c => c.String(maxLength: 400));
            AddColumn("dbo.ProductPictures", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetRoles", "IsDeleted", c => c.Boolean());
            DropColumn("dbo.ProductPictures", "ProductPicture1");
            DropColumn("dbo.ProductPictures", "ProductPicture2");
            DropColumn("dbo.ProductPictures", "ProductPicture3");
            DropColumn("dbo.ProductPictures", "ProductPicture4");
            DropColumn("dbo.ProductPictures", "ProductPicture5");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductPictures", "ProductPicture5", c => c.String(maxLength: 400));
            AddColumn("dbo.ProductPictures", "ProductPicture4", c => c.String(maxLength: 400));
            AddColumn("dbo.ProductPictures", "ProductPicture3", c => c.String(maxLength: 400));
            AddColumn("dbo.ProductPictures", "ProductPicture2", c => c.String(maxLength: 400));
            AddColumn("dbo.ProductPictures", "ProductPicture1", c => c.String(maxLength: 400));
            DropColumn("dbo.AspNetRoles", "IsDeleted");
            DropColumn("dbo.ProductPictures", "IsDeleted");
            DropColumn("dbo.ProductPictures", "Picture");
            DropColumn("dbo.Categories", "IsDeleted");
            DropColumn("dbo.Products", "IsDeleted");
            DropColumn("dbo.Order Details", "IsDeleted");
            DropColumn("dbo.Orders", "IsDeleted");
            DropColumn("dbo.AspNetUsers", "IsDeleted");
        }
    }
}
