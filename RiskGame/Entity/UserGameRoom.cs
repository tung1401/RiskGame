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
    
    public partial class UserGameRoom
    {
        public int UserGameRoomId { get; set; }
        public int GameRoomId { get; set; }
        public int UserId { get; set; }
        public string PlayerName { get; set; }
        public Nullable<int> JobType { get; set; }
        public int MoneyValue { get; set; }
        public int ProjectValue { get; set; }
        public int TeamValue { get; set; }
        public Nullable<System.DateTime> JoinDate { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<bool> GameFinished { get; set; }
        public Nullable<int> TurnValue { get; set; }
        public string ImageUrl { get; set; }
        public Nullable<bool> IsBot { get; set; }
    
        public virtual GameRoom GameRoom { get; set; }
        public virtual User User { get; set; }
    }
}
