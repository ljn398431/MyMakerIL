namespace BloxGameSystems
{
	public interface ICharacterAttributesOwner
	{
		/// <summary> Return the value of Character Attribute. </summary>
		float GetAttributeValue(string ident);

		/// <summary> Return the max value of Character Attribute. </summary>
		float GetAttributeMaxValue(string ident);

		/// <summary> Set the value of Character Attribute. </summary>
		void SetAttributeValue(string ident, float value);

		/// <summary> Set the max value of Character Attribute. </summary>
		void SetAttributeMaxValue(string ident, float value);

		/// <summary> Change the value of Character Attribute by certain amount. You can send either a negative or positive value depending on whether you want to add to or remove from the attribute's value. </summary>
		void ChangeAttributeValueBy(string ident, float value);

		/// <summary> Change the max value of Character Attribute by certain amount. You can send either a negative or positive value depending on whether you want to add to or remove from the attribute's value. </summary>
		void ChangeAttributeMaxValueBy(string ident, float value);

		/// <summary> Returns reference to a character's attribute object </summary>
		CharacterAttribute GetAttribute(string ident);

		/// <summary> Returns reference to a character's attribute object </summary>
		CharacterAttribute GetAttribute(int id);
	}
}
