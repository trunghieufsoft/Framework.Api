namespace Common.DTOs.Common
{
    public class DataInput<TData>
    {
        public DataInput() { }

        public DataInput(TData data, string currentUser) {
            CurrentUser = currentUser;
            Dto = data;
        }

        public string CurrentUser { get; set; }

        public TData Dto { get; set; }
    }

    public class DataInput<TData, TProperties>
    {
        public DataInput() { }

        public DataInput(TProperties properties, TData data, string currentUser)
        {
            Properties = properties;
            Dto = data;
            CurrentUser = currentUser;
        }

        public string CurrentUser { get; set; }

        public TData Dto { get; set; }

        public TProperties Properties { get; set; }
    }

    public class DataInput
    {
        public DataInput() { }

        public DataInput(dynamic dynamicData, string currentUser)
        {
            CurrentUser = currentUser;
            Dto = dynamicData;
        }

        public string CurrentUser { get; set; }

        public dynamic Dto { get; set; }
    }
}
