namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtablereviewproduct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_ReviewProduct",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        UserName = c.String(),
                        FullName = c.String(),
                        Email = c.String(),
                        Content = c.String(),
                        Rate = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tb_Product", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_ReviewProduct", "ProductId", "dbo.tb_Product");
            DropIndex("dbo.tb_ReviewProduct", new[] { "ProductId" });
            DropTable("dbo.tb_ReviewProduct");
        }
    }
}
