using AdmxPolicyManager.Contracts.Presentation;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AdmxPolicyManager.Models.Presentation
{
    /// <summary>
    /// Represents the information about a policy presentation.
    /// </summary>
    public sealed class PolicyPresentationInfo
    {
        internal PolicyPresentationInfo() { }

        // Illustrated Examples: https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-gpreg/9cf23695-efa1-43dc-b035-198160ffbb13

        /// <summary>
        /// Gets or sets the ID of the policy presentation.
        /// </summary>
        public string Id { get; internal set; } = default;

        private readonly List<IPresentationControlInfo> _controls = new List<IPresentationControlInfo>();
        private readonly PresentationControlHasRefIdCollection<PolicyCheckBoxInfo> _checkBoxes = new PresentationControlHasRefIdCollection<PolicyCheckBoxInfo>();
        private readonly PresentationControlHasRefIdCollection<PolicyTextBoxInfo> _textBoxes = new PresentationControlHasRefIdCollection<PolicyTextBoxInfo>();
        private readonly PresentationControlHasRefIdCollection<PolicyDecimalTextBoxInfo> _decimalTextBoxes = new PresentationControlHasRefIdCollection<PolicyDecimalTextBoxInfo>();
        private readonly PresentationControlHasRefIdCollection<PolicyLongDecimalTextBoxInfo> _longDecimalTextBoxes = new PresentationControlHasRefIdCollection<PolicyLongDecimalTextBoxInfo>();
        private readonly PresentationControlHasRefIdCollection<PolicyComboBoxInfo> _comboBoxes = new PresentationControlHasRefIdCollection<PolicyComboBoxInfo>();
        private readonly PresentationControlHasRefIdCollection<PolicyDropdownListInfo> _dropdownLists = new PresentationControlHasRefIdCollection<PolicyDropdownListInfo>();
        private readonly PresentationControlHasRefIdCollection<PolicyListBoxInfo> _listBoxes = new PresentationControlHasRefIdCollection<PolicyListBoxInfo>();
        private readonly PresentationControlHasRefIdCollection<PolicyMultiTextBoxInfo> _multiTextBoxes = new PresentationControlHasRefIdCollection<PolicyMultiTextBoxInfo>();

        internal List<IPresentationControlInfo> ControlsInternal => _controls;
        internal PresentationControlHasRefIdCollection<PolicyCheckBoxInfo> CheckBoxesInternal => _checkBoxes;
        internal PresentationControlHasRefIdCollection<PolicyTextBoxInfo> TextBoxesInternal => _textBoxes;
        internal PresentationControlHasRefIdCollection<PolicyDecimalTextBoxInfo> DecimalTextBoxesInternal => _decimalTextBoxes;
        internal PresentationControlHasRefIdCollection<PolicyLongDecimalTextBoxInfo> LongDecimalTextBoxesInternal => _longDecimalTextBoxes;
        internal PresentationControlHasRefIdCollection<PolicyComboBoxInfo> ComboBoxesInternal => _comboBoxes;
        internal PresentationControlHasRefIdCollection<PolicyDropdownListInfo> DropdownListsInternal => _dropdownLists;
        internal PresentationControlHasRefIdCollection<PolicyListBoxInfo> ListBoxesInternal => _listBoxes;
        internal PresentationControlHasRefIdCollection<PolicyMultiTextBoxInfo> MultiTextBoxesInternal => _multiTextBoxes;

        private IReadOnlyList<IPresentationControlInfo> _controlsReadOnly;
        private IReadOnlyList<PolicyCheckBoxInfo> _checkBoxesReadOnly;
        private IReadOnlyList<PolicyTextBoxInfo> _textBoxesReadOnly;
        private IReadOnlyList<PolicyDecimalTextBoxInfo> _decimalTextBoxesReadOnly;
        private IReadOnlyList<PolicyLongDecimalTextBoxInfo> _longDecimalTextBoxesReadOnly;
        private IReadOnlyList<PolicyComboBoxInfo> _comboBoxesReadOnly;
        private IReadOnlyList<PolicyDropdownListInfo> _dropdownListReadOnly;
        private IReadOnlyList<PolicyListBoxInfo> _listBoxesReadOnly;
        private IReadOnlyList<PolicyMultiTextBoxInfo> _multiTextBoxesReadOnly;

        /// <summary>
        /// Gets the read-only list of presentation controls.
        /// </summary>
        public IReadOnlyList<IPresentationControlInfo> Controls => _controlsReadOnly = _controlsReadOnly ?? _controls.AsReadOnly();

        /// <summary>
        /// Gets the read-only list of policy check boxes.
        /// </summary>
        public IReadOnlyList<PolicyCheckBoxInfo> CheckBoxes => _checkBoxesReadOnly = _checkBoxesReadOnly ?? new ReadOnlyCollection<PolicyCheckBoxInfo>(_checkBoxes);

        /// <summary>
        /// Gets the read-only list of policy text boxes.
        /// </summary>
        public IReadOnlyList<PolicyTextBoxInfo> TextBoxes => _textBoxesReadOnly = _textBoxesReadOnly ?? new ReadOnlyCollection<PolicyTextBoxInfo>(_textBoxes);

        /// <summary>
        /// Gets the read-only list of policy decimal text boxes.
        /// </summary>
        public IReadOnlyList<PolicyDecimalTextBoxInfo> DecimalTextBoxes => _decimalTextBoxesReadOnly = _decimalTextBoxesReadOnly ?? new ReadOnlyCollection<PolicyDecimalTextBoxInfo>(_decimalTextBoxes);

        /// <summary>
        /// Gets the read-only list of policy long decimal text boxes.
        /// </summary>
        public IReadOnlyList<PolicyLongDecimalTextBoxInfo> LongDecimalTextBoxes => _longDecimalTextBoxesReadOnly = _longDecimalTextBoxesReadOnly ?? new ReadOnlyCollection<PolicyLongDecimalTextBoxInfo>(_longDecimalTextBoxes);

        /// <summary>
        /// Gets the read-only list of policy combo boxes.
        /// </summary>
        public IReadOnlyList<PolicyComboBoxInfo> ComboBoxes => _comboBoxesReadOnly = _comboBoxesReadOnly ?? new ReadOnlyCollection<PolicyComboBoxInfo>(_comboBoxes);

        /// <summary>
        /// Gets the read-only list of policy dropdown lists.
        /// </summary>
        public IReadOnlyList<PolicyDropdownListInfo> DropdownLists => _dropdownListReadOnly = _dropdownListReadOnly ?? new ReadOnlyCollection<PolicyDropdownListInfo>(_dropdownLists);

        /// <summary>
        /// Gets the read-only list of policy list boxes.
        /// </summary>
        public IReadOnlyList<PolicyListBoxInfo> ListBoxes => _listBoxesReadOnly = _listBoxesReadOnly ?? new ReadOnlyCollection<PolicyListBoxInfo>(_listBoxes);

        /// <summary>
        /// Gets the read-only list of policy multi text boxes.
        /// </summary>
        public IReadOnlyList<PolicyMultiTextBoxInfo> MultiTextBoxes => _multiTextBoxesReadOnly = _multiTextBoxesReadOnly ?? new ReadOnlyCollection<PolicyMultiTextBoxInfo>(_multiTextBoxes);

        /// <summary>
        /// Tries to get the policy check box with the specified reference ID.
        /// </summary>
        /// <param name="refId">The reference ID of the policy check box.</param>
        /// <param name="ctrl">When this method returns, contains the policy check box with the specified reference ID, if found; otherwise, null.</param>
        /// <returns>true if the policy check box with the specified reference ID is found; otherwise, false.</returns>
        public bool TryGetCheckBox(string refId, out PolicyCheckBoxInfo ctrl)
            => _checkBoxes.TryGetValue(refId, out ctrl);

        /// <summary>
        /// Tries to get the policy text box with the specified reference ID.
        /// </summary>
        /// <param name="refId">The reference ID of the policy text box.</param>
        /// <param name="ctrl">When this method returns, contains the policy text box with the specified reference ID, if found; otherwise, null.</param>
        /// <returns>true if the policy text box with the specified reference ID is found; otherwise, false.</returns>
        public bool TryGetTextBox(string refId, out PolicyTextBoxInfo ctrl)
            => _textBoxes.TryGetValue(refId, out ctrl);

        /// <summary>
        /// Tries to get the policy decimal text box with the specified reference ID.
        /// </summary>
        /// <param name="refId">The reference ID of the policy decimal text box.</param>
        /// <param name="ctrl">When this method returns, contains the policy decimal text box with the specified reference ID, if found; otherwise, null.</param>
        /// <returns>true if the policy decimal text box with the specified reference ID is found; otherwise, false.</returns>
        public bool TryGetDecimalTextBox(string refId, out PolicyDecimalTextBoxInfo ctrl)
            => _decimalTextBoxes.TryGetValue(refId, out ctrl);

        /// <summary>
        /// Tries to get the policy long decimal text box with the specified reference ID.
        /// </summary>
        /// <param name="refId">The reference ID of the policy long decimal text box.</param>
        /// <param name="ctrl">When this method returns, contains the policy long decimal text box with the specified reference ID, if found; otherwise, null.</param>
        /// <returns>true if the policy long decimal text box with the specified reference ID is found; otherwise, false.</returns>
        public bool TryGetLongDecimalTextBox(string refId, out PolicyLongDecimalTextBoxInfo ctrl)
            => _longDecimalTextBoxes.TryGetValue(refId, out ctrl);

        /// <summary>
        /// Tries to get the policy combo box with the specified reference ID.
        /// </summary>
        /// <param name="refId">The reference ID of the policy combo box.</param>
        /// <param name="ctrl">When this method returns, contains the policy combo box with the specified reference ID, if found; otherwise, null.</param>
        /// <returns>true if the policy combo box with the specified reference ID is found; otherwise, false.</returns>
        public bool TryGetComboBox(string refId, out PolicyComboBoxInfo ctrl)
            => _comboBoxes.TryGetValue(refId, out ctrl);

        /// <summary>
        /// Tries to get the policy dropdown list with the specified reference ID.
        /// </summary>
        /// <param name="refId">The reference ID of the policy dropdown list.</param>
        /// <param name="ctrl">When this method returns, contains the policy dropdown list with the specified reference ID, if found; otherwise, null.</param>
        /// <returns>true if the policy dropdown list with the specified reference ID is found; otherwise, false.</returns>
        public bool TryGetDropdownList(string refId, out PolicyDropdownListInfo ctrl)
            => _dropdownLists.TryGetValue(refId, out ctrl);

        /// <summary>
        /// Tries to get the policy list box with the specified reference ID.
        /// </summary>
        /// <param name="refId">The reference ID of the policy list box.</param>
        /// <param name="ctrl">When this method returns, contains the policy list box with the specified reference ID, if found; otherwise, null.</param>
        /// <returns>true if the policy list box with the specified reference ID is found; otherwise, false.</returns>
        public bool TryGetListBox(string refId, out PolicyListBoxInfo ctrl)
            => _listBoxes.TryGetValue(refId, out ctrl);

        /// <summary>
        /// Tries to get the policy multi text box with the specified reference ID.
        /// </summary>
        /// <param name="refId">The reference ID of the policy multi text box.</param>
        /// <param name="ctrl">When this method returns, contains the policy multi text box with the specified reference ID, if found; otherwise, null.</param>
        /// <returns>true if the policy multi text box with the specified reference ID is found; otherwise, false.</returns>
        public bool TryGetMultiTextBox(string refId, out PolicyMultiTextBoxInfo ctrl)
            => _multiTextBoxes.TryGetValue(refId, out ctrl);
    }
}
