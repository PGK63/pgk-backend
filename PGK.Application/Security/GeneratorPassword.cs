using System.Text;
using PGK.Application.Common;

namespace PGK.Application.Security;

internal class GeneratorPassword
{
    public static string GetPassword(int length = Constants.PASSWORD_LENGTH)
    {
        const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
 
        var sb = new StringBuilder();
        var rnd = new Random();
 
        for(var i = 0; i < length; i++)
        {
            var index = rnd.Next(chars.Length);
            sb.Append(chars[index]);
        }
 
        return sb.ToString();
    }
}