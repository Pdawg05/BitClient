using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using net.vieapps.Components.Utility;
using System.Numerics;
using System.IO;
using HBitcoin.KeyManagement;
using NBitcoin;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text.Json;
using System.Runtime.Serialization;

namespace BitcoinClient
{
    public class WalletGeneration
    {
        public string PublicKeyStr { get; set; }
        public string PrivateKeyStr { get; set; }
        public static string WalletDirectoryPath = "C:/Users/" + Environment.UserName + "/AppData/Roaming/BitcoinClient";
        public static string WalletFilePath = "C:/Users/" + Environment.UserName + "/AppData/Roaming/BitcoinClient/wallet.json";
        public byte[] PublicKey { get; set; }
        public byte[] EncryptedPrivateKey { get; set; }
        public string Address { get; set; }

        // Create a bitcoin address, public, and private key on constuctor

        // a CreateWallet parameter exists so that when deserializing the code inside the constructor is not ran again
        public WalletGeneration(bool CreateWallet = false)
        {
            if (CreateWallet)
            {
                if (File.Exists(WalletFilePath))
                {
                    throw new Exception("Wallet already exists! Please delete wallet.json to make a new wallet!");
                }
                // Creates public and private ECDsa keys
                var ecdsa = ECDsa.Create();

                // Create public key (step 1)
                var publicKey = ecdsa.ExportSubjectPublicKeyInfo();
                var privateKey = ecdsa.ExportECPrivateKey();

                // Encrypt with SHA256 (step 2)
                var publicHash = System.Security.Cryptography.SHA256.Create().ComputeHash(publicKey);


                // Then hash with RIPDEMD160 (Step 3)
                byte[] RIPEMDHASH = CryptoService.GetRIPEMD160Hash(ByteArrayToString(publicHash));

                // Add 00 to beginning (step 4)
                RIPEMDHASH = AppendBitcoinNetwork(RIPEMDHASH, 0);

                // Hash two more times with SHA256 (step 5 and 6)
                var hashed = SHA256.Create().ComputeHash(RIPEMDHASH);
                var hashed2 = SHA256.Create().ComputeHash(hashed);
                
                // Take first 4 bytes and add to end of RIPDEMDHASH, step 7
                byte[] byteAddress = ConcatAddress(RIPEMDHASH, hashed2);

                // Hash one last time, step 8
                string bitcoinAddress = Base58Encode(byteAddress);

                // Encrypt private key and set values
                PublicKey = publicKey;
                EncryptedPrivateKey = ProtectedData.Protect(privateKey, null, DataProtectionScope.CurrentUser);
                Address = bitcoinAddress;
                PrivateKeyStr = ByteArrayToString(EncryptedPrivateKey);
                PublicKeyStr = ByteArrayToString(PublicKey);

                StoreKeysOnDisk();
                LoadKeysOnDisk();
            }
        }

        public static WalletGeneration LoadKeysOnDisk()
        {
            // read json
            string keys = File.ReadAllText(WalletFilePath);

            // deserialize into class
            WalletGeneration wallet = JsonConvert.DeserializeObject<WalletGeneration>(keys); // -> this line triggers constructor code to run so that is why CreateWallet boolean is used

            wallet.PublicKeyStr = ByteArrayToString(wallet.PublicKey);
            wallet.PrivateKeyStr = ByteArrayToString(ProtectedData.Unprotect(wallet.EncryptedPrivateKey, null, DataProtectionScope.CurrentUser));
            return wallet;
        } 

        private void StoreKeysOnDisk()
        {
            File.Create(WalletFilePath).Close();
            string WalletData = JsonConvert.SerializeObject(this);
            File.WriteAllText(WalletFilePath, WalletData);
        }



        public static byte[] AppendBitcoinNetwork(byte[] RipeHash, byte Network)
        {
            byte[] prefixed = new byte[RipeHash.Length + 1];
            prefixed[0] = 0;
            Array.Copy(RipeHash, 0, prefixed, 1, RipeHash.Length);
            return prefixed;
            /*
            byte[] extended = new byte[RipeHash.Length + 1];
            extended[0] = (byte)Network;
            Array.Copy(RipeHash, 0, extended, 1, RipeHash.Length);
            return extended; */
        }

        public static byte[] ConcatAddress(byte[] RipeHash, byte[] Checksum)
        {
            byte[] ret = new byte[RipeHash.Length + 4];
            Array.Copy(RipeHash, ret, RipeHash.Length);
            Array.Copy(Checksum, 0, ret, RipeHash.Length, 4);
            return ret;
        }

        // Private class functions to help with address generation
        private byte[] HashWith256(byte[] input)
        {
            return SHA256.Create().ComputeHash(input);
        }

        static string ByteArrayToString(byte[] array)
        {
            return BitConverter.ToString(array).Replace("-", "");
        }

        public static string Base58Encode(byte[] array)
        {
            const string ALPHABET = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            string retString = string.Empty;
            BigInteger encodeSize = ALPHABET.Length;
            BigInteger arrayToInt = 0;

            for (int i = 0; i < array.Length; ++i)
            {
                arrayToInt = arrayToInt * 256 + array[i];
            }

            while (arrayToInt > 0)
            {
                int rem = (int)(arrayToInt % encodeSize);
                arrayToInt /= encodeSize;
                retString = ALPHABET[rem] + retString;
            }

            for (int i = 0; i < array.Length && array[i] == 0; ++i)
                retString = ALPHABET[0] + retString;
            return retString;
        }
    }
}
