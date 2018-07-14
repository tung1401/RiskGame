//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RiskGame.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserGameRisk
    {
        public int UserGameRiskId { get; set; }
        public int RiskId { get; set; }
        public int RiskOptionId { get; set; }
        public int UserId { get; set; }
        public int GameRoomId { get; set; }
        public Nullable<int> Turn { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> CreateBy { get; set; }
    
        public virtual GameRoom GameRoom { get; set; }
        public virtual Risk Risk { get; set; }
        public virtual RiskOption RiskOption { get; set; }
        public virtual User User { get; set; }
    }
}