//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Projekt_1.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Transaction
    {
        public int transactionID { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<System.DateTime> transactionDate { get; set; }
        public Nullable<double> transactionAmount { get; set; }
        public string transactionType { get; set; }
    
        public virtual user user { get; set; }
    }
}
