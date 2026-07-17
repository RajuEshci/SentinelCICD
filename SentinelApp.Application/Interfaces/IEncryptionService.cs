using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
        byte[] EncryptStringToBytes_Aes(string plainText);
        string DecryptStringFromBytes_Aes(byte[] cipherText);
    }
}
