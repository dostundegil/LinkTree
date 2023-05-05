using System.ComponentModel.DataAnnotations;

namespace LinkTree.Models
{
    public class Link
    {
        [Key]
        public int LinkID { get; set; }
        public string LinkTitle { get; set; }

        public string LinkUrl { get; set; }

        public AppUser AppUser { get; set; }
    }
}
