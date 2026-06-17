using System.Text.Json;

namespace LTWeb.Infrastructure
{
    public static class SessionExtensions
    {
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Đọc dữ liệu từ Session và chuyển đổi ngược lại thành Object
        public static T GetJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (value == null) return default;

            try
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            catch (JsonException)
            {
                // Nếu dữ liệu JSON cũ bị lỗi, xóa nó đi và trả về mặc định
                session.Remove(key);
                return default;
            }
        }
    }
}
