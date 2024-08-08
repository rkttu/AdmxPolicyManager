using AdmxParser;
using AdmxPolicyManager;
using AdmxPolicyManager.Models;

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

        [Fact]
        public void BatchRunTest()
        {
            var requests = new SetMultipleGroupPolicyRequest[] {
                new SetMultipleGroupPolicyRequest
                {
                    SubKey = "Software\\Policies\\Microsoft\\Edge",
                    ValueName = "AllowSurfGame",
                    Value = 0,
                    RequireExpandString = false,
                },
                new SetMultipleGroupPolicyRequest
                {
                    SubKey = "Software\\Policies\\Microsoft\\Edge",
                    ValueName = "AllowSurfGame",
                    Value = 1,
                    RequireExpandString = false,
                },
                new SetMultipleGroupPolicyRequest
                {
                    SubKey = "Software\\Policies\\Microsoft\\Edge",
                    ValueName = "AllowSurfGame",
                    Value = 0,
                    RequireExpandString = false,
                },
                new SetMultipleGroupPolicyRequest
                {
                    SubKey = "Software\\Policies\\Microsoft\\Edge",
                    ValueName = "AllowSurfGame",
                    Value = 1,
                    RequireExpandString = false,
                },
            };

            var results = GroupPolicy.SetMultipleUserPolicies(requests);

            Assert.Equal(4, results);
            Assert.Equal(4, requests.Count(x => x.Result == GroupPolicyUpdateResult.UpdateSucceed));

            var result = GroupPolicy.GetUserPolicy("Software\\Policies\\Microsoft\\Edge", "AllowSurfGame");
            Assert.Equal(1, result.Value);
        }
    }
}