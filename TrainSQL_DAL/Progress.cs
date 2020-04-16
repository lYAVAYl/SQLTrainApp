namespace TrainSQL_DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Progress")]
    public partial class Progress
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(30)]
        public string Login { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime TestDate { get; set; }

        public int RightAnswersQuantity { get; set; }

        public int TotalQuastionsQuantity { get; set; }

        public virtual User User { get; set; }
    }
}
