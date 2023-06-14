namespace WebApplication1.Models
{
    public class MenuItem
    {
        public string Controller { get; set; }
        public string Action{ get; set; }
        public string Label{ get; set; }
        public List<MenuItem> DropdownItems{ get; set; }
    }
}
