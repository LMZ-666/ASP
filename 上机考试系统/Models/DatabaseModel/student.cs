//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace 上机考试系统.Models.DatabaseModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Student
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string ip_address { get; set; }
        public string pwd { get; set; }
        public Nullable<int> exam_Id { get; set; }
    }
}
