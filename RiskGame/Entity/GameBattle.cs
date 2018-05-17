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
    
    public partial class GameBattle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GameBattle()
        {
            this.UserGameBattles = new HashSet<UserGameBattle>();
        }
    
        public int GameBattleId { get; set; }
        public int GameRoomId { get; set; }
        public int RiskId { get; set; }
        public int RiskOptionId { get; set; }
        public Nullable<int> Ratio { get; set; }
        public Nullable<int> Turn { get; set; }
        public Nullable<int> ActionEffectType { get; set; }
        public Nullable<int> ActionEffectValue { get; set; }
    
        public virtual GameRoom GameRoom { get; set; }
        public virtual Risk Risk { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserGameBattle> UserGameBattles { get; set; }
    }
}
