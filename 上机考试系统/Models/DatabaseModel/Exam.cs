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
    
    public partial class Exam
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string time { get; set; }
        public int creatorId { get; set; }
        public string test_upload { get; set; }
        public Nullable<int> commmit_number { get; set; }
        public string is_being { get; set; }
        public string has_saved { get; set; }
        public string has_cleaned { get; set; }
        public string creator { get; set; }
        public string has_stopped { get; set; }
        public string PaperPath { get; set; }
        public string AnswerPath { get; set; }
    }
}
