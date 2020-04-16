namespace TrainSQL_DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Complaint
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ComplaintID { get; set; }

        [Required]
        [StringLength(30)]
        public string Login { get; set; }

        public int TaskID { get; set; }

        public string Comment { get; set; }

        public virtual Task Task { get; set; }

        public virtual User User { get; set; }
    }
}
