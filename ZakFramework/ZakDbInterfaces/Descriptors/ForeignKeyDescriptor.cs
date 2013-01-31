

namespace ZakDb.Descriptors
{
	public class ForeignKeyDescriptor
	{
		public string[] OwnerFields { get; private set; }
		public string[] OwnedFields { get; private set; }
		public TableDescriptor Descriptor { get; private set; }
		public ForeignKeyMultplicity Multplicity { get; private set; }
		public bool Embedded { get; private set; }

		public ForeignKeyDescriptor(TableDescriptor descriptor,
			ForeignKeyMultplicity multplicity = ForeignKeyMultplicity.OneToOne,bool embedded = false)
		{
			Descriptor = descriptor;
			Multplicity = multplicity;
			Embedded = embedded;
		}

		public void SetFields(string[] ownerFields, string[] ownedFields)
		{
			Descriptor.VerifyFieldsPresence(ownedFields);
			OwnerFields = ownerFields;
			OwnedFields = ownedFields;
		}

		public void SetFields(string ownerField, string ownedField)
		{
			var ownerFields = new[] { ownerField };
			var ownedFields = new[] { ownedField };
			SetFields(ownerFields,ownedFields);
		}
	}
}
