using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Euler
{
    class Problem59 : Problem
    {
        public override object Solve()
        {
            byte[] cipherText = File.ReadAllText("p059_cipher.txt").Split(',').Select(byte.Parse).ToArray();
            string encryptedMessage = Encoding.ASCII.GetString(cipherText);

            var regex = new Regex(@"\(the gospel of john", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            byte a = Encoding.ASCII.GetBytes(new[] { 'a' })[0];
            byte z = Encoding.ASCII.GetBytes(new[] { 'z' })[0];

            for (byte i = a; i <= z; i++)
            {
                for (byte j = a; j <= z; j++)
                {
                    for (byte k = a; k <= z; k++)
                    {
                        string key = Encoding.ASCII.GetString(new[] { i, j, k });

                        var plainText = Decrypt(encryptedMessage, key).ToArray();
                        var message = Encoding.ASCII.GetString(plainText);

                        //NonBlockingConsole.WriteLine(message);
                        //NonBlockingConsole.WriteLine();
                        //NonBlockingConsole.Write(".");

                        if (regex.IsMatch(message))
                        {
                            NonBlockingConsole.WriteLine(message);

                            var answer = 0;
                            foreach (var b in plainText)
                            {
                                answer += b;
                            }

                            return answer;
                        }
                    }
                }
            }

            return null;
        }

        private IEnumerable<byte> Decrypt(string cipherText, string key)
        {
            for (int i = 0; i < cipherText.Length; i++)
            {
                var character = Encoding.ASCII.GetBytes(new[] { cipherText[i] });
                var keyByte = Encoding.ASCII.GetBytes(new[] { key[i % 3] });

                yield return (byte)(keyByte[0] ^ character[0]);
            }
        }
    }
}
