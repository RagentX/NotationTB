using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using NotationTB.Data;
using NotationTB.Models;

namespace NotationTB.BusinessLogic;

public class UserLogic : IDisposable
{
    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <param name="login">Логин</param>
    /// <param name="pass">Пароль</param>
    /// <param name="id">Код пользователя, при успешной авторизации не равен нулю</param>
    /// <param name="message">"Сообщение об ошибке"</param>
    /// <returns>Возврощает true при успешной авторизации</returns>
    public bool Authorization(string login, string pass, out int id, out string message)
    {
        id = 0;
        message = string.Empty;
        try
        {
            using (AppDbContext db = new())
            {
                var loginUser =
                    db.Users.Where(u => u.Login == login).First();
                if (loginUser is not null)
                    if (loginUser.Password == sha256_hash(pass))
                    {
                        id = loginUser.Id;
                        return true;
                    }

                message = "Неправильный логин или пароль";
            }
        }
        catch (Exception e)
        {
            message = e.Message;
        }

        return false;
    }
    /// <summary>
    /// Расчёт хеша SHA256
    /// </summary>
    /// <param name="value">Строка для расчёта хеша</param>
    /// <returns>Возвращает хеш строки</returns>
    private static string sha256_hash(string value)
    {
        var Sb = new StringBuilder();
        using (var hash = SHA256.Create())
        {
            var enc = Encoding.UTF8;
            var result = hash.ComputeHash(enc.GetBytes(value));
            foreach (var b in result)
                Sb.Append(b.ToString("x2"));
        }

        return Sb.ToString();
    }

    public void Dispose()
    {
    }
}