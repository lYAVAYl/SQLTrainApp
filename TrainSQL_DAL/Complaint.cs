namespace TrainSQL_DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Complaint
    {
        public int ComplaintID { get; set; }

        [Required]
        [StringLength(255)]
        public string Login { get; set; }

        public int ThemeID { get; set; }

        public int TaskID { get; set; }

        [Required]
        [StringLength(255)]
        public string Comment { get; set; }

        public virtual Task Task { get; set; }
    }
}
