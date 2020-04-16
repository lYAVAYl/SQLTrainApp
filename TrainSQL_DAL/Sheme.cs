namespace TrainSQL_DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Sheme
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sheme()
        {
            TestDatabases = new HashSet<TestDatabas>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ShemeID { get; set; }

        [Column("Sheme")]
        public byte[] Sheme1 { get; set; }

        [Required]
        [StringLength(2048)]
        public string Info { get; set; }

        [Required]
        [StringLength(128)]
        public string ShemeName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TestDatabas> TestDatabases { get; set; }
    }
}
