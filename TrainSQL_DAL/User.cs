namespace TrainSQL_DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Progresses = new HashSet<Progress>();
        }

        [Key]
        [StringLength(255)]
        public string Login { get; set; }

        [Required]
        [StringLength(255)]
        public string UserEmail { get; set; }

        [Required]
        [StringLength(32)]
        public string Password { get; set; }

        public int RoleID { get; set; }

        public int? ThemeID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Progress> Progresses { get; set; }

        public virtual Role Role { get; set; }

        public virtual Theme Theme { get; set; }
    }
}
