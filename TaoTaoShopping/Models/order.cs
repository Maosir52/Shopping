//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace TaoTaoShopping.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public order()
        {
            this.order_detail = new HashSet<order_detail>();
        }
    
        public int id { get; set; }
        public Nullable<int> uid { get; set; }
        public string order_num { get; set; }
        public Nullable<decimal> sum_price { get; set; }
        public string mark { get; set; }
        public Nullable<System.DateTime> createtime { get; set; }
        public Nullable<short> is_pay { get; set; }
        public Nullable<short> state { get; set; }
        public string pay_way { get; set; }
        public Nullable<int> address_id { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<order_detail> order_detail { get; set; }
        public virtual user user { get; set; }
    }
}
