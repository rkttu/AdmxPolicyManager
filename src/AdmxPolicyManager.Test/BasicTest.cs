using AdmxPolicyManager.Contracts.Policies;
using AdmxPolicyManager.Models.Definitions;
using AdmxPolicyManager.Models.Elements;
using System.Diagnostics;

namespace AdmxPolicyManager.Test
{
    public class BasicTest
    {
        [Fact]
        public async Task RunFullTest()
        {
            var cancellationToken = default(CancellationToken);

            var inputDirectoryPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                @"Microsoft Group Policy\Windows 11 October 2023 Update (23H2)\PolicyDefinitions");

            var catalog = default(PolicyDefinitionCatalogInfo);
            if (Directory.Exists(inputDirectoryPath))
                catalog = await GroupPolicy.GetCatalogAsync(inputDirectoryPath, cancellationToken);
            else
                catalog = await GroupPolicy.GetCatalogFromSystemAsync(cancellationToken);

            using var context = new GroupPolicyContext();
            await context.ExecuteAsync(() =>
            {
                var subjectGuid = Guid.NewGuid();
                if (!catalog.TryGetPolicyDefinitionByPrefix("inetres", out var inetres) || inetres == null)
                    return Task.CompletedTask;

                var ieFontSizePolicy = inetres.UserPolicies.FirstOrDefault(x => x.Name == "FontSize");
                if (ieFontSizePolicy == null)
                    return Task.CompletedTask;

                Debug.WriteLine(GroupPolicy.GetGroupSecurityIdentifiersOfCurrentUser());
                Debug.WriteLine(GroupPolicy.IsGroupPolicyServiceRunning());
                Debug.WriteLine(GroupPolicy.IsGroupPolicyServiceConfiguredCorrectly());

                if (ieFontSizePolicy.Elements[0].Type == PolicyElementType.Enumeration)
                {
                    Debug.WriteLine(((PolicyEnumerationElementInfo)ieFontSizePolicy.Elements[0]));
                }

                Debug.WriteLine(ieFontSizePolicy.GetElementValue("element_id", null));

                if (GroupPolicy.IsCurrentUserAdministrator())
                {
                    var userOrGroupSid = default(string); // GroupPolicy.GetCurrentUserSecurityIdentifier();
                    Debug.WriteLine(ieFontSizePolicy.ToString());
                    Debug.WriteLine(ieFontSizePolicy.GetElementValue("FontSizeDefault", userOrGroupSid));
                    Debug.WriteLine(ieFontSizePolicy.GetElementEnumIds("FontSizeDefault"));

                    Debug.WriteLine("Changing IE Font Size Policy...");

                    var modResults = ieFontSizePolicy.EnablePolicy(subjectGuid, userOrGroupSid);
                    Debug.WriteLine($"Total {modResults.Count} registry item(s) have been processed.");

                    Debug.WriteLine("Refreshing pending policies...");
                    GroupPolicy.RefreshPolicy();

                    Debug.WriteLine($"Current IE Font Size Policy: {ieFontSizePolicy.QueryPolicyStatus(userOrGroupSid)}");
                }
                else
                    Debug.WriteLine("Cannot change the policy because you don't have Administrator privilege.");

                return Task.CompletedTask;
            }, cancellationToken: cancellationToken);
        }
    }
}