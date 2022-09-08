using Amazon;
using Amazon.KeyManagementService;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using AWS.EncryptionSDK;
using AWS.EncryptionSDK.Core;

Console.WriteLine("Hello, World!");
var keyArn = "arn:aws:kms:us-west-2:544610684157:key/9d20c852-b477-4c5a-ab14-b895a2ae61ce";

var secretsManager = new AmazonSecretsManagerClient(RegionEndpoint.USWest2);
var encryptionSdk = AwsEncryptionSdkFactory.CreateDefaultAwsEncryptionSdk();
var materialProviders = AwsCryptographicMaterialProvidersFactory.CreateDefaultAwsCryptographicMaterialProviders();

var kmsKeyringInput = new CreateAwsKmsKeyringInput
{    
    KmsClient = new AmazonKeyManagementServiceClient(),
    KmsKeyId = keyArn
};

var keyring = materialProviders.CreateAwsKmsKeyring(kmsKeyringInput);
var encryptionContext = await GetEncryptionContext();

var plaintext = "Some random text to encrypt";
            
// Define the encrypt input
var encryptInput = new EncryptInput
{
    Plaintext = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(plaintext)),
    Keyring = keyring,
    EncryptionContext = encryptionContext
};

var result = encryptionSdk.Encrypt(encryptInput).Ciphertext;

Console.WriteLine("Encryption completed!");

var streamReader = new StreamReader(result);
var cipher = await streamReader.ReadToEndAsync();
Console.WriteLine($"cipher: {cipher}");

async Task<Dictionary<string, string>> GetEncryptionContext()
{
     var secret = await secretsManager.GetSecretValueAsync(
        new GetSecretValueRequest()
        {
            SecretId = "encryption_context",
        }
    );
    var context = secret.SecretString;

    return new Dictionary<string,string>();
}



