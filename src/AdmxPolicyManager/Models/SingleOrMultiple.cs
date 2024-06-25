using System;
using System.Collections.Generic;

namespace AdmxPolicyManager.Models
{
    /// <summary>
    /// Represents a container that can hold a single value, multiple values, or a null value.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value(s) to be held in the container.
    /// </typeparam>
    public sealed class SingleOrMultiple<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleOrMultiple{T}"/> class with a null value.
        /// </summary>
        public SingleOrMultiple()
        {
            _isNull = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleOrMultiple{T}"/> class with a single value.
        /// </summary>
        /// <param name="single">The single value.</param>
        public SingleOrMultiple(T single)
        {
            _single = single;
            _isSingle = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleOrMultiple{T}"/> class with multiple values.
        /// </summary>
        /// <param name="multiple">The multiple values.</param>
        public SingleOrMultiple(IEnumerable<T> multiple)
        {
            _multiple = multiple;
        }

        private T _single = default;
        private IEnumerable<T> _multiple = Array.Empty<T>();
        private bool _isSingle = default;
        private bool _isNull = default;

        /// <summary>
        /// Gets a value indicating whether the container has a single value.
        /// </summary>
        public bool IsSingle => _isSingle;

        /// <summary>
        /// Gets a value indicating whether the container has multiple values.
        /// </summary>
        public bool IsNull => _isNull;

        /// <summary>
        /// Gets the single value from the container.
        /// </summary>
        /// <returns>The single value.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the container has a null value or multiple values.</exception>
        public T GetSingleValue()
        {
            if (_isNull) throw new InvalidOperationException("This container has a null value.");
            if (!_isSingle) throw new InvalidOperationException("This container has multiple values.");
            return _single;
        }

        /// <summary>
        /// Tries to get the single value from the container.
        /// </summary>
        /// <param name="var">The single value.</param>
        /// <returns><c>true</c> if the single value is successfully retrieved; otherwise, <c>false</c>.</returns>
        public bool TryGetSingleValue(out T var)
        {
            @var = default(T);
            if (_isNull) return false;
            if (!_isSingle) return false;
            @var = _single;
            return true;
        }

        /// <summary>
        /// Gets the multiple values from the container.
        /// </summary>
        /// <returns>The multiple values.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the container has a null value or a single value.</exception>
        public IEnumerable<T> GetMultipleValues()
        {
            if (_isNull) throw new InvalidOperationException("This container has a null value.");
            if (_isSingle) throw new InvalidOperationException("This container has a single value.");
            return _multiple;
        }

        /// <summary>
        /// Tries to get the multiple values from the container.
        /// </summary>
        /// <param name="var">The multiple values.</param>
        /// <returns><c>true</c> if the multiple values are successfully retrieved; otherwise, <c>false</c>.</returns>
        public bool TryGetMultipleValues(out IEnumerable<T> var)
        {
            @var = Array.Empty<T>();
            if (_isNull) return false;
            if (_isSingle) return false;
            @var = _multiple;
            return true;
        }
    }
}
