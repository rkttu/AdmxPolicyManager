using AdmxParser;
using AdmxParser.Serialization;
using AdmxPolicyManager.Contracts.Policies;
using AdmxPolicyManager.Contracts.Presentation;
using AdmxPolicyManager.Models.Definitions;
using AdmxPolicyManager.Models.Elements;
using AdmxPolicyManager.Models.Policies;
using AdmxPolicyManager.Models.Presentation;
using AdmxPolicyManager.Models.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace AdmxPolicyManager.Models
{
    internal static class ModelLoaders
    {
        public static PolicyDefinitionCatalogInfo LoadPolicyDefinitionInfosFromDirectory(this AdmxDirectory directory)
        {
            if (!directory.Loaded)
                throw new ArgumentException("Please load the ADMX directory first.", nameof(directory));

            var loadedPolicyDefinitions = new List<PolicyDefinitionInfo>();

            // 모든 리소스를 불러오되, 참조 관계 부분만 제외.
            foreach (var eachAdmxContent in directory.LoadedAdmxContents)
            {
                if (!eachAdmxContent.Loaded)
                    continue;

                loadedPolicyDefinitions.Add(LoadPolicyDefinitionInfo(eachAdmxContent));
            }

            // 불러온 리소스들의 Using Namespace 내역을 순회
            foreach (var eachLoadedPolicyDefInfo in loadedPolicyDefinitions)
            {
                // Using Namespace 내역이 없으면 건너뜀
                if (eachLoadedPolicyDefInfo.UsingNamespaces == null ||
                    eachLoadedPolicyDefInfo.UsingNamespaces.Count < 1)
                    continue;

                var referenceList = new List<PolicyDefinitionInfo>();

                foreach (var eachUsingStatement in eachLoadedPolicyDefInfo.UsingNamespaces)
                {
                    var foundPolicyDef = loadedPolicyDefinitions.FirstOrDefault(x => string.Equals(eachUsingStatement.@namespace, x.TargetNamespace.@namespace, StringComparison.Ordinal));

                    if (foundPolicyDef == null)
                        continue;

                    referenceList.Add(foundPolicyDef);
                }

                eachLoadedPolicyDefInfo.ReferencedPolicyDefinitions = referenceList.AsReadOnly();
            }

            var catalog = new PolicyDefinitionCatalogInfo
            {
                AdmxDirectoryPath = directory.DirectoryPath,
                PolicyDefinitions = loadedPolicyDefinitions,
                Prefixes = new ReadOnlyCollection<string>(loadedPolicyDefinitions.Select(x => x.TargetNamespace.prefix).ToArray()),
                Namespaces = new ReadOnlyCollection<string>(loadedPolicyDefinitions.Select(x => x.TargetNamespace.@namespace).ToArray()),
            };

            catalog.Resolve();
            return catalog;
        }

        public static PolicyDefinitionInfo LoadPolicyDefinitionInfo(this AdmxContent content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            if (!content.Loaded)
                throw new ArgumentException("Please load the AdmxContent before using it.");

            var resourceInfoList = content.LoadedAdmlResources
                .Select(x => new PolicyResourceInfo
                {
                    TargetCulture = x.Key,
                    Presentations = LoadPresentationInfo(x.Value),
                    StringKeys = x.Value.StringKeys,
                    StringTable = x.Value.StringTable,
                }).ToArray();

            var fallbackResourceInfo = resourceInfoList.FirstOrDefault(x => x.IsFallbackCulture == true);
            var fallbackPresentations = fallbackResourceInfo?.Presentations ?? Array.Empty<PolicyPresentationInfo>();

            var supersededFiles = new List<string>();
            if (content.SupersededAdm != null)
                supersededFiles.AddRange(content.SupersededAdm.Where(x => !string.IsNullOrWhiteSpace(x.fileName)).Select(x => x.fileName));

            var supportedVersions = Enumerable.Empty<SupportedOnProductInfo>();
            var supportedTargetDefinitions = new List<SupportedOnDefinitionInfo>();

            if (content.SupportedOn != null)
            {
                supportedVersions = (content.SupportedOn.products ?? Array.Empty<SupportedProduct>())
                    .SelectMany(product => product.majorVersion ?? Array.Empty<SupportedMajorVersion>(), (product, majorVersion) => new { product, majorVersion, })
                    .SelectMany(pm => (pm.majorVersion?.minorVersion ?? Array.Empty<SupportedMinorVersion>()).DefaultIfEmpty(), (pm, minorVersion) => new SupportedOnProductInfo
                    {
                        HasMajorVersion = true,
                        HasMinorVersion = (minorVersion != null),

                        ProductName = pm.product.name,
                        MajorVersionName = pm.majorVersion.name,
                        MinorVersionName = minorVersion?.name ?? string.Empty,
                        Version = new Version($"{pm.majorVersion.versionIndex}.{minorVersion?.versionIndex ?? 0}"),

                        ProductDisplayNameKey = new ResourceKeyReference(pm.product.displayName),

                        MajorVersionDisplayNameKey = new ResourceKeyReference(pm.majorVersion.displayName),
                        MajorVersionIndex = pm.majorVersion.versionIndex,

                        MinorVersionDisplayNameKey = string.IsNullOrWhiteSpace(minorVersion?.displayName) ? null : new ResourceKeyReference(minorVersion.displayName),
                        MinorVersionIndex = minorVersion?.versionIndex,
                    })
                    .ToList()
                    .AsReadOnly();

                foreach (var eachDefinition in content.SupportedOn.definitions ?? Array.Empty<SupportedOnDefinition>())
                {
                    var andConditions = new List<SupportedCriteriaInfo>();
                    var orConditions = new List<SupportedCriteriaInfo>();

                    switch (eachDefinition.Item)
                    {
                        case SupportedAndCondition and:
                            var andItems = and.Items ?? Array.Empty<object>();
                            foreach (var eachAndItem in andItems)
                            {
                                switch (eachAndItem)
                                {
                                    case SupportedOnRange andRange:
                                        andConditions.Add(new SupportedCriteriaInfo
                                        {
                                            SupportedOn = SupportedOnType.Range,
                                            RefId = andRange.@ref,
                                            MaxVersion = andRange.maxVersionIndexSpecified ? (uint?)andRange.maxVersionIndex : null,
                                            MinVersion = andRange.minVersionIndexSpecified ? (uint?)andRange.minVersionIndex : null,
                                        });
                                        break;
                                    case SupportedOnReference andReference:
                                        andConditions.Add(new SupportedCriteriaInfo
                                        {
                                            SupportedOn = SupportedOnType.Reference,
                                            RefId = andReference.@ref,
                                        });
                                        break;
                                    default:
                                        throw new GroupPolicyManagementException($"Unsupported type '{eachAndItem?.GetType()?.ToString() ?? "(null)"}' found.");
                                }
                            }
                            break;
                        case SupportedOrCondition or:
                            var orItems = or.Items ?? Array.Empty<object>();
                            foreach (var eachOrItem in orItems)
                            {
                                switch (eachOrItem)
                                {
                                    case SupportedOnRange orRange:
                                        orConditions.Add(new SupportedCriteriaInfo
                                        {
                                            SupportedOn = SupportedOnType.Range,
                                            RefId = orRange.@ref,
                                            MaxVersion = orRange.maxVersionIndexSpecified ? (uint?)orRange.maxVersionIndex : null,
                                            MinVersion = orRange.minVersionIndexSpecified ? (uint?)orRange.minVersionIndex : null,
                                        });
                                        break;
                                    case SupportedOnReference orReference:
                                        orConditions.Add(new SupportedCriteriaInfo
                                        {
                                            SupportedOn = SupportedOnType.Reference,
                                            RefId = orReference.@ref,
                                        });
                                        break;
                                    default:
                                        throw new GroupPolicyManagementException($"Unsupported type '{eachOrItem?.GetType()?.ToString() ?? "(null)"}' found.");
                                }
                            }
                            break;
                    }

                    supportedTargetDefinitions.Add(new SupportedOnDefinitionInfo
                    {
                        DefinitionName = eachDefinition.name,
                        DisplayNameKey = new ResourceKeyReference(eachDefinition.displayName),
                        AndConditions = andConditions.AsReadOnly(),
                        OrConditions = orConditions.AsReadOnly(),
                    });
                }
            }

            var machinePolicyList = new List<MachinePolicyInfo>();
            var userPolicyList = new List<UserPolicyInfo>();
            var policyNames = new List<string>();

            foreach (var eachPolicy in content.Policies)
            {
                policyNames.Add(eachPolicy.name);
                var isMachinePolicy = eachPolicy.@class == PolicyClass.Both || eachPolicy.@class == PolicyClass.Machine;
                var isUserPolicy = eachPolicy.@class == PolicyClass.Both || eachPolicy.@class == PolicyClass.User;

                var categoryKeys = new List<EntityReference>();
                var parentCategory = eachPolicy.parentCategory;
                while (parentCategory != null)
                {
                    var @ref = parentCategory.@ref;

                    if (!string.IsNullOrWhiteSpace(@ref))
                        categoryKeys.Insert(0, new EntityReference(@ref));

                    parentCategory = content.Categories.FirstOrDefault(x => string.Equals(x.name, @ref, StringComparison.Ordinal))?.parentCategory;
                }

                var policyRegistryKey = eachPolicy.key;

                var enabledList = new List<PolicyRegistryValue>();
                if (eachPolicy.enabledValue != null)
                    enabledList.Add(new PolicyRegistryValue(policyRegistryKey, eachPolicy.valueName, eachPolicy.enabledValue));
                else if (eachPolicy.valueName != null)
                    enabledList.Add(new PolicyRegistryValue(policyRegistryKey, eachPolicy.valueName, 1.ToAdmxValue()));

                if (eachPolicy.enabledList?.item != null)
                {
                    var currentDefaultKey = string.IsNullOrWhiteSpace(eachPolicy.enabledList.defaultKey) ? policyRegistryKey : eachPolicy.enabledList.defaultKey;

                    foreach (var eachItem in eachPolicy.enabledList.item)
                    {
                        var currentRegistryKey = string.IsNullOrWhiteSpace(eachItem.key) ? currentDefaultKey : eachItem.key;
                        enabledList.Add(new PolicyRegistryValue(currentRegistryKey, eachItem.valueName, eachItem.value));
                    }
                }

                var disabledList = new List<PolicyRegistryValue>();
                if (eachPolicy.disabledValue != null)
                    disabledList.Add(new PolicyRegistryValue(policyRegistryKey, eachPolicy.valueName, eachPolicy.disabledValue));
                else if (eachPolicy.valueName != null)
                    disabledList.Add(new PolicyRegistryValue(policyRegistryKey, eachPolicy.valueName, 0.ToAdmxValue()));

                if (eachPolicy.disabledList?.item != null)
                {
                    var currentDefaultKey = string.IsNullOrWhiteSpace(eachPolicy.disabledList.defaultKey) ? policyRegistryKey : eachPolicy.disabledList.defaultKey;

                    foreach (var eachItem in eachPolicy.disabledList.item)
                    {
                        var currentRegistryKey = string.IsNullOrWhiteSpace(eachItem.key) ? currentDefaultKey : eachItem.key;
                        disabledList.Add(new PolicyRegistryValue(currentRegistryKey, eachItem.valueName, eachItem.value));
                    }
                }

                var elements = new List<IElementInfo>();
                var beList = new List<PolicyBooleanElementInfo>();
                var deList = new List<PolicyDecimalElementInfo>();
                var eeList = new List<PolicyEnumerationElementInfo>();
                var leList = new List<PolicyListElementInfo>();
                var ldeList = new List<PolicyLongDecimalElementInfo>();
                var mteList = new List<PolicyMultiTextElementInfo>();
                var teList = new List<PolicyTextElementInfo>();
                var defaultValueList = new List<PolicyRegistryValue>();

                var presentationInfo = fallbackPresentations.FirstOrDefault(x => string.Equals(eachPolicy.name, x.Id, StringComparison.Ordinal));

                if (eachPolicy.elements != null)
                {
                    foreach (var eachElement in eachPolicy.elements)
                    {
                        switch (eachElement)
                        {
                            case BooleanElement be:
                                var beElement = LoadBooleanElementInfo(be, fallbackResourceInfo, presentationInfo, policyRegistryKey);
                                beList.Add(beElement);
                                elements.Add(beElement);

                                if (beElement.RegistryValue != null)
                                    defaultValueList.Add(beElement.RegistryValue);

                                if (beElement.DefaultValue == true)
                                    defaultValueList.AddRange(beElement.TrueValueList);
                                else if (beElement.DefaultValue == false)
                                    defaultValueList.AddRange(beElement.FalseValueList);

                                break;
                            case DecimalElement de:
                                var deElement = LoadDecimalElementInfo(de, fallbackResourceInfo, presentationInfo, policyRegistryKey);
                                deList.Add(deElement);
                                elements.Add(deElement);

                                if (deElement.RegistryValue != null)
                                    defaultValueList.Add(deElement.RegistryValue);
                                break;
                            case EnumerationElement ee:
                                var eeElement = LoadEnumerationElementInfo(ee, fallbackResourceInfo, presentationInfo, policyRegistryKey);
                                eeList.Add(eeElement);
                                elements.Add(eeElement);

                                if (eeElement.RegistryValue != null)
                                    defaultValueList.Add(eeElement.RegistryValue);
                                break;
                            case ListElement le:
                                var leElement = LoadListElementInfo(le, fallbackResourceInfo, presentationInfo, policyRegistryKey);
                                leList.Add(leElement);
                                elements.Add(leElement);
                                break;
                            case LongDecimalElement lde:
                                var ldeElement = LoadLongDecimalElementInfo(lde, fallbackResourceInfo, presentationInfo, policyRegistryKey);
                                ldeList.Add(ldeElement);
                                elements.Add(ldeElement);

                                if (ldeElement.RegistryValue != null)
                                    defaultValueList.Add(ldeElement.RegistryValue);
                                break;
                            case multiTextElement mte:
                                var mteElement = LoadMultiTextElementInfo(mte, fallbackResourceInfo, presentationInfo, policyRegistryKey);
                                mteList.Add(mteElement);
                                elements.Add(mteElement);

                                if (mteElement.RegistryValue != null)
                                    defaultValueList.Add(mteElement.RegistryValue);
                                break;
                            case TextElement te:
                                var teElement = LoadTextElementInfo(te, fallbackResourceInfo, presentationInfo, policyRegistryKey);
                                teList.Add(teElement);
                                elements.Add(teElement);

                                if (teElement.RegistryValue != null)
                                    defaultValueList.Add(teElement.RegistryValue);
                                break;
                            default:
                                throw new GroupPolicyManagementException($"Unsupported element type '{(eachElement?.GetType()?.ToString() ?? "(null)")}' found.");
                        }
                    }
                }

                var supportedOnRef = eachPolicy.supportedOn?.@ref;

                if (isMachinePolicy)
                {
                    machinePolicyList.Add(new MachinePolicyInfo
                    {
                        Name = eachPolicy.name,
                        DisplayNameKey = new ResourceKeyReference(eachPolicy.displayName),
                        ExplainTextKey = new ResourceKeyReference(eachPolicy.explainText),
                        CategoryKeys = categoryKeys.AsReadOnly(),
                        Keywords = eachPolicy.keywords,
                        SeeAlso = eachPolicy.seeAlso,
                        SupportedOnKey = string.IsNullOrWhiteSpace(supportedOnRef) ? null : new EntityReference(supportedOnRef),
                        SupersededFiles = supersededFiles.AsReadOnly(),
                        EnabledList = enabledList.AsReadOnly(),
                        DisabledList = disabledList.AsReadOnly(),
                        DefaultValueList = defaultValueList.AsReadOnly(),
                        Elements = elements.AsReadOnly(),
                        BooleanElements = beList.AsReadOnly(),
                        DecimalElements = deList.AsReadOnly(),
                        EnumerationElements = eeList.AsReadOnly(),
                        ListElements = leList.AsReadOnly(),
                        LongDecimalElements = ldeList.AsReadOnly(),
                        MultiTextElements = mteList.AsReadOnly(),
                        TextElements = teList.AsReadOnly(),
                    });
                }

                if (isUserPolicy)
                {
                    userPolicyList.Add(new UserPolicyInfo
                    {
                        Name = eachPolicy.name,
                        DisplayNameKey = new ResourceKeyReference(eachPolicy.displayName),
                        ExplainTextKey = new ResourceKeyReference(eachPolicy.explainText),
                        CategoryKeys = categoryKeys.AsReadOnly(),
                        Keywords = eachPolicy.keywords,
                        SeeAlso = eachPolicy.seeAlso,
                        SupportedOnKey = string.IsNullOrWhiteSpace(supportedOnRef) ? null : new EntityReference(supportedOnRef),
                        SupersededFiles = supersededFiles.AsReadOnly(),
                        EnabledList = enabledList.AsReadOnly(),
                        DisabledList = disabledList.AsReadOnly(),
                        DefaultValueList = defaultValueList.AsReadOnly(),
                        Elements = elements.AsReadOnly(),
                        BooleanElements = beList.AsReadOnly(),
                        DecimalElements = deList.AsReadOnly(),
                        EnumerationElements = eeList.AsReadOnly(),
                        ListElements = leList.AsReadOnly(),
                        LongDecimalElements = ldeList.AsReadOnly(),
                        MultiTextElements = mteList.AsReadOnly(),
                        TextElements = teList.AsReadOnly(),
                    });
                }
            }

            return new PolicyDefinitionInfo
            {
                AdmxFilePath = content.AdmxFilePath,
                TargetNamespace = content.TargetNamespace,
                UsingNamespaces = content.UsingNamespaces,
                PolicyNames = policyNames.AsReadOnly(),
                MachinePolicies = machinePolicyList.AsReadOnly(),
                UserPolicies = userPolicyList.AsReadOnly(),
                Resources = new ReadOnlyCollection<PolicyResourceInfo>(resourceInfoList),
                SupersededFiles = supersededFiles.AsReadOnly(),
                SupportedOnProducts = new ReadOnlyCollection<SupportedOnProductInfo>(supportedVersions.ToArray()),
                SupportedOnTargetDefinitions = supportedTargetDefinitions.AsReadOnly(),
            };
        }

        public static PolicyResourceInfo LoadResourceInfo(CultureInfo targetCulture, AdmlResource resource)
        {
            return new PolicyResourceInfo
            {
                TargetCulture = targetCulture,
                StringKeys = resource.StringKeys,
                StringTable = resource.StringTable,
                Presentations = LoadPresentationInfo(resource),
            };
        }

        public static IReadOnlyList<PolicyPresentationInfo> LoadPresentationInfo(AdmlResource resource)
        {
            if (resource.PresentationTable.Count < 1)
                return Array.Empty<PolicyPresentationInfo>();

            var list = new List<PolicyPresentationInfo>();

            foreach (var eachPresentation in resource.PresentationTable)
            {
                if (eachPresentation.Items == null || eachPresentation.Items.Length < 1)
                    continue;

                var info = new PolicyPresentationInfo()
                {
                    Id = eachPresentation.id,
                };

                foreach (var eachControl in eachPresentation.Items)
                {
                    switch (eachControl)
                    {
                        case CheckBox cb:
                            var cbi = new PolicyCheckBoxInfo
                            {
                                RefId = cb.refId,
                                Default = cb.defaultChecked,
                                Label = cb.Value,
                            };
                            info.CheckBoxesInternal.Add(cbi);
                            info.ControlsInternal.Add(cbi);
                            break;
                        case ComboBox cmb:
                            var cmbi = new PolicyComboBoxInfo
                            {
                                RefId = cmb.refId,
                                Default = cmb.@default,
                                Label = cmb.label,
                                NoSort = cmb.noSort,
                                Suggestions = cmb.suggestion,
                            };
                            info.ComboBoxesInternal.Add(cmbi);
                            info.ControlsInternal.Add(cmbi);
                            break;
                        case DecimalTextBox dtb:
                            var dtbi = new PolicyDecimalTextBoxInfo
                            {
                                RefId = dtb.refId,
                                Default = dtb.defaultValue,
                                Label = dtb.Value,
                                Spin = dtb.spin,
                                SpinStep = dtb.spinStep,
                            };
                            info.DecimalTextBoxesInternal.Add(dtbi);
                            info.ControlsInternal.Add(dtbi);
                            break;
                        case LongDecimalTextBox ldtb:
                            var ldtbi = new PolicyLongDecimalTextBoxInfo
                            {
                                RefId = ldtb.refId,
                                Default = ldtb.defaultValue,
                                Label = ldtb.Value,
                                Spin = ldtb.spin,
                                SpinStep = ldtb.spinStep,
                            };
                            info.LongDecimalTextBoxesInternal.Add(ldtbi);
                            info.ControlsInternal.Add(ldtbi);
                            break;
                        case DropdownList dl:
                            var dli = new PolicyDropdownListInfo
                            {
                                RefId = dl.refId,
                                DefaultItemIndex = dl.defaultItem,
                                DefaultItemSpecified = dl.defaultItemSpecified,
                                NoSort = dl.noSort,
                                Label = dl.Value,
                            };
                            info.DropdownListsInternal.Add(dli);
                            info.ControlsInternal.Add(dli);
                            break;
                        case ListBox lb:
                            var lbi = new PolicyListBoxInfo
                            {
                                RefId = lb.refId,
                                Label = lb.Value,
                            };
                            info.ListBoxesInternal.Add(lbi);
                            info.ControlsInternal.Add(lbi);
                            break;
                        case MultiTextBox mtb:
                            var mtbi = new PolicyMultiTextBoxInfo
                            {
                                RefId = mtb.refId,
                                ShowAsDialog = mtb.showAsDialog,
                                DefaultHeight = mtb.defaultHeight,
                            };
                            info.MultiTextBoxesInternal.Add(mtbi);
                            info.ControlsInternal.Add(mtbi);
                            break;
                        case TextBox tb:
                            var tbi = new PolicyTextBoxInfo
                            {
                                RefId = tb.refId,
                                Default = tb.defaultValue,
                                Label = tb.label,
                            };
                            info.TextBoxesInternal.Add(tbi);
                            info.ControlsInternal.Add(tbi);
                            break;
                        case string s:
                            info.ControlsInternal.Add(new Models.Presentation.TextInfo() { Label = s, });
                            break;
                        default:
                            throw new GroupPolicyManagementException($"Unsupported control type '{eachControl?.GetType()?.ToString() ?? "(null)"}' found.");
                    }
                }

                list.Add(info);
            }

            return list.AsReadOnly();
        }

        public static PolicyBooleanElementInfo LoadBooleanElementInfo(BooleanElement be, PolicyResourceInfo fallbackResourceInfo, PolicyPresentationInfo presentation, string policyRegistryKey)
        {
            var currentRegistryKey = string.IsNullOrWhiteSpace(be.key) ? policyRegistryKey : be.key;

            var trueList = new List<PolicyRegistryValue>();
            if (be.trueList != null)
            {
                var subCurrentRegistryKey = string.IsNullOrWhiteSpace(be.trueList.defaultKey) ? currentRegistryKey : be.trueList.defaultKey;
                foreach (var eachItem in be.trueList.item)
                {
                    var eachItemCurrentRegistryKey = string.IsNullOrWhiteSpace(eachItem.key) ? subCurrentRegistryKey : eachItem.key;
                    if (eachItem.value == null)
                        continue;
                    trueList.Add(new PolicyRegistryValue(eachItemCurrentRegistryKey, eachItem.valueName, eachItem.value));
                }
            }

            var falseList = new List<PolicyRegistryValue>();
            if (be.falseList != null)
            {
                var subCurrentRegistryKey = string.IsNullOrWhiteSpace(be.falseList.defaultKey) ? currentRegistryKey : be.falseList.defaultKey;
                foreach (var eachItem in be.falseList.item)
                {
                    var eachItemCurrentRegistryKey = string.IsNullOrWhiteSpace(eachItem.key) ? subCurrentRegistryKey : eachItem.key;
                    if (eachItem.value == null)
                        continue;
                    falseList.Add(new PolicyRegistryValue(eachItemCurrentRegistryKey, eachItem.valueName, eachItem.value));
                }
            }

            var presentationCtrl = (presentation?.TryGetCheckBox(be.id, out var ctrl) == true && ctrl != null) ? ctrl : default;

            // [Citation from the syntax reference guide] The defaultChecked attribute is an optional attribute and is false by default.
            var @default = presentationCtrl?.Default;

            return new PolicyBooleanElementInfo()
            {
                ClientExtension = be.clientExtension,
                Id = be.id,
                TrueValue = be.trueValue,
                FalseValue = be.falseValue,
                TrueValueList = trueList.AsReadOnly(),
                FalseValueList = falseList.AsReadOnly(),
                Presentation = presentationCtrl,
                DefaultValue = @default,
                RegistryValue = PolicyRegistryValue.GetGroupPolicyRegistryValueFromBooleanElement(currentRegistryKey, @default, be),
            };
        }

        public static PolicyDecimalElementInfo LoadDecimalElementInfo(DecimalElement de, PolicyResourceInfo fallbackResourceInfo, PolicyPresentationInfo presentation, string policyRegistryKey)
        {
            var currentRegistryKey = string.IsNullOrWhiteSpace(de.key) ? policyRegistryKey : de.key;
            var presentationCtrl = (presentation?.TryGetDecimalTextBox(de.id, out var ctrl) == true && ctrl != null) ? ctrl : default;

            // [Citation from the syntax reference guide] The defaultValue attribute specifies the default numerical value of the parameter. If not specified the defaultValue attribute will be set to 1.
            var @default = presentationCtrl?.Default;

            return new PolicyDecimalElementInfo()
            {
                ClientExtension = de.clientExtension,
                Id = de.id,
                MaxValue = de.maxValue,
                MinValue = de.minValue,
                Required = de.required,
                Soft = de.soft,
                StoreAsText = de.storeAsText,
                Presentation = presentationCtrl,
                DefaultValue = @default,
                RegistryValue = PolicyRegistryValue.GetGroupPolicyRegistryValueFromDecimalElement(currentRegistryKey, @default, de),
            };
        }

        public static PolicyEnumerationElementInfo LoadEnumerationElementInfo(EnumerationElement ee, PolicyResourceInfo fallbackResourceInfo, PolicyPresentationInfo presentation, string policyRegistryKey)
        {
            var currentRegistryKey = string.IsNullOrWhiteSpace(ee.key) ? policyRegistryKey : ee.key;

            var list = new List<EnumerationElementItemInfo>();

            if (ee.item != null)
            {
                foreach (var eachItem in ee.item)
                {
                    var id = eachItem.GetMangledMemberNameFromDisplayName();
                    var valueList = new List<PolicyRegistryValue>();

                    if (eachItem.valueList != null)
                    {
                        var valueListCurrentRegistryKey = string.IsNullOrWhiteSpace(eachItem.valueList.defaultKey) ? currentRegistryKey : eachItem.valueList.defaultKey;

                        foreach (var eachValue in eachItem.valueList.item)
                        {
                            var valueCurrentRegistryKey = string.IsNullOrWhiteSpace(eachValue.key) ? valueListCurrentRegistryKey : eachValue.key;
                            if (eachValue.value == null)
                                continue;
                            valueList.Add(new PolicyRegistryValue(valueCurrentRegistryKey, eachValue.valueName, eachValue.value));
                        }
                    }

                    var info = new EnumerationElementItemInfo()
                    {
                        Id = id,
                        DisplayNameKey = new ResourceKeyReference(eachItem.displayName),
                        Value = eachItem.value,
                        ItemValues = valueList.AsReadOnly(),
                    };

                    list.Add(info);
                }
            }

            var presentationCtrl = (presentation?.TryGetDropdownList(ee.id, out var ctrl) == true && ctrl != null) ? ctrl : default;
            var defaultItemIndex = (-1);

            if (presentationCtrl != null)
                defaultItemIndex = (int)presentationCtrl.DefaultItemIndex;

            var @default = list.ElementAtOrDefault(defaultItemIndex);

            return new PolicyEnumerationElementInfo()
            {
                ClientExtension = ee.clientExtension,
                Id = ee.id,
                Required = ee.required,
                Items = list.AsReadOnly(),
                Presentation = presentationCtrl,
                DefaultValue = defaultItemIndex < 0 ? null : (int?)defaultItemIndex,
                RegistryValue = PolicyRegistryValue.GetGroupPolicyRegistryValueFromEnumerationElement(currentRegistryKey, defaultItemIndex, ee),
            };
        }

        public static PolicyListElementInfo LoadListElementInfo(ListElement le, PolicyResourceInfo fallbackResourceInfo, PolicyPresentationInfo presentation, string policyRegistryKey)
        {
            var currentRegistryKey = string.IsNullOrWhiteSpace(le.key) ? policyRegistryKey : le.key;
            var presentationCtrl = (presentation?.TryGetListBox(le.id, out var ctrl) == true && ctrl != null) ? ctrl : default;

            return new PolicyListElementInfo()
            {
                ClientExtension = le.clientExtension,
                Id = le.id,
                Additive = le.additive,
                Expandable = le.expandable,
                ExplicitValue = le.explicitValue,
                Presentation = presentationCtrl,

                RegistryKeyPath = currentRegistryKey,
                RegistryValuePrefix = le.valuePrefix,
            };
        }

        public static PolicyLongDecimalElementInfo LoadLongDecimalElementInfo(LongDecimalElement lde, PolicyResourceInfo fallbackResourceInfo, PolicyPresentationInfo presentation, string policyRegistryKey)
        {
            var currentRegistryKey = string.IsNullOrWhiteSpace(lde.key) ? policyRegistryKey : lde.key;
            var presentationCtrl = (presentation?.TryGetLongDecimalTextBox(lde.id, out var ctrl) == true && ctrl != null) ? ctrl : default;
            var @default = presentationCtrl?.Default;

            return new PolicyLongDecimalElementInfo()
            {
                ClientExtension = lde.clientExtension,
                Id = lde.id,
                MaxValue = lde.maxValue,
                MinValue = lde.minValue,
                Required = lde.required,
                Soft = lde.soft,
                StoreAsText = lde.storeAsText,
                Presentation = presentationCtrl,
                DefaultValue = @default,
                RegistryValue = PolicyRegistryValue.GetGroupPolicyRegistryValueFromLongDecimalElement(currentRegistryKey, @default, lde),
            };
        }

        public static PolicyMultiTextElementInfo LoadMultiTextElementInfo(multiTextElement mte, PolicyResourceInfo fallbackResourceInfo, PolicyPresentationInfo presentation, string policyRegistryKey)
        {
            var currentRegistryKey = string.IsNullOrWhiteSpace(mte.key) ? policyRegistryKey : mte.key;
            var presentationCtrl = (presentation?.TryGetMultiTextBox(mte.id, out var ctrl) == true && ctrl != null) ? ctrl : default;

            return new PolicyMultiTextElementInfo()
            {
                ClientExtension = mte.clientExtension,
                Id = mte.id,
                MaxLength = mte.maxLength,
                MaxStrings = mte.maxStrings,
                Required = mte.required,
                Soft = mte.soft,
                Presentation = presentationCtrl,
                RegistryValue = PolicyRegistryValue.GetGroupPolicyRegistryValueFromMultiTextElement(currentRegistryKey, mte),
            };
        }

        public static PolicyTextElementInfo LoadTextElementInfo(TextElement te, PolicyResourceInfo fallbackResourceInfo, PolicyPresentationInfo presentation, string policyRegistryKey)
        {
            var currentRegistryKey = string.IsNullOrWhiteSpace(te.key) ? policyRegistryKey : te.key;
            var presentationTextBoxCtrl = (presentation?.TryGetTextBox(te.id, out var ctrl) == true && ctrl != null) ? ctrl : default;
            var presentationComboBoxCtrl = (presentation?.TryGetComboBox(te.id, out var ctrl2) == true && ctrl2 != null) ? ctrl2 : default;

            var presentationCtrl = default(IPresentationControlHasRefId);
            var @default = default(string);

            if (presentationTextBoxCtrl != null)
            {
                presentationCtrl = presentationTextBoxCtrl;
                @default = presentationTextBoxCtrl.Default;
            }
            else if (presentationComboBoxCtrl != null)
            {
                presentationCtrl = presentationComboBoxCtrl;
                @default = presentationComboBoxCtrl.Default;
            }

            return new PolicyTextElementInfo()
            {
                ClientExtension = te.clientExtension,
                Id = te.id,
                Expandable = te.expandable,
                MaxLength = te.maxLength,
                Required = te.required,
                Soft = te.soft,
                Presentation = presentationCtrl,
                DefaultValue = @default,
                RegistryValue = PolicyRegistryValue.GetGroupPolicyRegistryValueFromTextElement(currentRegistryKey, @default, te),
            };
        }
    }
}
