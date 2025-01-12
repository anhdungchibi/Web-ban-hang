namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tableimport : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_Import",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SupplierName = c.String(),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        SupplierId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        Modifiedby = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tb_Supplier", t => t.SupplierId, cascadeDelete: true)
                .Index(t => t.SupplierId);
            
            CreateTable(
                "dbo.tb_ImportDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImportId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Int(nullable: false),
                        CalculationUnit = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tb_Import", t => t.ImportId, cascadeDelete: true)
                .ForeignKey("dbo.tb_Product", t => t.ProductId, cascadeDelete: false)
                .Index(t => t.ImportId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_Import", "SupplierId", "dbo.tb_Supplier");
            DropForeignKey("dbo.tb_ImportDetail", "ProductId", "dbo.tb_Product");
            DropForeignKey("dbo.tb_ImportDetail", "ImportId", "dbo.tb_Import");
            DropIndex("dbo.tb_ImportDetail", new[] { "ProductId" });
            DropIndex("dbo.tb_ImportDetail", new[] { "ImportId" });
            DropIndex("dbo.tb_Import", new[] { "SupplierId" });
            DropTable("dbo.tb_ImportDetail");
            DropTable("dbo.tb_Import");
        }
    }
}
