using System.Security.Cryptography;
using System.Text;

namespace TaskFlow.UI.Web.Helpers
{
    /// <summary>
    /// Classe para criptografia e descriptografia de dados
    /// </summary>
    public static class FuncaoCriptografia
    {
        // Chave fixa para criptografia simétrica (em produção, use configuração segura)
        private static readonly string ChaveSecreta = "TaskFlow2025@#$%";

        /// <summary>
        /// Criptografa uma string usando AES
        /// </summary>
        /// <param name="textoClaro">Texto a ser criptografado</param>
        /// <returns>Texto criptografado em Base64</returns>
        public static string Criptografar(string textoClaro)
        {
            if (string.IsNullOrEmpty(textoClaro))
                return string.Empty;

            try
            {
                byte[] chaveBytes = Encoding.UTF8.GetBytes(ChaveSecreta.PadRight(32).Substring(0, 32));
                byte[] ivBytes = Encoding.UTF8.GetBytes(ChaveSecreta.PadRight(16).Substring(0, 16));

                using (Aes aes = Aes.Create())
                {
                    aes.Key = chaveBytes;
                    aes.IV = ivBytes;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(textoClaro);
                            }
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Descriptografa uma string usando AES
        /// </summary>
        /// <param name="textoCriptografado">Texto criptografado em Base64</param>
        /// <returns>Texto descriptografado</returns>
        public static string Descriptografar(string textoCriptografado)
        {
            if (string.IsNullOrEmpty(textoCriptografado))
                return string.Empty;

            try
            {
                byte[] chaveBytes = Encoding.UTF8.GetBytes(ChaveSecreta.PadRight(32).Substring(0, 32));
                byte[] ivBytes = Encoding.UTF8.GetBytes(ChaveSecreta.PadRight(16).Substring(0, 16));
                byte[] buffer = Convert.FromBase64String(textoCriptografado);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = chaveBytes;
                    aes.IV = ivBytes;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(buffer))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Criptografa um número inteiro
        /// </summary>
        public static string CriptografarInt(int valor)
        {
            return Criptografar(valor.ToString());
        }

        /// <summary>
        /// Descriptografa para número inteiro
        /// </summary>
        public static int DescriptografarInt(string textoCriptografado)
        {
            string valor = Descriptografar(textoCriptografado);
            return int.TryParse(valor, out int resultado) ? resultado : 0;
        }
    }
}