using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_Import")]
    public class Import : CommonAbstract
    {
        public Import()
        {
            this.ImportDetails = new HashSet<ImportDetail>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Description { get; set; }
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<ImportDetail> ImportDetails { get; set; }

    }
}