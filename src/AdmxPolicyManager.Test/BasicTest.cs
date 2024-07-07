using AdmxParser;
using AdmxPolicyManager;

namespace AdmxPolicyManager.Test
{
    public class BasicTest
    {
        [Fact]
        public async Task RunTest()
        {
            var admxDirectory = AdmxDirectory.GetSystemPolicyDefinitions();
            await admxDirectory.LoadAsync();

            var inetres = admxDirectory.GetAdmxContentByPrefix("inetres");
            Assert.NotNull(inetres);

            var fontSizePolicy = inetres.GetUserPolicy("FontSize");
            Assert.NotNull(fontSizePolicy);

            fontSizePolicy.SetUserPolicy(true);
            Console.Out.WriteLine(fontSizePolicy.GetUserPolicy());

            var elemIdList = fontSizePolicy.GetElementIds();
            Assert.NotEmpty(elemIdList);

            var elemId = elemIdList.FirstOrDefault(x => string.Equals("FontSizeDefault", x, StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(elemId);

            var value = 2;
            fontSizePolicy.SetUserElement(elemId, value);

            var queryResult = fontSizePolicy.GetUserElement(elemId);
            Assert.NotNull(queryResult);
            Assert.Equal(value, queryResult.Value);
        }
    }
}