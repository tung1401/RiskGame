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
    
    public partial class Risk
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Risk()
        {
            this.RiskOptions = new HashSet<RiskOption>();
            this.UserGameRisks = new HashSet<UserGameRisk>();
            this.GameBattles = new HashSet<GameBattle>();
        }
    
        public int RiskId { get; set; }
        public string RiskName { get; set; }
        public string RiskDetail { get; set; }
        public Nullable<int> RiskType { get; set; }
        public Nullable<int> ExpertSuggestion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RiskOption> RiskOptions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserGameRisk> UserGameRisks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GameBattle> GameBattles { get; set; }
    }
}
