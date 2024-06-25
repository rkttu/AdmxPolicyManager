using System;

namespace AdmxPolicyManager.Models.Policies
{
    /// <summary>
    /// Represents a reference to an entity.
    /// </summary>
    public sealed class EntityReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityReference"/> class with the specified expression.
        /// </summary>
        /// <param name="expression">The expression representing the entity reference.</param>
        /// <exception cref="ArgumentException">Thrown when the expression is null or whitespace.</exception>
        public EntityReference(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Expression cannot be null or whitespace string.", nameof(expression));

            var colonIndex = expression.IndexOf(':');
            var prefix = string.Empty;
            var key = string.Empty;

            if (colonIndex < 0)
            {
                prefix = string.Empty;
                key = expression;
            }
            else
            {
                prefix = expression.Substring(0, colonIndex);
                key = expression.Substring(colonIndex + 1);
            }

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace string.", nameof(key));

            _prefix = prefix ?? string.Empty;
            _key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityReference"/> class with the specified prefix and key.
        /// </summary>
        /// <param name="prefix">The prefix of the entity reference.</param>
        /// <param name="key">The key of the entity reference.</param>
        /// <exception cref="ArgumentException">Thrown when the key is null or whitespace.</exception>
        public EntityReference(string prefix, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace string.", nameof(key));

            _prefix = prefix ?? string.Empty;
            _key = key;
        }

        private string _prefix;
        private string _key;

        /// <summary>
        /// Gets the expression representing the entity reference.
        /// </summary>
        public string Expression
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_prefix))
                    return _key;
                else
                    return $"{_prefix}:{_key}";
            }
        }

        /// <summary>
        /// Gets the prefix of the entity reference.
        /// </summary>
        public string Prefix => _prefix;

        /// <summary>
        /// Gets the key of the entity reference.
        /// </summary>
        public string Key => _key;

        /// <summary>
        /// Returns a string that represents the current entity reference.
        /// </summary>
        /// <returns>A string that represents the current entity reference.</returns>
        public override string ToString()
            => Expression;
    }
}
