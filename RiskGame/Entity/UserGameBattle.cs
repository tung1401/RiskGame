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
    
    public partial class UserGameBattle
    {
        public int UserGameBattleId { get; set; }
        public int GameBattleId { get; set; }
        public int UserId { get; set; }
        public Nullable<int> Turn { get; set; }
        public Nullable<int> Money { get; set; }
        public Nullable<int> Team { get; set; }
        public Nullable<int> Project { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<bool> Active { get; set; }
    
        public virtual User User { get; set; }
        public virtual GameBattle GameBattle { get; set; }
    }
}
