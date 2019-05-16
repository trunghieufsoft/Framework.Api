namespace Common.DTOs.Common
{
    public class DropdownList
    {
        public DropdownList(object id = null, string user = null, string name = null)
        {
            Id = id;
            User = user;
            Name = name;
        }

        public DropdownList(object id = null, string name = null)
        {
            Id = id;
            Name = name;
        }
        public object Id { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
    }
}