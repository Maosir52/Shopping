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
    
    public partial class shop_car
    {
        public int id { get; set; }
        public Nullable<int> uid { get; set; }
        public Nullable<int> shopid { get; set; }
        public Nullable<int> shopNum { get; set; }
        public Nullable<System.DateTime> createtime { get; set; }
    
        public virtual shopping shopping { get; set; }
        public virtual user user { get; set; }
    }
}
