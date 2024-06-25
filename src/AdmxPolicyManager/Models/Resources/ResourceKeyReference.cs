using AdmxPolicyManager.Models.Definitions;
using AdmxPolicyManager.Models.Presentation;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdmxPolicyManager.Models.Resources
{
    /// <summary>
    /// Represents a reference to a resource key.
    /// </summary>
    public sealed class ResourceKeyReference
    {
        private static readonly Lazy<Regex> _regexFactory = new Lazy<Regex>(
            () => new Regex(@"\$\((?<ResourceType>[^.]+)\.(?<ResourceKey>[^\)]+)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            LazyThreadSafetyMode.None);

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceKeyReference"/> class with the specified expression.
        /// </summary>
        /// <param name="expression">The expression representing the resource key reference.</param>
        /// <exception cref="ArgumentException">Thrown when the expression is null or whitespace, or when the expression is not valid.</exception>
        public ResourceKeyReference(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Expression cannot be null or whitespace string.", nameof(expression));

            var match = _regexFactory.Value.Match(expression);
            if (!match.Success)
                throw new ArgumentException($"The expression '{expression}' is not valid.", nameof(expression));

            var resourceType = match.Groups["ResourceType"].Value;
            var resourceKey = match.Groups["ResourceKey"].Value;

            if (string.IsNullOrWhiteSpace(resourceType))
                throw new ArgumentException("Resource type cannot be null or whitespace string.", nameof(resourceType));
            if (string.IsNullOrWhiteSpace(resourceKey))
                throw new ArgumentException("Resource key cannot be null or whitespace string.", nameof(resourceKey));

            _resourceType = resourceType;
            _resourceKey = resourceKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceKeyReference"/> class with the specified resource type and resource key.
        /// </summary>
        /// <param name="resourceType">The type of the resource.</param>
        /// <param name="resourceKey">The key of the resource.</param>
        /// <exception cref="ArgumentException">Thrown when the resource type is null or whitespace, or when the resource key is null or whitespace.</exception>
        public ResourceKeyReference(string resourceType, string resourceKey)
        {
            if (string.IsNullOrWhiteSpace(resourceType))
                throw new ArgumentException("Resource type cannot be null or whitespace string.", nameof(resourceType));
            if (string.IsNullOrWhiteSpace(resourceKey))
                throw new ArgumentException("Resource key cannot be null or whitespace string.", nameof(resourceKey));

            _resourceType = resourceType;
            _resourceKey = resourceKey;
        }

        private readonly string _resourceType;
        private readonly string _resourceKey;

        /// <summary>
        /// Gets the expression representing the resource key reference.
        /// </summary>
        public string Expression
            => $"$({_resourceType}.{_resourceKey})";

        /// <summary>
        /// Gets the type of the resource.
        /// </summary>
        public string ResourceType => _resourceType;

        /// <summary>
        /// Gets the key of the resource.
        /// </summary>
        public string ResourceKey => _resourceKey;

        /// <summary>
        /// Gets a value indicating whether the resource reference is a string reference.
        /// </summary>
        public bool IsStringReference
            => string.Equals("string", _resourceType, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a value indicating whether the resource reference is a presentation reference.
        /// </summary>
        public bool IsPresentationReference
            => string.Equals("presentation", _resourceKey, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Tries to resolve the string value of the resource key reference.
        /// </summary>
        /// <param name="policyDefinitionInfo">The policy definition info.</param>
        /// <param name="targetCulture">The target culture.</param>
        /// <param name="resolvedString">The resolved string value.</param>
        /// <returns><c>true</c> if the string value is resolved successfully; otherwise, <c>false</c>.</returns>
        public bool TryResolveString(PolicyDefinitionInfo policyDefinitionInfo, CultureInfo targetCulture, out string resolvedString)
        {
            resolvedString = default;

            if (IsStringReference)
            {
                if (targetCulture != null)
                {
                    if (policyDefinitionInfo.TryGetResourceByCulture(targetCulture, out var targetResource) && targetResource != null)
                    {
                        if (targetResource.TryGetStringByKey(_resourceKey, out var targetString) && targetString != null)
                        {
                            resolvedString = targetString;
                            return true;
                        }
                    }
                }
                else
                {
                    if (policyDefinitionInfo.TryGetFallbackResource(out var fallbackResource) && fallbackResource != null)
                    {
                        if (fallbackResource.TryGetStringByKey(_resourceKey, out var fallbackString) && fallbackString != null)
                        {
                            resolvedString = fallbackString;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to resolve the string value of the resource key reference from the fallback culture.
        /// </summary>
        /// <param name="policyDefinitionInfo">The policy definition info.</param>
        /// <param name="resolvedString">The resolved string value.</param>
        /// <returns><c>true</c> if the string value is resolved successfully; otherwise, <c>false</c>.</returns>
        public bool TryResolveStringFromFallbackCulture(PolicyDefinitionInfo policyDefinitionInfo, out string resolvedString)
            => TryResolveString(policyDefinitionInfo, null, out resolvedString);

        /// <summary>
        /// Tries to resolve the presentation info of the resource key reference.
        /// </summary>
        /// <param name="policyDefinitionInfo">The policy definition info.</param>
        /// <param name="targetCulture">The target culture.</param>
        /// <param name="resolvedPresentation">The resolved presentation info.</param>
        /// <returns><c>true</c> if the presentation info is resolved successfully; otherwise, <c>false</c>.</returns>
        public bool TryResolvePresentation(PolicyDefinitionInfo policyDefinitionInfo, CultureInfo targetCulture, out PolicyPresentationInfo resolvedPresentation)
        {
            resolvedPresentation = default;

            if (IsPresentationReference)
            {
                if (targetCulture != null)
                {
                    if (policyDefinitionInfo.TryGetResourceByCulture(targetCulture, out var targetResource) && targetResource != null)
                    {
                        if (targetResource.TryGetPresentationByKey(_resourceKey, out var targetPresentation) && targetPresentation != null)
                        {
                            resolvedPresentation = targetPresentation;
                            return true;
                        }
                    }
                }
                else
                {
                    if (policyDefinitionInfo.TryGetFallbackResource(out var fallbackResource) && fallbackResource != null)
                    {
                        if (fallbackResource.TryGetPresentationByKey(_resourceKey, out var fallbackPresentation) && fallbackPresentation != null)
                        {
                            resolvedPresentation = fallbackPresentation;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to resolve the presentation info of the resource key reference from the fallback culture.
        /// </summary>
        /// <param name="policyDefinitionInfo">The policy definition info.</param>
        /// <param name="resolvedPresentation">The resolved presentation info.</param>
        /// <returns><c>true</c> if the presentation info is resolved successfully; otherwise, <c>false</c>.</returns>
        public bool TryResolvePresentationFromFallbackCulture(PolicyDefinitionInfo policyDefinitionInfo, out PolicyPresentationInfo resolvedPresentation)
            => TryResolvePresentation(policyDefinitionInfo, null, out resolvedPresentation);

        /// <summary>
        /// Returns the expression representing the resource key reference.
        /// </summary>
        /// <returns>The expression representing the resource key reference.</returns>
        public override string ToString()
            => Expression;
    }
}
