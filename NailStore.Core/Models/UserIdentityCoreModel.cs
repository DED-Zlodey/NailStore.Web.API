namespace NailStore.Core.Models
{
    public class UserIdentityCoreModel
    {
        public string Id { get; }
        public string UserName { get; }
        public DateTime RegisterAt { get; }
        public string? Phone { get; }
        public bool Enable { get; }

        private UserIdentityCoreModel(string id, string userName, DateTime registerDate, string? phone, bool enable)
        {
            Id = id;
            UserName = userName;
            RegisterAt = registerDate;
            Phone = phone;
            Enable = enable;
        }

        public static ResponseModelCore CreateUser(string id, string userName, DateTime registerDate, string? phone, bool enable)
        {
            var response = new ResponseModelCore
            {
                Header = new ResponseHeaderCore
                {
                    Error = string.Empty,
                    StatusCode = 200
                }
            };
            if (string.IsNullOrEmpty(id))
            {
                response.Header.Error = $"Поле \"id\" не может быть пустым";
                response.Header.StatusCode = 500;
            }
            if (string.IsNullOrEmpty(userName))
            {
                response.Header.Error = $"Поле \"userName\" не может быть пустым";
                response.Header.StatusCode = 500;
            }
            var user = new UserIdentityCoreModel(id, userName, registerDate, phone, enable);
            return response;
        }
    }
}
