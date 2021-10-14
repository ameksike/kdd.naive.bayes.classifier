using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Classifier.Models.ORM
{
    public class TopicWord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Required(ErrorMessage = "This field is required...")]
        [DisplayName("topic")]
        public string topic { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Required(ErrorMessage = "This field is required...")]
        [DisplayName("word")]
        public string word { get; set; }
        public int count { get; set; }
    }
}
