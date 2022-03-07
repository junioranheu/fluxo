using System.Security.Cryptography;
using System.Text;
using TimeZoneConverter;

namespace ProjetoGuia_Biblioteca
{
    public class Biblioteca
    {
        // Constantes
        public const string localApiOnline = "https://fluxoapi.azurewebsites.net/";
        public const string localApiLocalHost = "https://localhost:7131/";

        public const string localReactOnline = "https://fluxo.vercel.app/";
        public const string localReactLocalHost = "http://localhost:3000/";

        public const string nomeApp = "Fluxo";
        public const string sloganApp = "Conheça sua cidade!";
        public const string keywordsApp = "fluxo, conheça, conhecer, cidade, lugar, lugares, festa, lanchonete, bar, amigo,";

        // Converter para o horário de Brasilia: https://blog.yowko.com/timezoneinfo-time-zone-id-not-found/;
        public static DateTime HorarioBrasilia()
        {
            TimeZoneInfo timeZone = TZConvert.GetTimeZoneInfo("E. South America Standard Time");
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone);
        }

        // Criptografar e decriptografar senha: https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp;
        public static string Criptografar(string clearText)
        {
            string EncryptionKey = "abc123";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using MemoryStream ms = new();
                using (CryptoStream cs = new(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }

                clearText = Convert.ToBase64String(ms.ToArray());
            }

            return clearText;
        }

        public static string Descriptografar(string cipherText)
        {
            string EncryptionKey = "abc123";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using MemoryStream ms = new();
                using (CryptoStream cs = new(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }

                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }

            return cipherText;
        }

        public static string PrimeiroNome(string nome)
        {
            string primeiroNome;

            if (String.IsNullOrEmpty(nome))
            {
                return string.Empty;
            }

            primeiroNome = nome.Split(' ').FirstOrDefault();

            if (String.IsNullOrEmpty(primeiroNome))
            {
                return string.Empty;
            }

            return primeiroNome;
        }

        public static string CodigoAleatorio(int tamanho)
        {
            /* Código aleatório: https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings */
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[tamanho];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var codigoAleatorio = new String(stringChars);
            return codigoAleatorio;
        }

        public static string NumeroAleatorio(int tamanho)
        {
            /* Código aleatório: https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings */
            var chars = "0123456789";
            var stringChars = new char[tamanho];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var codigoAleatorio = new String(stringChars);
            return codigoAleatorio;
        }

        public static string FormatarBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;

            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        public static bool IsDebug()
        {
            // https://stackoverflow.com/questions/12135854/best-way-to-tell-if-in-production-or-development-environment-in-net
#if DEBUG
            return true;
#else
        return false;
#endif
        }

        public static string CaminhoAPI()
        {
            string urlApi = localApiOnline;

            if (IsDebug())
            {
                urlApi = localApiLocalHost;
            }

            return urlApi;
        }

        public static string CaminhoReact()
        {
            string urlApi = localReactOnline;

            if (IsDebug())
            {
                urlApi = localReactLocalHost;
            }

            return urlApi;
        }

        public static string MensagemErroAPI(HttpResponseMessage res, string caminho)
        {
            string statusErro = "Status: " + res.StatusCode.ToString();
            string caminhoErro = "Caminho: " + caminho;
            string codigoErro = "Codigo: " + res.IsSuccessStatusCode.ToString();
            string razaoErro = "Razão: " + res.ReasonPhrase?.ToString();
            string mensagem = "Mensagem: " + res.RequestMessage;
            string conteudo = "Conteúdo: " + res.Content.ReadAsStringAsync().Result;

            string msgFinal = statusErro + "\n" + caminhoErro + "\n" + codigoErro + "\n" + razaoErro + "\n" + mensagem + "\n" + conteudo;
            msgFinal = String.Format("Houve uma falha ao realizar uma ou mais chamadas no back-end\n {0}", msgFinal);

            return msgFinal;
        }

        public static string MensagemDiaTardeNoite()
        {
            int hora = HorarioBrasilia().Hour;
            string saudacao = (
                hora >= 5 && hora <= 12 ? "Bom dia" :
                hora >= 18 || hora <= 4 ? "Boa noite" : "Boa tarde"
                );

            return saudacao;
        }

        public static string GerarPalavraAleatoria(int tamanho)
        {
            // https://stackoverflow.com/questions/18110243/random-word-generator-2
            Random rnd = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };
            string[] vowels = { "a", "e", "i", "o", "u" };

            string word = "";

            if (tamanho == 1)
            {
                word = PegarLetraAleatoria(rnd, vowels);
            }
            else
            {
                for (int i = 0; i < tamanho; i += 2)
                {
                    word += PegarLetraAleatoria(rnd, consonants) + PegarLetraAleatoria(rnd, vowels);
                }

                word = word.Replace("q", "qu").Substring(0, tamanho); // We may generate a string longer than requested length, but it doesn't matter if cut off the excess.
            }

            return word;
        }

        private static string PegarLetraAleatoria(Random rnd, string[] letters)
        {
            return letters[rnd.Next(0, letters.Length - 1)];
        }
    }
}