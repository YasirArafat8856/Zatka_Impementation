using System.Security.Cryptography;

using (RSA rsa = RSA.Create(2048))  // Generate a 2048-bit RSA key
{
    // Export private key
    string privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
    File.WriteAllText("private_key.pem", privateKey);

    // Export public key
    string publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
    File.WriteAllText("public_key.pem", publicKey);

    Console.WriteLine("Keys generated and saved!");
}
